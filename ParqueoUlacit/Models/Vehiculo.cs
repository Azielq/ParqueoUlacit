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
    
    public partial class Vehiculo
    {
        public Vehiculo()
        {
            this.BitacoraParqueos = new HashSet<BitacoraParqueo>();
            this.IntentosIngresoFallidos = new HashSet<IntentosIngresoFallido>();
        }
    
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
