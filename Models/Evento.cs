using System; // Necessário para DateTime e DayOfWeek

namespace IBNB.Models
{
    public class Evento : ISoftDeletable
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
        public string Local { get; set; } = string.Empty;

        // Recorrência Flexível
        public bool IsRecorrente { get; set; }
        
        // Padrões: "Nunca", "Diario", "SegundaSexta", "Semanal", "Mensal", "Anual", "Personalizado"
        public string? TipoRecorrencia { get; set; } = "Nunca";

        // Usado quando TipoRecorrencia == "Personalizado"
        public string? FrequenciaPersonalizada { get; set; } // "dia", "semana", "mes", "ano"
        public int IntervaloRecorrencia { get; set; } = 1;  // "A cada X" (1, 2, 3...)

        // Para casos específicos (ex: "2.º domingo")
        public int? IndiceSemana { get; set; } 
        public DayOfWeek? DiaDaSemana { get; set; } 

        // --- Propriedades do Soft Delete ---
        public bool IsAtivo { get; set; } = true;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedPor { get; set; }
    }
}