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

        
    }
}