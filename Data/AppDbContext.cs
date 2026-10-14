using Microsoft.EntityFrameworkCore;
using IBNB.Models;

namespace IBNB.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opttions) : base(opttions)
        {
            
        }
        //Tabela Eventos
        public DbSet<Evento> Eventos { get; set; }
    }
}