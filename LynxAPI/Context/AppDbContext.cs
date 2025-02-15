using LynxAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LynxAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Equipamento> Equipamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Definindo a chave primária composta (Instalação + Lote)
            modelBuilder.Entity<Equipamento>()
                .HasKey(e => new { e.Instalacao, e.Lote });

            // Configuração do enum Operadora para armazenar como string no banco (VARCHAR(5))
            modelBuilder.Entity<Equipamento>()
                .Property(e => e.Operadora)
                .HasConversion(
                    v => v.ToString(),
                    v => (OperadoraEnum)Enum.Parse(typeof(OperadoraEnum), v)
                );
        }
    }
}
