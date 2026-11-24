namespace IBNB.ViewModels
{
    public class HomeViewModel
    {
        public VersiculoDoDiaViewModel? Versiculo { get; set; }
    }

    public class VersiculoDoDiaViewModel
    {
        public string Texto { get; set; } = string.Empty;

        public string Referencia { get; set; } = string.Empty;

        public string? Autor { get; set; }

        public string Traducao { get; set; } = string.Empty;
    }
}
