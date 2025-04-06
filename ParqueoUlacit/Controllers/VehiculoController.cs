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
    }
}