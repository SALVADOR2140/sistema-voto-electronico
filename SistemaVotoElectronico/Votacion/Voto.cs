using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SistemaVoto.Modelos
{
    public class Voto
    {
        [Key]
        public int Id { get; set; }

        public DateTime Fecha { get; set; }
        public string HashSeguridad { get; set; } // SHA-256

        // FK
        public int EventoElectoralId { get; set; }
        public int? ListaPoliticaId { get; set; } // Puede ser nulo (Voto Blanco)

        // Navegacion
        public EventoElectoral? EventoElectoral { get; set; }
        public ListaPolitica? ListaPolitica { get; set; }
    }
}