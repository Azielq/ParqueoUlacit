using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParqueoUlacit.Models.TableViewModel
{
    public class IntentosIngresosFallidoTableViewModel
    {

        public int IntentoID { get; set; }
        public Nullable<int> VehiculoID { get; set; }
        public string Motivo { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }

        public virtual Vehiculo Vehiculo { get; set; }

    }
}