using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParqueoUlacit.Models.ViewModel
{
	public class VerificacionAccesoViewModel
	{

            public string NumeroPlaca { get; set; }
            public bool PuedeIngresar { get; set; }
            public string MensajeError { get; set; }
            public Vehiculo Vehiculo { get; set; }
        }
    }


