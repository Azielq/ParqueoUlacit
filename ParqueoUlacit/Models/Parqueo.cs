//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ParqueoUlacit.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Parqueo
    {
        public Parqueo()
        {
            this.BitacoraParqueos = new HashSet<BitacoraParqueo>();
            this.Vehiculoes = new HashSet<Vehiculo>();
        }
    
        public int ParqueoID { get; set; }
        public string Nombre_Parqueo { get; set; }
        public string Ubicacion { get; set; }
        public Nullable<int> Espacios_Carros { get; set; }
        public Nullable<int> Espacios_Moto { get; set; }
        public Nullable<int> Espacios_Ley7600 { get; set; }
    
        public virtual ICollection<BitacoraParqueo> BitacoraParqueos { get; set; }
        public virtual ICollection<Vehiculo> Vehiculoes { get; set; }
    }
}
