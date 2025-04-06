using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParqueoUlacit.Models.ViewModel
{
    public class ParqueoViewModel
    {

        public int ParqueoID { get; set; }
        public string Nombre_Parqueo { get; set; }
        public string Ubicacion { get; set; }
        public Nullable<int> Espacios_Carros { get; set; }
        public Nullable<int> Espacios_Moto { get; set; }
        public Nullable<int> Espacios_Ley7600 { get; set; }

        public virtual ICollection<Vehiculo> Vehiculoes { get; set; }

    }
}