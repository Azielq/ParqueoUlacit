using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParqueoUlacit.Models.ViewModel
{
    public class BitacoraParqueoViewModel
    {
        public int BitacoraID { get; set; }
        public Nullable<int> VehiculoID { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public string TipoMovimiento { get; set; }
        public Nullable<bool> EstadoIngreso { get; set; }
        public string NumeroPlaca { get; set; }

        public virtual Vehiculo Vehiculo { get; set; }

    }
}