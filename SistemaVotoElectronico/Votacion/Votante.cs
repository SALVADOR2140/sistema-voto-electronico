using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Agregamos las librerías de validación como en tu modelo Candidato
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SistemaVotoElectronico.Modelos.Votacion
{
    public class Votante
    {
        [Key]
        public int Id { get; set; }

        [Required] 
        public string Cedula { get; set; }

        [Required]
        public string Nombres { get; set; }

        public string Correo { get; set; }

       
        public bool HaVotado { get; set; } = false;

        // El Token servirá para validar el ingreso a la mesa de votación
        // Se genera automáticamente al importar el Excel
        public string? Token { get; set; }
    }
}