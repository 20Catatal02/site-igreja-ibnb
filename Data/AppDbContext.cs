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
        public DbSet<Versiculo> Versiculos { get; set; } = null!;
        public DbSet<VersiculoDoDia> VersiculosDoDia { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Filtro Global: toda query feita pelo EF Core vai ignorar registros inativos
            builder.Entity<Evento>().HasQueryFilter(e => e.IsAtivo);
            builder.Entity<Evento>().Property(e => e.Departamento).HasMaxLength(60);
            builder.Entity<Aniversariante>().HasQueryFilter(a => a.IsAtivo);

            builder.Entity<Versiculo>(entity =>
            {
                entity.Property(v => v.Referencia).HasMaxLength(120).IsRequired();
                entity.Property(v => v.Texto).IsRequired();
                entity.Property(v => v.Traducao).HasMaxLength(40).IsRequired();
                entity.Property(v => v.Autor).HasMaxLength(80);
                entity.HasIndex(v => new { v.Referencia, v.Traducao }).IsUnique();
            });

            builder.Entity<VersiculoDoDia>(entity =>
            {
                entity.HasKey(v => v.Data);
                entity.HasOne(v => v.Versiculo)
                    .WithMany()
                    .HasForeignKey(v => v.VersiculoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
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
