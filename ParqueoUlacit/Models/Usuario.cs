//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ParqueoUlacit.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Usuario
    {
        public Usuario()
        {
            this.Vehiculo = new HashSet<Vehiculo>();
        }
    
        public int UsuarioID { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public System.DateTime FechaNacimiento { get; set; }
        public string Identificacion { get; set; }
        public string Clave { get; set; }
        public Nullable<bool> ClaveCambiada { get; set; }
        public Nullable<int> RolID { get; set; }
    
        public virtual Rol Rol { get; set; }
        public virtual ICollection<Vehiculo> Vehiculo { get; set; }
    }
}
