namespace IBNB.Models
{
    public class Imagem
    {
        public int Id { get; set; }

        // Caminho relativo do arquivo (ex: /uploads/eventos/arquivo.jpg)
        public string Caminho { get; set; } = string.Empty;

        // Nome original enviado pelo usuário (opcional, útil para exibição)
        public string? NomeOriginal { get; set; }

        public DateTime DataUpload { get; set; } = DateTime.Now;

        // Chave estrangeira para o Evento
        public int EventoId { get; set; }

        // Propriedade de navegação
        public virtual Evento? Evento { get; set; }
    }
}
