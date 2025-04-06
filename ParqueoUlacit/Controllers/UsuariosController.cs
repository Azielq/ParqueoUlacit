using ParqueoUlacit.Models;
using ParqueoUlacit.Models.TableViewModel;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ParqueoUlacit.Controllers
{
    public class UsuariosController : Controller
    {
        private ParqueoUlacitEntities db = new ParqueoUlacitEntities();

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            // Usar una lista fuerte para el rol
            var roles = db.Rols.Select(r => new { r.RolID, r.NombreRol }).ToList();
            ViewBag.RolID = new SelectList(roles, "RolID", "NombreRol");

            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Nombre,Correo,FechaNacimiento,Identificacion,Clave,RolID")] UsuarioTableViewModel usuario)
        {
            if (ModelState.IsValid)
            {
                var nuevoUsuario = new Usuario
                {
                    Nombre = usuario.Nombre,
                    Correo = usuario.Correo,
                    FechaNacimiento = usuario.FechaNacimiento,
                    Identificacion = usuario.Identificacion,
                    Clave = "Ulacit123", 
                    ClaveCambiada = false, 
                    RolID = usuario.RolID
                };

                db.Usuarios.Add(nuevoUsuario);
                db.SaveChanges();

                return RedirectToAction("Index", "AdminHome");
            }

            // Si el modelo no es válido, mantener el rol seleccionado y mostrar el formulario de nuevo
            var roles = db.Rols.Select(r => new { r.RolID, r.NombreRol }).ToList();
            ViewBag.RolID = new SelectList(roles, "RolID", "NombreRol", usuario.RolID);
            return View(usuario);
        }



    }
}
