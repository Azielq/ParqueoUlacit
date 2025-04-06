using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParqueoUlacit.Models.ViewModel
{
    public class UsuarioViewModel
    {

        public int UsuarioID { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public System.DateTime FechaNacimiento { get; set; }
        public string Identificacion { get; set; }
        public string Clave { get; set; }
        public Nullable<bool> ClaveCambiada { get; set; }
        public Nullable<int> RolID { get; set; }

        public virtual Rol Rol { get; set; }
        public virtual ICollection<Vehiculo> Vehiculoes { get; set; }


    }
}