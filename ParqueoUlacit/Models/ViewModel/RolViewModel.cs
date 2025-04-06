using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParqueoUlacit.Models.ViewModel
{
    public class RolViewModel
    {

        public int RolID { get; set; }
        public string NombreRol { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; }

    }
}