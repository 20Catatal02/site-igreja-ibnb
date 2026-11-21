using System;

namespace IBNB.ViewModels
{
    /// <summary>
    /// Um "tipo" de evento já consolidado para a vitrine (/Eventos).
    /// Eventos recorrentes que se repetem várias vezes aparecem UMA única vez,
    /// com o resumo da sua recorrência.
    /// </summary>
    public class EventoVitrineViewModel
    {
        // Título do evento (o mesmo de todos que compartilham o tipo)
        public string Titulo { get; set; } = string.Empty;

        // Descrição livre (a primeira encontrada entre as ocorrências do tipo)
        public string Descricao { get; set; } = string.Empty;

        // Local
        public string Local { get; set; } = string.Empty;

        // Resumo legível da recorrência. Ex.: "Todo domingo, às 09:00"
        public string ResumoRecorrencia { get; set; } = string.Empty;

        // Próxima data em que acontece (para eventos futuros/recorrentes)
        public DateTime? ProximaData { get; set; }

        // Data e hora da primeira ocorrência cadastrada
        public DateTime DataHora { get; set; }

        // Classificação: "Ceia", "EBD", "CultoNoturno" ou "Evento"
        public string Subtipo { get; set; } = "Evento";

        // Destaque (Ceia do Senhor / Virada de ano) -> usa a tarja amarela
        public bool Destaque { get; set; }

        // Quantidade de ocorrências cadastradas para este tipo (informativo interno)
        public int OcorrenciasCadastradas { get; set; }

        // Rótulo curto do tipo, usado como "tag" na vitrine
        public string Tag => Subtipo switch
        {
            "Ceia" => "Ceia do Senhor",
            "EBD" => "Escola Bíblica",
            "CultoNoturno" => "Culto de Celebração",
            "Virada" => "Virada de Ano",
            _ => "Evento"
        };
    }

    public class EventosIndexViewModel
    {
        public List<EventoVitrineViewModel> Tipos { get; set; } = new();

        // Nome do mês corrente (mantido para o cabeçalho)
        public string NomeMes { get; set; } = string.Empty;

        public DateTime Hoje { get; set; } = DateTime.Now;

        // Usuário autenticado pode criar eventos
        public bool PodeCriarEvento { get; set; }
    }
}
