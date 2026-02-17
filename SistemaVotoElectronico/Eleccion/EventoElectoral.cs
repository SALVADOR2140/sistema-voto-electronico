using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVoto.Modelos
{
    public class EventoElectoral
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }

        [NotMapped]
        public List<Candidato> Candidatos { get; set; } = new List<Candidato>();
    }
}