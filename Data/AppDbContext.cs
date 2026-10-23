using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IBNB.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IBNB.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Evento> Eventos { get; set; } = null!;
        public DbSet<Aniversariante> Aniversariantes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Filtro Global: toda query feita pelo EF Core vai ignorar registros inativos
            builder.Entity<Evento>().HasQueryFilter(e => e.IsAtivo);
            builder.Entity<Aniversariante>().HasQueryFilter(a => a.IsAtivo);
        }

        // Sobrescreve o SaveChanges para transformar Exclusões em Inativações
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    // Muda o estado de "Deletado" para "Modificado"
                    entry.State = EntityState.Modified;
                    entry.Entity.IsAtivo = false;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    // entry.Entity.DeletedPor = _currentUser; // Pode ser adicionado futuramente
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}