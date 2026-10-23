namespace IBNB.ViewModels
{
    public class CalendarioItemViewModel
    {
        public string Titulo { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string Local { get; set; } = string.Empty;
    }
}