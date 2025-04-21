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

        // GET: Usuarios
        public ActionResult Index()
        {
            var usuarios = db.Usuarios
                .Select(u => new UsuarioTableViewModel
                {
                    UsuarioID = u.UsuarioID,
                    Nombre = u.Nombre,
                    Correo = u.Correo,
                    Identificacion = u.Identificacion,
                    FechaNacimiento = u.FechaNacimiento,
                    RolID = u.RolID,

                })
                .ToList();

            return View(usuarios);
        }


        // GET: Usuarios/Create or Usuarios/Crear
        [ActionName("Create")]
        public ActionResult Create()
        {
            // Usar una lista fuerte para el rol
            var roles = db.Rols.Select(r => new { r.RolID, r.NombreRol }).ToList();
            ViewBag.RolID = new SelectList(roles, "RolID", "NombreRol");

            return View();
        }

        // Adding Spanish alias for Create
        [ActionName("Crear")]
        public ActionResult CreateSpanish()
        {
            return Create();
        }

        // POST: Usuarios/Create or Usuarios/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public ActionResult Create_Post([Bind(Include = "Nombre,Correo,FechaNacimiento,Identificacion,Clave,RolID")] UsuarioTableViewModel usuario)
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

        // Adding Spanish alias for Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Crear")]
        public ActionResult CreateSpanish_Post([Bind(Include = "Nombre,Correo,FechaNacimiento,Identificacion,Clave,RolID")] UsuarioTableViewModel usuario)
        {
            return Create_Post(usuario);
        }

        // GET: Usuarios/Edit/5 or Usuarios/Editar/5
        [ActionName("Edit")]
        public ActionResult Edit(int id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            // Map database entity to view model
            var usuarioViewModel = new UsuarioTableViewModel
            {
                UsuarioID = usuario.UsuarioID,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                FechaNacimiento = usuario.FechaNacimiento,
                Identificacion = usuario.Identificacion,
                RolID = usuario.RolID  // Asegúrate de mapear el RolID
            };

            // Preparar la lista de roles para el dropdown
            var roles = db.Rols.Select(r => new { r.RolID, r.NombreRol }).ToList();
            ViewBag.RolID = new SelectList(roles, "RolID", "NombreRol", usuario.RolID);

            return View("Edit", usuarioViewModel);
        }

        // Adding Spanish alias for Edit
        [ActionName("Editar")]
        public ActionResult EditSpanish(int id)
        {
            return Edit(id);
        }

        // POST: Usuarios/Edit/5 or Usuarios/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public ActionResult Edit_Post([Bind(Include = "UsuarioID,Nombre,Correo,FechaNacimiento,Identificacion,RolID")] UsuarioTableViewModel usuario)
        {
            if (ModelState.IsValid)
            {
                var usuarioExistente = db.Usuarios.Find(usuario.UsuarioID);
                if (usuarioExistente == null)
                {
                    return HttpNotFound();
                }

                // Actualizar los campos del usuario existente
                usuarioExistente.Nombre = usuario.Nombre;
                usuarioExistente.Correo = usuario.Correo;
                usuarioExistente.FechaNacimiento = usuario.FechaNacimiento;
                usuarioExistente.Identificacion = usuario.Identificacion;
                usuarioExistente.RolID = usuario.RolID;  // Asegúrate de actualizar el RolID

                db.SaveChanges();
                return RedirectToAction("Index", "AdminHome");
            }

            // Si el modelo no es válido, mantener el rol seleccionado y mostrar el formulario de nuevo
            var roles = db.Rols.Select(r => new { r.RolID, r.NombreRol }).ToList();
            ViewBag.RolID = new SelectList(roles, "RolID", "NombreRol", usuario.RolID);
            return View(usuario);
        }

        // Adding Spanish alias for Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Editar")]
        public ActionResult EditSpanish_Post([Bind(Include = "UsuarioID,Nombre,Correo,FechaNacimiento,Identificacion,RolID")] UsuarioTableViewModel usuario)
        {
            return Edit_Post(usuario);
        }

        // Los métodos GET para Delete/Eliminar no son necesarios ya que se utiliza un modal para confirmar

        // POST: Usuarios/Delete/5 or Usuarios/Eliminar/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            // Primero eliminar los vehículos asociados al usuario
            var vehiculosUsuario = db.Vehiculoes.Where(v => v.UsuarioID == id).ToList();
            foreach (var vehiculo in vehiculosUsuario)
            {
                db.Vehiculoes.Remove(vehiculo);
            }

            // Luego eliminar el usuario
            db.Usuarios.Remove(usuario);
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