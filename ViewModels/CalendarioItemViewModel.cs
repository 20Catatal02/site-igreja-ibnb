namespace IBNB.ViewModels
{
    public class CalendarioItemViewModel
    {
        public string Titulo { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string Categoria { get; set; } = string.Empty; // "Evento", "Feriado", "Aniversário"
        public string Local { get; set; } = string.Empty;

        // Departamento responsável pela programação (opcional)
        public string? Departamento { get; set; }

        // Cor do evento: departamentos têm cores próprias; os demais usam o verde.
        public string Cor => string.IsNullOrWhiteSpace(Departamento)
            ? "#198754"
            : Departamento.Trim().ToLowerInvariant() switch
            {
                "homens" => "#1e3a8a",
                "mulheres" => "#ec4899",
                "jovens" => "#8b5cf6",
                _ => "#198754"
            };

        // Classificação do evento: "Ceia", "EBD", "CultoNoturno" ou "Evento"
        public string Subtipo { get; set; } = "Evento";

        // Exibe tarja de destaque (Culto de Ceia do Senhor)
        public bool Destaque { get; set; }

        // true quando a data (somente dia) é igual à data de hoje
        public bool EhHoje { get; set; }

        // Texto exibido na célula, no modal e na agenda.
        // No dia 31/12 (Destaque + Subtipo "Virada") o rótulo é único:
        // "Culto da Virada + Ceia do Senhor" — mesmo horário, uma só celebração.
        public string Rotulo => Subtipo switch
        {
            "Virada" => "Culto da Virada + Ceia do Senhor",
            "Ceia" => "Culto de Ceia do Senhor",
            "Aniversário" => "Aniversário: " + Titulo,
            "Feriado" => Titulo,
            _ => Categoria == "Evento"
                ? Data.ToString("HH:mm") + " " + Titulo
                : Titulo
        };
    }

    // Uma célula (dia) do calendário mensal
    public class CalendarioDiaViewModel
    {
        public DateTime Data { get; set; }
        public bool NoMesAtual { get; set; }
        public bool EhHoje { get; set; }
        public List<CalendarioItemViewModel> Itens { get; set; } = new();

        // Abreviação do dia da semana (Dom, Seg, ...) — usada no resumo do modal
        public string DiaSemana { get; set; } = string.Empty;
    }

    public class CalendarioViewModel
    {
        public DateTime Hoje { get; set; } = DateTime.Now;

        // Mês exibido
        public int Mes { get; set; }
        public int Ano { get; set; }
        public DateTime PrimeiroDia => new DateTime(Ano, Mes, 1);
        public string NomeMes { get; set; } = string.Empty;

        // Dias do mês exibido (somente este mês)
        public List<CalendarioDiaViewModel> Dias { get; set; } = new();

        // Resumo do dia de hoje
        public List<CalendarioItemViewModel> EventosHoje { get; set; } = new();
        public List<CalendarioItemViewModel> FeriadosHoje { get; set; } = new();
        public List<CalendarioItemViewModel> AniversariantesHoje { get; set; } = new();
        public bool TemCultoHoje => EventosHoje.Any();

        // Próximo evento da igreja a partir de hoje
        public CalendarioItemViewModel? ProximoEvento { get; set; }

        // Aniversariantes só são exibidos para usuários autenticados
        public bool MostrarAniversariantes { get; set; }

        // Estatísticas do mês exibido
        public int TotalEventos { get; set; }
        public int TotalFeriados { get; set; }
        public int TotalAniversariantes { get; set; }
        public int TotalCultos { get; set; }
    }
}
