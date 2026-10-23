using System;

namespace IBNB.Models
{
    public interface ISoftDeletable
    {
        bool IsAtivo { get; set; }
        DateTime? DeletedAt { get; set; }
        string? DeletedPor { get; set; }
    }
}