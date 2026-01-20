using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.ComponentModel.DataAnnotations;

namespace SistemaVoto.Modelos
{
    public class ListaPolitica
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }
        public string Eslogan { get; set; }
        public string LogoUrl { get; set; }

        // FK
        public int EventoElectoralId { get; set; }

        // Navegacion
        public EventoElectoral? EventoElectoral { get; set; }
    }
}