 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParqueoUlacit.Models.ViewModel
{
	public class VehiculoViewModel
	{
        public int VehiculoID { get; set; }
        public string Marca { get; set; }
        public string Color { get; set; }
        public string NumeroPlaca { get; set; }
        public string Tipo { get; set; }
        public Nullable<int> UsuarioID { get; set; }
        public Nullable<bool> UsaEspacioLey7600 { get; set; }
        public string Estado { get; set; }
        public Nullable<int> ParqueoID { get; set; }

        public virtual ICollection<BitacoraParqueo> BitacoraParqueos { get; set; }
        public virtual ICollection<IntentosIngresoFallido> IntentosIngresoFallidos { get; set; }
        public virtual Parqueo Parqueo { get; set; }
        public virtual Usuario Usuario { get; set; }


    }
}