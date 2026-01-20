using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.ComponentModel.DataAnnotations;

namespace SistemaVoto.Modelos
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        public string Cedula { get; set; }
        public string Nombres { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public bool YaVoto { get; set; } // Estado

        // FK
        public int RolUsuarioId { get; set; }

        // Navegacion
        public RolUsuario? RolUsuario { get; set; }
    }
}