using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParqueoUlacit.Models;
using ParqueoUlacit.Models.TableViewModel;

namespace ParqueoUlacit.Controllers
{
    public class SeguridadHomeController : Controller
    {
        private ParqueoUlacitEntities db = new ParqueoUlacitEntities();

        public ActionResult Index()
        {
            if (Session["RolID"] == null || (int)Session["RolID"] != 2)
            {
                return RedirectToAction("Login", "Login");
            }

            // Si ya se seleccionó un parqueo, mostrar la página principal
            if (Session["ParqueoID"] != null && Session["NombreParqueo"] != null)
            {
                ViewBag.NombreParqueo = Session["NombreParqueo"];
                int parqueoID = (int)Session["ParqueoID"];

                // Obtener información de espacios disponibles
                var parqueo = db.Parqueos.Find(parqueoID);
                if (parqueo != null)
                {
                    // Contar vehículos actuales por tipo
                    int autosOcupados = db.Vehiculoes.Count(v => v.ParqueoID == parqueoID && v.Tipo == "Automovil" && (v.UsaEspacioLey7600 == null || v.UsaEspacioLey7600 == false));
                    int motosOcupadas = db.Vehiculoes.Count(v => v.ParqueoID == parqueoID && v.Tipo == "Moto");
                    int ley7600Ocupados = db.Vehiculoes.Count(v => v.ParqueoID == parqueoID && v.UsaEspacioLey7600 == true);

                    // Establecer variables simples en ViewBag en lugar de objetos anónimos
                    ViewBag.EspaciosAutosTotal = parqueo.Espacios_Carros;
                    ViewBag.EspaciosAutosOcupados = autosOcupados;
                    ViewBag.EspaciosAutosDisponibles = parqueo.Espacios_Carros - autosOcupados;

                    ViewBag.EspaciosMotosTotal = parqueo.Espacios_Moto;
                    ViewBag.EspaciosMotosOcupadas = motosOcupadas;
                    ViewBag.EspaciosMotosDisponibles = parqueo.Espacios_Moto - motosOcupadas;

                    ViewBag.EspaciosLey7600Total = parqueo.Espacios_Ley7600;
                    ViewBag.EspaciosLey7600Ocupados = ley7600Ocupados;
                    ViewBag.EspaciosLey7600Disponibles = parqueo.Espacios_Ley7600 - ley7600Ocupados;
                }

                // Obtener la bitácora del parqueo actual (últimos 10 registros)
                var bitacora = db.BitacoraParqueos
                    .Where(b => b.ParqueoID == parqueoID)
                    .OrderByDescending(b => b.Fecha)
                    .Take(10)
                    .ToList();

                // Mapear a TableViewModel
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

                return View(bitacoraViewModel);
            }

            // Si no hay parqueo seleccionado, redirigir a la selección
            return RedirectToAction("SeleccionarParqueo");
        }

        public ActionResult SeleccionarParqueo()
        {
            if (Session["RolID"] == null || (int)Session["RolID"] != 2)
            {
                return RedirectToAction("Login", "Login");
            }

            // Obtener lista de parqueos disponibles
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

        [HttpPost]
        public ActionResult SeleccionarParqueo(int parqueoID)
        {
            if (Session["RolID"] == null || (int)Session["RolID"] != 2)
            {
                return RedirectToAction("Login", "Login");
            }

            // Buscar el parqueo seleccionado
            var parqueo = db.Parqueos.Find(parqueoID);
            if (parqueo != null)
            {
                // Guardar información del parqueo en la sesión
                Session["ParqueoID"] = parqueo.ParqueoID;
                Session["NombreParqueo"] = parqueo.Nombre_Parqueo;

                // Redirigir al inicio
                return RedirectToAction("Index");
            }

            // Si no se encuentra el parqueo, volver a la selección
            return RedirectToAction("SeleccionarParqueo");
        }

        public ActionResult CambiarParqueo()
        {
            // Limpiar la sesión de parqueo actual
            Session.Remove("ParqueoID");
            Session.Remove("NombreParqueo");

            // Redirigir a la selección de parqueo
            return RedirectToAction("SeleccionarParqueo");
        }

        // Método para obtener la bitácora completa del parqueo
        public ActionResult BitacoraCompleta()
        {
            if (Session["RolID"] == null || (int)Session["RolID"] != 2)
            {
                return RedirectToAction("Login", "Login");
            }

            if (Session["ParqueoID"] == null)
            {
                return RedirectToAction("SeleccionarParqueo");
            }

            int parqueoID = (int)Session["ParqueoID"];
            ViewBag.NombreParqueo = Session["NombreParqueo"];

            // Obtener todos los registros de bitácora para este parqueo
            var bitacora = db.BitacoraParqueos
                .Where(b => b.ParqueoID == parqueoID)
                .OrderByDescending(b => b.Fecha)
                .ToList();

            // Mapear a TableViewModel
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

            return View(bitacoraViewModel);
        }

        // Método para ver vehículos actualmente en el parqueo
        public ActionResult VehiculosActuales()
        {
            if (Session["RolID"] == null || (int)Session["RolID"] != 2)
            {
                return RedirectToAction("Login", "Login");
            }

            if (Session["ParqueoID"] == null)
            {
                return RedirectToAction("SeleccionarParqueo");
            }

            int parqueoID = (int)Session["ParqueoID"];
            ViewBag.NombreParqueo = Session["NombreParqueo"];

            // Obtener vehículos en el parqueo
            var vehiculos = db.Vehiculoes
                .Where(v => v.ParqueoID == parqueoID)
                .ToList();

            return View(vehiculos);
        }

        /* Parte del controlador */

        // Método para ingresar un vehículo al parqueo
        public ActionResult IngresarVehiculo()
        {
            if (Session["RolID"] == null || (int)Session["RolID"] != 2)
            {
                return RedirectToAction("Login", "Login");
            }

            if (Session["ParqueoID"] == null)
            {
                return RedirectToAction("SeleccionarParqueo");
            }

            ViewBag.NombreParqueo = Session["NombreParqueo"];

            // Inicializar variables en ViewBag para la vista
            ViewBag.ConsultaRealizada = false;

            return View();
        }

        [HttpPost]
        public ActionResult IngresarVehiculo(string numeroPlaca, string tipoVehiculo)
        {
            if (Session["RolID"] == null || (int)Session["RolID"] != 2)
            {
                return RedirectToAction("Login", "Login");
            }

            if (Session["ParqueoID"] == null)
            {
                return RedirectToAction("SeleccionarParqueo");
            }

            ViewBag.NombreParqueo = Session["NombreParqueo"];
            int parqueoID = (int)Session["ParqueoID"];

            // Inicializar variables para la vista
            ViewBag.NumeroPlaca = numeroPlaca;
            ViewBag.ConsultaRealizada = true;
            ViewBag.PuedeIngresar = false;
            ViewBag.MensajeError = "";

            // PASO 1: Verificar si el vehículo ya está en CUALQUIER parqueo
            var vehiculoEnParqueo = db.Vehiculoes
                .FirstOrDefault(v => v.NumeroPlaca == numeroPlaca && v.ParqueoID != null);

            if (vehiculoEnParqueo != null)
            {
                // Buscar el nombre del parqueo donde se encuentra el vehículo
                string nombreParqueo = "otro parqueo";
                var parqueoActual = db.Parqueos.Find(vehiculoEnParqueo.ParqueoID);
                if (parqueoActual != null)
                {
                    nombreParqueo = parqueoActual.Nombre_Parqueo;
                }

                ViewBag.MensajeError = $"Este vehículo ya se encuentra dentro de {nombreParqueo}. No puede ingresar a ningún parqueo hasta que salga.";

                // Registrar intento fallido
                var intentoFallido = new IntentosIngresoFallido
                {
                    VehiculoID = vehiculoEnParqueo.VehiculoID,
                    Motivo = $"Vehículo ya se encuentra en {nombreParqueo}",
                    Fecha = DateTime.Now
                };

                db.IntentosIngresoFallidos.Add(intentoFallido);
                db.SaveChanges();

                return View();
            }

            // PASO 2: Buscar si hay un vehículo FORMALMENTE registrado con UsuarioID
            var vehiculoRegistradoConUsuario = db.Vehiculoes
                .FirstOrDefault(v => v.NumeroPlaca == numeroPlaca && v.UsuarioID != null);

            if (vehiculoRegistradoConUsuario != null)
            {
                // Existe un vehículo formalmente registrado con usuario asignado
                // Este tiene prioridad sobre cualquier otro registro con la misma placa

                // Verificar espacios según el tipo de vehículo seleccionado en el formulario
                var parqueo = db.Parqueos.Find(parqueoID);
                bool hayEspacio = false;
                string tipoEspacio = tipoVehiculo;

                if (tipoVehiculo == "Ley7600")
                {
                    // Verificar espacios para ley 7600
                    int espaciosOcupados = db.Vehiculoes.Count(v => v.ParqueoID == parqueoID && v.UsaEspacioLey7600 == true);
                    hayEspacio = parqueo.Espacios_Ley7600 > espaciosOcupados;
                    tipoEspacio = "Ley 7600";

                    // Actualizar la información del vehículo para reflejar que usa espacio Ley 7600
                    vehiculoRegistradoConUsuario.UsaEspacioLey7600 = true;
                    vehiculoRegistradoConUsuario.Tipo = "Automovil"; // Asumimos que es un automóvil con necesidades especiales
                }
                else if (tipoVehiculo == "Moto")
                {
                    // Verificar espacios para motos
                    int espaciosOcupados = db.Vehiculoes.Count(v => v.ParqueoID == parqueoID && v.Tipo == "Moto");
                    hayEspacio = parqueo.Espacios_Moto > espaciosOcupados;
                    tipoEspacio = "Moto";

                    // Actualizar la información del vehículo
                    vehiculoRegistradoConUsuario.Tipo = "Moto";
                    vehiculoRegistradoConUsuario.UsaEspacioLey7600 = false;
                }
                else // Automovil
                {
                    // Verificar espacios para automóviles
                    int espaciosOcupados = db.Vehiculoes.Count(v => v.ParqueoID == parqueoID && v.Tipo == "Automovil" && (v.UsaEspacioLey7600 == null || v.UsaEspacioLey7600 == false));
                    hayEspacio = parqueo.Espacios_Carros > espaciosOcupados;
                    tipoEspacio = "Automóvil";

                    // Actualizar la información del vehículo
                    vehiculoRegistradoConUsuario.Tipo = "Automovil";
                    vehiculoRegistradoConUsuario.UsaEspacioLey7600 = false;
                }

                // Si no hay espacio disponible
                if (!hayEspacio)
                {
                    ViewBag.MensajeError = $"El parqueo no tiene espacios disponibles para {tipoEspacio}.";

                    // Registrar intento fallido
                    var intentoFallido = new IntentosIngresoFallido
                    {
                        VehiculoID = vehiculoRegistradoConUsuario.VehiculoID,
                        Motivo = $"Parqueo lleno para {tipoEspacio}",
                        Fecha = DateTime.Now
                    };

                    db.IntentosIngresoFallidos.Add(intentoFallido);
                    db.SaveChanges();

                    return View();
                }

                // Todo está bien, el vehículo puede ingresar
                ViewBag.PuedeIngresar = true;

                // Actualizar el registro del vehículo con el ParqueoID
                vehiculoRegistradoConUsuario.ParqueoID = parqueoID;

                // Registrar en bitácora
                var bitacoraEntry = new BitacoraParqueo
                {
                    VehiculoID = vehiculoRegistradoConUsuario.VehiculoID,
                    Fecha = DateTime.Now,
                    TipoMovimiento = "Ingreso",
                    EstadoIngreso = true,
                    NumeroPlaca = vehiculoRegistradoConUsuario.NumeroPlaca,
                    ParqueoID = parqueoID
                };

                db.BitacoraParqueos.Add(bitacoraEntry);
                db.SaveChanges();

                return View();
            }

            // PASO 3: Buscar si el vehículo está FORMALMENTE registrado en la tabla Vehiculos
            // Los vehículos registrados formalmente por un administrador tienen Estado = "Activo"
            var vehiculoRegistrado = db.Vehiculoes
                .FirstOrDefault(v => v.NumeroPlaca == numeroPlaca && v.Estado == "Activo");

            // CASO A: Vehículo registrado formalmente - permitir ingreso sin restricciones
            if (vehiculoRegistrado != null)
            {
                // Verificar espacios según el tipo de vehículo seleccionado en el formulario
                var parqueo = db.Parqueos.Find(parqueoID);
                bool hayEspacio = false;
                string tipoEspacio = tipoVehiculo;

                if (tipoVehiculo == "Ley7600")
                {
                    // Verificar espacios para ley 7600
                    int espaciosOcupados = db.Vehiculoes.Count(v => v.ParqueoID == parqueoID && v.UsaEspacioLey7600 == true);
                    hayEspacio = parqueo.Espacios_Ley7600 > espaciosOcupados;
                    tipoEspacio = "Ley 7600";

                    // Actualizar la información del vehículo
                    vehiculoRegistrado.UsaEspacioLey7600 = true;
                    vehiculoRegistrado.Tipo = "Automovil";
                }
                else if (tipoVehiculo == "Moto")
                {
                    // Verificar espacios para motos
                    int espaciosOcupados = db.Vehiculoes.Count(v => v.ParqueoID == parqueoID && v.Tipo == "Moto");
                    hayEspacio = parqueo.Espacios_Moto > espaciosOcupados;
                    tipoEspacio = "Moto";

                    // Actualizar la información del vehículo
                    vehiculoRegistrado.Tipo = "Moto";
                    vehiculoRegistrado.UsaEspacioLey7600 = false;
                }
                else // Automovil
                {
                    // Verificar espacios para automóviles
                    int espaciosOcupados = db.Vehiculoes.Count(v => v.ParqueoID == parqueoID && v.Tipo == "Automovil" && (v.UsaEspacioLey7600 == null || v.UsaEspacioLey7600 == false));
                    hayEspacio = parqueo.Espacios_Carros > espaciosOcupados;
                    tipoEspacio = "Automóvil";

                    // Actualizar la información del vehículo
                    vehiculoRegistrado.Tipo = "Automovil";
                    vehiculoRegistrado.UsaEspacioLey7600 = false;
                }

                // Si no hay espacio disponible
                if (!hayEspacio)
                {
                    ViewBag.MensajeError = $"El parqueo no tiene espacios disponibles para {tipoEspacio}.";

                    // Registrar intento fallido
                    var intentoFallido = new IntentosIngresoFallido
                    {
                        VehiculoID = vehiculoRegistrado.VehiculoID,
                        Motivo = $"Parqueo lleno para {tipoEspacio}",
                        Fecha = DateTime.Now
                    };

                    db.IntentosIngresoFallidos.Add(intentoFallido);
                    db.SaveChanges();

                    return View();
                }

                // Todo está bien, el vehículo puede ingresar
                ViewBag.PuedeIngresar = true;

                // Actualizar el registro del vehículo con el ParqueoID
                vehiculoRegistrado.ParqueoID = parqueoID;

                // Registrar en bitácora
                var bitacoraEntry = new BitacoraParqueo
                {
                    VehiculoID = vehiculoRegistrado.VehiculoID,
                    Fecha = DateTime.Now,
                    TipoMovimiento = "Ingreso",
                    EstadoIngreso = true,
                    NumeroPlaca = vehiculoRegistrado.NumeroPlaca,
                    ParqueoID = parqueoID
                };

                db.BitacoraParqueos.Add(bitacoraEntry);
                db.SaveChanges();

                return View();
            }

            // PASO 4: Verificar si existe un vehículo con la misma placa que tenga estado diferente a "Activo"
            // (Puede ser un vehículo con "Primer Ingreso" o pendiente de registro formal)
            var vehiculoExistente = db.Vehiculoes
                .Where(v => v.NumeroPlaca == numeroPlaca)
                .OrderByDescending(v => v.VehiculoID)  // Tomar el más reciente
                .FirstOrDefault();

            // PASO 5: Verificar si ha tenido un "Primer Ingreso" previamente
            bool tuvoPrimerIngreso = db.BitacoraParqueos
                .Any(b => b.NumeroPlaca == numeroPlaca && b.TipoMovimiento == "Primer Ingreso");

            // Si tuvo primer ingreso y no está registrado formalmente, denegar acceso
            if (tuvoPrimerIngreso && vehiculoExistente != null && vehiculoExistente.Estado != "Activo")
            {
                ViewBag.MensajeError = "Este vehículo ya utilizó su ingreso de cortesía. Debe registrarse formalmente en el sistema antes de volver a ingresar.";

                // Registrar intento fallido
                var intentoFallido = new IntentosIngresoFallido
                {
                    VehiculoID = vehiculoExistente.VehiculoID,
                    Motivo = "Vehículo requiere registro formal después de ingreso previo",
                    Fecha = DateTime.Now
                };

                db.IntentosIngresoFallidos.Add(intentoFallido);

                // Agregar a la bitácora el rechazo del vehículo por no estar activado en el sistema
                var bitacoraRechazo = new BitacoraParqueo
                {
                    VehiculoID = vehiculoExistente.VehiculoID,
                    Fecha = DateTime.Now,
                    TipoMovimiento = "Ingreso Rechazado",
                    EstadoIngreso = false,
                    NumeroPlaca = numeroPlaca,
                    ParqueoID = parqueoID
                };

                db.BitacoraParqueos.Add(bitacoraRechazo);
                db.SaveChanges();

                return View();
            }

            // CASO C: Primer ingreso para vehículo no registrado
            // Verificar espacios disponibles según el tipo seleccionado
            var parqueoInicial = db.Parqueos.Find(parqueoID);
            bool hayEspacioInicial = false;
            string tipoEspacioInicial = tipoVehiculo;

            if (tipoVehiculo == "Ley7600")
            {
                // Verificar espacios para ley 7600
                int espaciosOcupados = db.Vehiculoes.Count(v => v.ParqueoID == parqueoID && v.UsaEspacioLey7600 == true);
                hayEspacioInicial = parqueoInicial.Espacios_Ley7600 > espaciosOcupados;
                tipoEspacioInicial = "Ley 7600";
            }
            else if (tipoVehiculo == "Moto")
            {
                // Verificar espacios para motos
                int espaciosOcupados = db.Vehiculoes.Count(v => v.ParqueoID == parqueoID && v.Tipo == "Moto");
                hayEspacioInicial = parqueoInicial.Espacios_Moto > espaciosOcupados;
                tipoEspacioInicial = "Moto";
            }
            else // Automovil
            {
                // Verificar espacios para automóviles
                int espaciosOcupados = db.Vehiculoes.Count(v => v.ParqueoID == parqueoID && v.Tipo == "Automovil" && (v.UsaEspacioLey7600 == null || v.UsaEspacioLey7600 == false));
                hayEspacioInicial = parqueoInicial.Espacios_Carros > espaciosOcupados;
                tipoEspacioInicial = "Automóvil";
            }

            if (!hayEspacioInicial)
            {
                ViewBag.MensajeError = $"El parqueo no tiene espacios disponibles para {tipoEspacioInicial}.";

                // Registrar intento fallido
                var intentoFallido = new IntentosIngresoFallido
                {
                    VehiculoID = null,
                    Motivo = $"Parqueo lleno para {tipoEspacioInicial}",
                    Fecha = DateTime.Now
                };

                db.IntentosIngresoFallidos.Add(intentoFallido);
                db.SaveChanges();

                return View();
            }

            // Crear un nuevo vehículo en el sistema (PRIMER INGRESO SIN REGISTRO FORMAL)
            var nuevoVehiculo = new Vehiculo
            {
                NumeroPlaca = numeroPlaca,
                Estado = "Pendiente", // Marcamos como pendiente para distinguir de los registrados formalmente
                ParqueoID = parqueoID,
                Marca = "No especificado",
                Color = "No especificado"
            };

            // Configurar según el tipo de vehículo seleccionado
            if (tipoVehiculo == "Ley7600")
            {
                nuevoVehiculo.Tipo = "Automovil";
                nuevoVehiculo.UsaEspacioLey7600 = true;
            }
            else if (tipoVehiculo == "Moto")
            {
                nuevoVehiculo.Tipo = "Moto";
                nuevoVehiculo.UsaEspacioLey7600 = false;
            }
            else // Automovil
            {
                nuevoVehiculo.Tipo = "Automovil";
                nuevoVehiculo.UsaEspacioLey7600 = false;
            }

            db.Vehiculoes.Add(nuevoVehiculo);
            db.SaveChanges();

            ViewBag.PuedeIngresar = true;
            ViewBag.EsPrimeraVez = true;

            // Registrar en bitácora
            var bitacora = new BitacoraParqueo
            {
                VehiculoID = nuevoVehiculo.VehiculoID,
                Fecha = DateTime.Now,
                TipoMovimiento = "Primer Ingreso",
                EstadoIngreso = true,
                NumeroPlaca = nuevoVehiculo.NumeroPlaca,
                ParqueoID = parqueoID
            };

            db.BitacoraParqueos.Add(bitacora);
            db.SaveChanges();

            return View();
        }

        // Método para registrar la salida de un vehículo del parqueo
        public ActionResult RegistrarSalida(int? id)
        {
            if (Session["RolID"] == null || (int)Session["RolID"] != 2)
            {
                return RedirectToAction("Login", "Login");
            }

            if (Session["ParqueoID"] == null)
            {
                return RedirectToAction("SeleccionarParqueo");
            }

            int parqueoID = (int)Session["ParqueoID"];
            ViewBag.NombreParqueo = Session["NombreParqueo"];

            // Si no se proporciona ID, mostramos el formulario para buscar por placa
            if (id == null)
            {
                return View();
            }

            // Buscar el vehículo
            var vehiculo = db.Vehiculoes.Find(id);
            if (vehiculo == null)
            {
                TempData["Error"] = "El vehículo no existe en el sistema.";
                return RedirectToAction("VehiculosActuales");
            }

            // Verificar que el vehículo esté en el parqueo actual
            if (vehiculo.ParqueoID != parqueoID)
            {
                TempData["Error"] = "El vehículo no se encuentra en este parqueo.";
                return RedirectToAction("VehiculosActuales");
            }

            // Preparar la vista con la información del vehículo
            return View(vehiculo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarSalidaPorPlaca(string numeroPlaca)
        {
            if (Session["RolID"] == null || (int)Session["RolID"] != 2)
            {
                return RedirectToAction("Login", "Login");
            }

            if (Session["ParqueoID"] == null)
            {
                return RedirectToAction("SeleccionarParqueo");
            }

            int parqueoID = (int)Session["ParqueoID"];
            ViewBag.NombreParqueo = Session["NombreParqueo"];
            ViewBag.NumeroPlaca = numeroPlaca;

            if (string.IsNullOrEmpty(numeroPlaca))
            {
                ViewBag.MensajeError = "Debe ingresar un número de placa.";
                return View("RegistrarSalida");
            }

            // Buscar el vehículo por número de placa
            var vehiculo = db.Vehiculoes
                .FirstOrDefault(v => v.NumeroPlaca == numeroPlaca && v.ParqueoID == parqueoID);

            if (vehiculo == null)
            {
                ViewBag.MensajeError = "No se encontró ningún vehículo con esta placa en el parqueo actual.";
                return View("RegistrarSalida");
            }

            // Encontró el vehículo, ahora redirigir al método normal de RegistrarSalida
            return RedirectToAction("RegistrarSalida", new { id = vehiculo.VehiculoID });
        }

        [HttpPost, ActionName("RegistrarSalida")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmarSalida(int id)
        {
            if (Session["RolID"] == null || (int)Session["RolID"] != 2)
            {
                return RedirectToAction("Login", "Login");
            }

            if (Session["ParqueoID"] == null)
            {
                return RedirectToAction("SeleccionarParqueo");
            }

            int parqueoID = (int)Session["ParqueoID"];
            ViewBag.NombreParqueo = Session["NombreParqueo"];

            // Buscar el vehículo
            var vehiculo = db.Vehiculoes.Find(id);
            if (vehiculo == null)
            {
                TempData["Error"] = "El vehículo no existe en el sistema.";
                return RedirectToAction("VehiculosActuales");
            }

            // Verificar que el vehículo esté en el parqueo actual
            if (vehiculo.ParqueoID != parqueoID)
            {
                TempData["Error"] = "El vehículo no se encuentra en este parqueo.";
                return RedirectToAction("VehiculosActuales");
            }

            // Registrar la salida del vehículo en la bitácora
            var bitacora = new BitacoraParqueo
            {
                VehiculoID = vehiculo.VehiculoID,
                Fecha = DateTime.Now,
                TipoMovimiento = "Salida",
                EstadoIngreso = true,
                NumeroPlaca = vehiculo.NumeroPlaca,
                ParqueoID = parqueoID
            };

            db.BitacoraParqueos.Add(bitacora);

            // Actualizar el vehículo para quitar la referencia al parqueo
            vehiculo.ParqueoID = null;

            db.SaveChanges();

            TempData["Mensaje"] = $"Salida del vehículo {vehiculo.NumeroPlaca} registrada correctamente.";
            return RedirectToAction("Index");
        }
    }
}