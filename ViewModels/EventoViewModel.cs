using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace IBNB.ViewModels
{
    public class EventoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o título do evento.")]
        [Display(Name = "Título")]
        [StringLength(150)]
        public string Titulo { get; set; } = string.Empty;

        [Display(Name = "Descrição")]
        [StringLength(2000)]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a data e hora do evento.")]
        [Display(Name = "Data e Hora")]
        public DateTime DataHora { get; set; } = DateTime.Now;

        [Display(Name = "Local")]
        [StringLength(200)]
        public string Local { get; set; } = string.Empty;

        [Display(Name = "Este evento é recorrente?")]
        public bool IsRecorrente { get; set; }

        [Display(Name = "Tipo de Recorrência")]
        public string? TipoRecorrencia { get; set; } = "Nunca";

        [Display(Name = "Frequência")]
        public string? FrequenciaPersonalizada { get; set; }

        [Display(Name = "Intervalo")]
        [Range(1, 365, ErrorMessage = "O intervalo deve ser maior que zero.")]
        public int IntervaloRecorrencia { get; set; } = 1;

        [Display(Name = "Índice da Semana")]
        public int? IndiceSemana { get; set; }

        [Display(Name = "Dia da Semana")]
        public DayOfWeek? DiaDaSemana { get; set; }

        // Upload da imagem do evento
        [Display(Name = "Imagem do Evento")]
        public IFormFile? ImagemUpload { get; set; }
    }
}
