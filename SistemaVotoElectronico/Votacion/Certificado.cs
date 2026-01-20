using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaVoto.Modelos
{
    public class Certificado
    {
        [Key]
        public int Id { get; set; }

        public string CodigoQR { get; set; }
        public DateTime FechaEmision { get; set; }

        // FK
        public int UsuarioId { get; set; }
        public int EventoElectoralId { get; set; }

        // Navegacion
        public Usuario? Usuario { get; set; }
        public EventoElectoral? EventoElectoral { get; set; }
    }
}