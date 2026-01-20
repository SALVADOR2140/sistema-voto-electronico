using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SistemaVoto.Modelos
{
    public class ResultadoEleccion
    {
        [Key]
        public int Id { get; set; }

        public int TotalVotos { get; set; } // Cantidad de votos obtenidos

        // Opcional: Podrías guardar el % aquí si quieres evitar calcularlo en el front
        // public decimal Porcentaje { get; set; } 

        // FK
        public int EventoElectoralId { get; set; }
        public int ListaPoliticaId { get; set; }

        // Navegacion
        public EventoElectoral? EventoElectoral { get; set; }
        public ListaPolitica? ListaPolitica { get; set; }
    }
}