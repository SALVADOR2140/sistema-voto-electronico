using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SistemaVoto.Modelos
{
    public class Candidato
    {
        [Key]
        public int Id { get; set; }

        public string Nombres { get; set; }
        public string Cargo { get; set; }
        public string FotoUrl { get; set; }
        public string PlanGobiernoUrl { get; set; }

        // FK
        public int ListaPoliticaId { get; set; }

        // Navegacion
        public ListaPolitica? ListaPolitica { get; set; }
    }
}