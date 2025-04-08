using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Asegúrate de incluir este using
using System.Linq;
using System.Web;

namespace ParqueoUlacit.Models.ViewModel
{
    public class UsuarioViewModel
    {
        public int UsuarioID { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio. Por favor, ingrese su nombre.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El número de identificación es obligatorio.")]
        public string Identificacion { get; set; }

        // Aquí la clave puede ser opcional o validarse de otra manera según la lógica de tu negocio.
        public string Clave { get; set; }

        public Nullable<bool> ClaveCambiada { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol para el usuario.")]
        public Nullable<int> RolID { get; set; }

        public virtual Rol Rol { get; set; }
        public virtual ICollection<Vehiculo> Vehiculoes { get; set; }
    }
}
