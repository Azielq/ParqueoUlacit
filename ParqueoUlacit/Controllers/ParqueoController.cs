using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParqueoUlacit.Models;
using ParqueoUlacit.Models.TableViewModel;

namespace ParqueoUlacit.Controllers
{
    public class ParqueoController : Controller
    {
        private ParqueoUlacitEntities db = new ParqueoUlacitEntities();

        // GET: Parqueo
        public ActionResult Index()
        {
            // Verificar si el usuario está autenticado y tiene permisos (admin)
            if (Session["RolID"] == null || (int)Session["RolID"] != 1)
            {
                return RedirectToAction("Login", "Login");
            }

            // Obtener todos los parqueos
            var parqueos = db.Parqueos.Select(p => new ParqueoTableViewModel
            {
                ParqueoID = p.ParqueoID,
                Nombre_Parqueo = p.Nombre_Parqueo,
                Ubicacion = p.Ubicacion,
                Espacios_Carros = p.Espacios_Carros,
                Espacios_Moto = p.Espacios_Moto,
                Espacios_Ley7600 = p.Espacios_Ley7600
            }).ToList();

            return View(parqueos);
        }

        // GET: Parqueo/Create
        public ActionResult Create()
        {
            // Verificar si el usuario está autenticado y tiene permisos (admin)
            if (Session["RolID"] == null || (int)Session["RolID"] != 1)
            {
                return RedirectToAction("Login", "Login");
            }

            return View(new ParqueoTableViewModel());
        }

        // POST: Parqueo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ParqueoTableViewModel parqueoViewModel)
        {
            // Verificar si el usuario está autenticado y tiene permisos (admin)
            if (Session["RolID"] == null || (int)Session["RolID"] != 1)
            {
                return RedirectToAction("Login", "Login");
            }

            if (ModelState.IsValid)
            {
                // Crear nuevo parqueo con los datos del viewmodel
                var parqueo = new Parqueo
                {
                    Nombre_Parqueo = parqueoViewModel.Nombre_Parqueo,
                    Ubicacion = parqueoViewModel.Ubicacion,
                    Espacios_Carros = parqueoViewModel.Espacios_Carros,
                    Espacios_Moto = parqueoViewModel.Espacios_Moto,
                    Espacios_Ley7600 = parqueoViewModel.Espacios_Ley7600
                };

                // Agregar y guardar en la base de datos
                db.Parqueos.Add(parqueo);
                db.SaveChanges();

                // Redireccionar al panel de administración
                return RedirectToAction("Index", "AdminHome");
            }

            // Si hay errores de validación, volver a mostrar el formulario
            return View(parqueoViewModel);
        }



        // GET: Parqueo/Edit/5
        public ActionResult Edit(int id)
        {
            // Verificar si el usuario está autenticado y tiene permisos (admin)
            if (Session["RolID"] == null || (int)Session["RolID"] != 1)
            {
                return RedirectToAction("Login", "Login");
            }

            // Buscar el parqueo por su ID
            var parqueo = db.Parqueos.Find(id);
            if (parqueo == null)
            {
                return HttpNotFound();
            }

            // Mapear a ViewModel
            var parqueoViewModel = new ParqueoTableViewModel
            {
                ParqueoID = parqueo.ParqueoID,
                Nombre_Parqueo = parqueo.Nombre_Parqueo,
                Ubicacion = parqueo.Ubicacion,
                Espacios_Carros = parqueo.Espacios_Carros,
                Espacios_Moto = parqueo.Espacios_Moto,
                Espacios_Ley7600 = parqueo.Espacios_Ley7600
            };

            return View(parqueoViewModel);
        }

        // POST: Parqueo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ParqueoTableViewModel parqueoViewModel)
        {
            // Verificar si el usuario está autenticado y tiene permisos (admin)
            if (Session["RolID"] == null || (int)Session["RolID"] != 1)
            {
                return RedirectToAction("Login", "Login");
            }

            if (ModelState.IsValid)
            {
                // Buscar el parqueo existente
                var parqueo = db.Parqueos.Find(parqueoViewModel.ParqueoID);
                if (parqueo == null)
                {
                    return HttpNotFound();
                }

                // Actualizar los campos
                parqueo.Nombre_Parqueo = parqueoViewModel.Nombre_Parqueo;
                parqueo.Ubicacion = parqueoViewModel.Ubicacion;
                parqueo.Espacios_Carros = parqueoViewModel.Espacios_Carros;
                parqueo.Espacios_Moto = parqueoViewModel.Espacios_Moto;
                parqueo.Espacios_Ley7600 = parqueoViewModel.Espacios_Ley7600;

                // Guardar los cambios
                db.SaveChanges();

                // Redireccionar al panel de administración
                return RedirectToAction("Index", "AdminHome");
            }

            // Si hay errores de validación, volver a mostrar el formulario
            return View(parqueoViewModel);
        }

        // POST: Parqueo/Delete/5 or Parqueo/Eliminar/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Verificar si el usuario está autenticado y tiene permisos (admin)
            if (Session["RolID"] == null || (int)Session["RolID"] != 1)
            {
                return RedirectToAction("Login", "Login");
            }

            Parqueo parqueo = db.Parqueos.Find(id);
            if (parqueo == null)
            {
                return HttpNotFound();
            }

            // Verificar si hay vehículos asociados a este parqueo
            bool tieneVehiculosAsociados = db.Vehiculoes.Any(v => v.ParqueoID == id);
            if (tieneVehiculosAsociados)
            {
                // Redirigir con un mensaje de error (necesitaríamos configurar TempData para mostrar el mensaje)
                TempData["ErrorMessage"] = "No se puede eliminar el parqueo porque tiene vehículos asociados.";
                return RedirectToAction("Index");
            }

            // Verificar si hay bitácoras de parqueo asociadas
            var bitacorasParqueo = db.BitacoraParqueos.Where(b => b.ParqueoID == id).ToList();
            foreach (var bitacora in bitacorasParqueo)
            {
                db.BitacoraParqueos.Remove(bitacora);
            }

            // Eliminar el parqueo
            db.Parqueos.Remove(parqueo);
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