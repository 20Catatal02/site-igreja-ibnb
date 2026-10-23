using System;

namespace IBNB.Models
{
    // Adicionada a interface ISoftDeletable aqui
    public class Aniversariante : ISoftDeletable 
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }

        // --- Propriedades do Soft Delete ---
        public bool IsAtivo { get; set; } = true;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedPor { get; set; }
    }
}