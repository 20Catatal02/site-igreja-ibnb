namespace IBNB.Models
{
    public class Versiculo
    {
        public int Id { get; set; }

        public string Referencia { get; set; } = string.Empty;

        public string Texto { get; set; } = string.Empty;

        public string Traducao { get; set; } = "Almeida 1911";

        public string? Autor { get; set; }

        public bool Ativo { get; set; } = true;

        public DateOnly? UltimaExibicao { get; set; }
    }
}
