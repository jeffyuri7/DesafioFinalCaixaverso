using DesafioFinalCaixaverso.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DesafioFinalCaixaverso.Infraestrutura.AcessoDados;

public class CaixaversoDbContext : DbContext
{
    public CaixaversoDbContext(DbContextOptions<CaixaversoDbContext> options) : base(options) { }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Simulacao> Simulacoes { get; set; }
    public DbSet<TelemetriaServico> TelemetriaServicos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CaixaversoDbContext).Assembly);

        modelBuilder.Entity<Produto>(builder =>
        {
            builder.Property(produto => produto.Rentabilidade).HasPrecision(18, 4);
            builder.Property(produto => produto.MinimoInvestimento).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Simulacao>(builder =>
        {
            builder.Property(simulacao => simulacao.ValorInvestido).HasPrecision(18, 2);
            builder.Property(simulacao => simulacao.ValorFinal).HasPrecision(18, 2);
            builder.Property(simulacao => simulacao.RentabilidadeEfetiva).HasPrecision(18, 4);
        });
    }
}
