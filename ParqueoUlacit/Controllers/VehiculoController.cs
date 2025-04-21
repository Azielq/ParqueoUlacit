using ParqueoUlacit.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using ParqueoUlacit.Models.ViewModel;
using System.Collections.Generic;

namespace ParqueoUlacit.Controllers
{
    public class VehiculoController : Controller
    {
        private ParqueoUlacitEntities db = new ParqueoUlacitEntities();

        // GET: Vehiculo
        public ActionResult Index()
        {
            var vehiculos = db.Vehiculoes
                .Select(v => new VehiculoViewModel
                {
                    VehiculoID = v.VehiculoID,
                    Marca = v.Marca,
                    Color = v.Color,
                    NumeroPlaca = v.NumeroPlaca,
                    Tipo = v.Tipo,
                    UsuarioID = v.UsuarioID,
                    UsaEspacioLey7600 = v.UsaEspacioLey7600,
                    Estado = v.Estado,
                    ParqueoID = v.ParqueoID,

                })
                .ToList();

            return View(vehiculos);
        }

        // GET: Vehiculo/Create
        public ActionResult Create()
        {
            var usuarios = db.Usuarios
                              .Where(u => u.Vehiculoes.Count() < 2)
                              .Select(u => new { u.UsuarioID, u.Nombre })
                              .ToList();

            ViewBag.UsuarioID = new SelectList(usuarios, "UsuarioID", "Nombre");

            // Inicializar el modelo con valores predeterminados
            var vehiculoViewModel = new VehiculoViewModel
            {
                UsaEspacioLey7600 = false
            };

            return View(vehiculoViewModel);
        }

        // POST: Vehiculo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Marca,Color,NumeroPlaca,Tipo,UsuarioID,UsaEspacioLey7600,Estado,ParqueoID")] VehiculoViewModel vehiculoViewModel)
        {
            if (ModelState.IsValid)
            {
                // Buscar vehículos con la misma placa y tipo
                var vehiculosExistentes = db.Vehiculoes
                    .Where(v => v.NumeroPlaca == vehiculoViewModel.NumeroPlaca && v.Tipo == vehiculoViewModel.Tipo)
                    .ToList();

                if (vehiculosExistentes.Any())
                {
                    // Verificar si alguno de los vehículos encontrados tiene UsuarioID asignado
                    bool existeVehiculoConUsuario = vehiculosExistentes.Any(v => v.UsuarioID != null);

                    if (existeVehiculoConUsuario)
                    {
                        // Si hay un vehículo con la misma placa y tipo que ya tiene un UsuarioID asignado
                        ModelState.AddModelError("NumeroPlaca", $"Ya existe un vehículo registrado de tipo {vehiculoViewModel.Tipo} con la placa {vehiculoViewModel.NumeroPlaca}.");
                        var usuarios = db.Usuarios
                            .Where(u => u.Vehiculoes.Count() < 2)
                            .Select(u => new { u.UsuarioID, u.Nombre })
                            .ToList();
                        ViewBag.UsuarioID = new SelectList(usuarios, "UsuarioID", "Nombre", vehiculoViewModel.UsuarioID);
                        return View(vehiculoViewModel);
                    }
                    else
                    {
                        // En lugar de eliminar los vehículos existentes, actualizaremos sus datos
                        // para convertirlos en el nuevo registro formal
                        var vehiculoExistente = vehiculosExistentes.FirstOrDefault();

                        if (vehiculoExistente != null)
                        {
                            // Actualizar el vehículo existente con los nuevos datos
                            vehiculoExistente.Marca = vehiculoViewModel.Marca;
                            vehiculoExistente.Color = vehiculoViewModel.Color;
                            vehiculoExistente.Tipo = vehiculoViewModel.Tipo;
                            vehiculoExistente.UsuarioID = vehiculoViewModel.UsuarioID;
                            vehiculoExistente.UsaEspacioLey7600 = vehiculoViewModel.UsaEspacioLey7600 ?? false;
                            vehiculoExistente.Estado = vehiculoViewModel.Estado;

                            // Si hay más de un vehículo con la misma placa y tipo, marcamos los otros como inactivos
                            if (vehiculosExistentes.Count > 1)
                            {
                                foreach (var otroVehiculo in vehiculosExistentes.Where(v => v.VehiculoID != vehiculoExistente.VehiculoID))
                                {
                                    otroVehiculo.Estado = "Inactivo";
                                }
                            }

                            // Guardar cambios
                            db.SaveChanges();

                            // Registrar en bitácora la formalización (opcional)
                            var bitacoraEntry = new BitacoraParqueo
                            {
                                VehiculoID = vehiculoExistente.VehiculoID,
                                Fecha = DateTime.Now,
                                TipoMovimiento = "Registro Formal",
                                EstadoIngreso = true,
                                NumeroPlaca = vehiculoExistente.NumeroPlaca,
                                ParqueoID = vehiculoExistente.ParqueoID
                            };

                            db.BitacoraParqueos.Add(bitacoraEntry);
                            db.SaveChanges();

                            return RedirectToAction("Index", "AdminHome");
                        }
                    }
                }

                // Verificar si el usuario ya tiene dos vehículos
                var vehiculosUsuario = db.Vehiculoes.Count(v => v.UsuarioID == vehiculoViewModel.UsuarioID);
                if (vehiculosUsuario >= 2)
                {
                    ModelState.AddModelError("", "El usuario ya tiene el máximo de dos vehículos registrados.");
                    var usuarios = db.Usuarios
                        .Where(u => u.Vehiculoes.Count() < 2)
                        .Select(u => new { u.UsuarioID, u.Nombre })
                        .ToList();
                    ViewBag.UsuarioID = new SelectList(usuarios, "UsuarioID", "Nombre", vehiculoViewModel.UsuarioID);
                    return View(vehiculoViewModel);
                }

                // Asegurarse de que UsaEspacioLey7600 no sea null
                vehiculoViewModel.UsaEspacioLey7600 = vehiculoViewModel.UsaEspacioLey7600 ?? false;

                // Crear un nuevo vehículo con los valores del ViewModel
                var nuevoVehiculo = new Vehiculo
                {
                    Marca = vehiculoViewModel.Marca,
                    Color = vehiculoViewModel.Color,
                    NumeroPlaca = vehiculoViewModel.NumeroPlaca,
                    Tipo = vehiculoViewModel.Tipo,
                    UsuarioID = vehiculoViewModel.UsuarioID,
                    UsaEspacioLey7600 = vehiculoViewModel.UsaEspacioLey7600,
                    Estado = vehiculoViewModel.Estado,
                    ParqueoID = vehiculoViewModel.ParqueoID
                };

                db.Vehiculoes.Add(nuevoVehiculo);
                db.SaveChanges();
                return RedirectToAction("Index", "AdminHome");
            }

            // Si llegamos aquí, algo falló; volver a la vista con los errores
            var usuariosDisponibles = db.Usuarios
                .Where(u => u.Vehiculoes.Count() < 2)
                .Select(u => new { u.UsuarioID, u.Nombre })
                .ToList();
            ViewBag.UsuarioID = new SelectList(usuariosDisponibles, "UsuarioID", "Nombre", vehiculoViewModel.UsuarioID);
            return View(vehiculoViewModel);
        }

        // GET: Vehiculo/Edit/5 or Vehiculo/Editar/5
        [ActionName("Edit")]
        public ActionResult Edit(int id)
        {
            Vehiculo vehiculo = db.Vehiculoes.Find(id);
            if (vehiculo == null)
            {
                return HttpNotFound();
            }

            // Map database entity to view model
            var vehiculoViewModel = new VehiculoViewModel
            {
                VehiculoID = vehiculo.VehiculoID,
                Marca = vehiculo.Marca,
                Color = vehiculo.Color,
                NumeroPlaca = vehiculo.NumeroPlaca,
                Tipo = vehiculo.Tipo,
                UsuarioID = vehiculo.UsuarioID,
                UsaEspacioLey7600 = vehiculo.UsaEspacioLey7600,
                Estado = vehiculo.Estado,
                ParqueoID = vehiculo.ParqueoID
            };

            // Preparar la lista de usuarios para el dropdown
            var usuarios = db.Usuarios
                .Where(u => u.Vehiculoes.Count() < 2 || u.UsuarioID == vehiculo.UsuarioID)
                .Select(u => new { u.UsuarioID, u.Nombre })
                .ToList();

            ViewBag.UsuarioID = new SelectList(usuarios, "UsuarioID", "Nombre", vehiculo.UsuarioID);

            return View("Edit", vehiculoViewModel);
        }

        // Adding Spanish alias for Edit
        [ActionName("Editar")]
        public ActionResult EditSpanish(int id)
        {
            return Edit(id);
        }

        // POST: Vehiculo/Edit/5 or Vehiculo/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public ActionResult Edit_Post([Bind(Include = "VehiculoID,Marca,Color,NumeroPlaca,Tipo,UsuarioID,UsaEspacioLey7600,Estado,ParqueoID")] VehiculoViewModel vehiculo)
        {
            if (ModelState.IsValid)
            {
                var vehiculoExistente = db.Vehiculoes.Find(vehiculo.VehiculoID);
                if (vehiculoExistente == null)
                {
                    return HttpNotFound();
                }

                // Verificar si están intentando cambiar la placa a una ya existente (excepto la suya propia)
                var existePlacaDuplicada = db.Vehiculoes
                    .Any(v => v.NumeroPlaca == vehiculo.NumeroPlaca &&
                              v.Tipo == vehiculo.Tipo &&
                              v.VehiculoID != vehiculo.VehiculoID &&
                              v.UsuarioID != null);

                if (existePlacaDuplicada)
                {
                    ModelState.AddModelError("NumeroPlaca", $"Ya existe un vehículo registrado de tipo {vehiculo.Tipo} con la placa {vehiculo.NumeroPlaca}.");
                    var usuarios = db.Usuarios
                        .Where(u => u.Vehiculoes.Count() < 2 || u.UsuarioID == vehiculoExistente.UsuarioID)
                        .Select(u => new { u.UsuarioID, u.Nombre })
                        .ToList();
                    ViewBag.UsuarioID = new SelectList(usuarios, "UsuarioID", "Nombre", vehiculo.UsuarioID);
                    return View(vehiculo);
                }

                // Actualizar los campos del vehículo existente
                vehiculoExistente.Marca = vehiculo.Marca;
                vehiculoExistente.Color = vehiculo.Color;
                vehiculoExistente.NumeroPlaca = vehiculo.NumeroPlaca;
                vehiculoExistente.Tipo = vehiculo.Tipo;
                vehiculoExistente.UsuarioID = vehiculo.UsuarioID;
                vehiculoExistente.UsaEspacioLey7600 = vehiculo.UsaEspacioLey7600 ?? false;
                vehiculoExistente.Estado = vehiculo.Estado;
                vehiculoExistente.ParqueoID = vehiculo.ParqueoID;

                db.SaveChanges();
                return RedirectToAction("Index", "AdminHome");
            }

            // Si el modelo no es válido, mantener el usuario seleccionado y mostrar el formulario de nuevo
            var usuariosDisponibles = db.Usuarios
                .Where(u => u.Vehiculoes.Count() < 2 || u.UsuarioID == vehiculo.UsuarioID)
                .Select(u => new { u.UsuarioID, u.Nombre })
                .ToList();
            ViewBag.UsuarioID = new SelectList(usuariosDisponibles, "UsuarioID", "Nombre", vehiculo.UsuarioID);
            return View(vehiculo);
        }

        // Adding Spanish alias for Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Editar")]
        public ActionResult EditSpanish_Post([Bind(Include = "VehiculoID,Marca,Color,NumeroPlaca,Tipo,UsuarioID,UsaEspacioLey7600,Estado,ParqueoID")] VehiculoViewModel vehiculo)
        {
            return Edit_Post(vehiculo);
        }

        // POST: Vehiculo/Delete/5 or Vehiculo/Eliminar/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vehiculo vehiculo = db.Vehiculoes.Find(id);
            if (vehiculo == null)
            {
                return HttpNotFound();
            }

            // Primero eliminar registros relacionados en BitacoraParqueo
            var bitacorasRelacionadas = db.BitacoraParqueos.Where(b => b.VehiculoID == id).ToList();
            foreach (var bitacora in bitacorasRelacionadas)
            {
                db.BitacoraParqueos.Remove(bitacora);
            }

            // Luego eliminar registros relacionados en IntentosIngresoFallido
            var intentosFallidos = db.IntentosIngresoFallidos.Where(i => i.VehiculoID == id).ToList();
            foreach (var intento in intentosFallidos)
            {
                db.IntentosIngresoFallidos.Remove(intento);
            }

            // Finalmente eliminar el vehículo
            db.Vehiculoes.Remove(vehiculo);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Adding Spanish alias for Delete POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Eliminar")]
        public ActionResult DeleteSpanish_Post(int id)
        {
            return DeleteConfirmed(id);
        }
    }
}