using DesafioFinalCaixaverso.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DesafioFinalCaixaverso.Infraestrutura.AcessoDados;

public class CaixaversoDbContext : DbContext
{
    public CaixaversoDbContext(DbContextOptions<CaixaversoDbContext> options) : base(options) { }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Simulacao> Simulacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CaixaversoDbContext).Assembly);
    }
}
