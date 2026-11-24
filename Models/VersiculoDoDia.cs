namespace IBNB.Models
{
    public class VersiculoDoDia
    {
        public DateOnly Data { get; set; }

        public int VersiculoId { get; set; }

        public Versiculo? Versiculo { get; set; }
    }
}
