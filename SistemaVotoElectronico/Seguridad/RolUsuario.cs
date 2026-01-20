using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace SistemaVoto.Modelos
{
    public class RolUsuario
    {
        [Key]
        public int Id { get; set; }
        public string NombreRol { get; set; } // "Administrador", "Votante"
    }
}