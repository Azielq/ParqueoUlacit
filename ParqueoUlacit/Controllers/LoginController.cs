using ParqueoUlacit.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ParqueoUlacit.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string Correo, string Clave)
        {
            using (var db = new ParqueoUlacitEntities())
            {
                var usuario = db.Usuarios.FirstOrDefault(u => u.Correo == Correo && u.Clave == Clave);

                if (usuario != null)
                {
                    // Verificar si la clave sigue siendo la predeterminada
                    if (usuario.Clave == "Ulacit123")
                    {
                        Session["UsuarioID"] = usuario.UsuarioID;
                        return RedirectToAction("ChangePassword", "Login");
                    }

                    // Guardar datos en la sesión
                    Session["UsuarioID"] = usuario.UsuarioID;
                    Session["Nombre"] = usuario.Nombre;
                    Session["RolID"] = usuario.RolID;

                    // Redireccionar según el rol del usuario
                    return RedirigirSegunRol(usuario.RolID);
                }
                else
                {
                    ModelState.AddModelError("", "Correo o contraseña incorrectos.");
                    return View();
                }
            }
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string nuevaClave)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login");
            }

            using (var db = new ParqueoUlacitEntities())
            {
                int usuarioID = (int)Session["UsuarioID"];
                var usuario = db.Usuarios.Find(usuarioID);

                if (usuario != null)
                {
                    usuario.Clave = nuevaClave; // Aquí puedes aplicar encriptación si es necesario
                    usuario.ClaveCambiada = true;

                    db.SaveChanges(); // Guardar cambios en la base de datos

                    return RedirigirSegunRol(usuario.RolID);
                }
            }

            ModelState.AddModelError("", "Error al cambiar la contraseña.");
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        private ActionResult RedirigirSegunRol(int? rolID)
        {
            switch (rolID)
            {
                case 1:
                    return RedirectToAction("Index", "AdminHome");
                case 2:
                    return RedirectToAction("Index", "SeguridadHome");
                case 3:
                    return RedirectToAction("Index", "AdministrativoHome");
                case 4:
                    return RedirectToAction("Index", "EstudianteHome");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }
    }
}
