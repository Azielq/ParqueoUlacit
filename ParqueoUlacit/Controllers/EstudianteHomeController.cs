using ParqueoUlacit.Models;
using ParqueoUlacit.Models.TableViewModel;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

namespace ParqueoUlacit.Controllers
{
    public class EstudianteHomeController : Controller
    {
        // GET: EstudianteHome
        public ActionResult Index(int? mes, int? año)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // Si no se proporciona un mes, usar el mes actual
            if (!mes.HasValue)
            {
                mes = DateTime.Now.Month;
            }

            // Si no se proporciona un año, usar el año actual
            if (!año.HasValue)
            {
                año = DateTime.Now.Year;
            }

            int usuarioID = (int)Session["UsuarioID"];
            using (var db = new ParqueoUlacitEntities())
            {
                // Obtener vehículos del estudiante
                var vehiculos = db.Vehiculoes
                    .Where(v => v.UsuarioID == usuarioID)
                    .Select(v => v.VehiculoID)
                    .ToList();

                // Obtener bitácora solo de los vehículos del estudiante, filtrada por mes y año
                var bitacora = db.BitacoraParqueos
                    .Where(b => b.VehiculoID.HasValue &&
                               vehiculos.Contains(b.VehiculoID.Value) &&
                               b.Fecha.HasValue &&
                               b.Fecha.Value.Month == mes.Value &&
                               b.Fecha.Value.Year == año.Value)
                    .OrderByDescending(b => b.Fecha)
                    .Take(100) // Incrementamos el límite para mostrar más resultados del mes seleccionado
                    .ToList();

                // Mapear a ViewModel
                var bitacoraViewModel = bitacora.Select(b => new BitacoraParqueoTableViewModel
                {
                    BitacoraID = b.BitacoraID,
                    VehiculoID = b.VehiculoID,
                    Fecha = b.Fecha,
                    TipoMovimiento = b.TipoMovimiento,
                    EstadoIngreso = b.EstadoIngreso,
                    NumeroPlaca = b.NumeroPlaca,
                    Vehiculo = b.Vehiculo
                }).ToList();

                // Preparar lista de meses para el dropdown
                var mesesList = new List<SelectListItem>();
                string[] nombresMeses = {
                    "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                    "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
                };

                for (int i = 1; i <= 12; i++)
                {
                    mesesList.Add(new SelectListItem
                    {
                        Value = i.ToString(),
                        Text = nombresMeses[i - 1],
                        Selected = i == mes.Value
                    });
                }

                // Preparar lista de años
                var añoslist = new List<SelectListItem>();
                int añoActual = DateTime.Now.Year;
                for (int i = añoActual - 5; i <= añoActual; i++)
                {
                    añoslist.Add(new SelectListItem
                    {
                        Value = i.ToString(),
                        Text = i.ToString(),
                        Selected = i == año.Value
                    });
                }

                // Pasar los valores a ViewBag para usar en la vista
                ViewBag.Meses = mesesList;
                ViewBag.Años = añoslist;
                ViewBag.MesSeleccionado = mes.Value;
                ViewBag.AñoSeleccionado = año.Value;

                return View(bitacoraViewModel);
            }
        }
    }
}