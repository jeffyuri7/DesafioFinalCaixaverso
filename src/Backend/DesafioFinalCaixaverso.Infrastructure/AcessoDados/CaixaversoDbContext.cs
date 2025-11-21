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
    public DbSet<ClientePerfil> ClientePerfis { get; set; }
    public DbSet<QuestionarioInvestidor> QuestionariosInvestidor { get; set; }
    public DbSet<ClientePerfilDinamico> ClientePerfisDinamicos { get; set; }

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

        modelBuilder.Entity<TelemetriaServico>(builder =>
        {
            builder.Property(telemetria => telemetria.Servico).HasMaxLength(300);
        });

        modelBuilder.Entity<ClientePerfil>(builder =>
        {
            builder.HasIndex(perfil => perfil.ClienteId).IsUnique();
            builder.Property(perfil => perfil.ValorMedioInvestido).HasPrecision(18, 2);
            builder.Property(perfil => perfil.ValorTotalInvestido).HasPrecision(18, 2);
            builder.Property(perfil => perfil.RentabilidadeMediaProduto).HasPrecision(18, 4);
            builder.Property(perfil => perfil.PontuacaoComportamental).HasPrecision(5, 2);
            builder.Property(perfil => perfil.PontuacaoQuestionario).HasPrecision(5, 2);
            builder.Property(perfil => perfil.MetodoCalculo).HasMaxLength(100);
            builder.Property(perfil => perfil.Observacoes).HasMaxLength(500);
            builder.Property(perfil => perfil.DadosSuficientes).HasDefaultValue(false);
        });

        modelBuilder.Entity<QuestionarioInvestidor>(builder =>
        {
            builder.HasIndex(questionario => questionario.ClienteId).IsUnique();
            builder.Property(questionario => questionario.RendaMensal).HasPrecision(18, 2);
            builder.Property(questionario => questionario.PatrimonioTotal).HasPrecision(18, 2);
            builder.Property(questionario => questionario.ToleranciaPerdaPercentual).HasPrecision(5, 2);
        });

        modelBuilder.Entity<ClientePerfilDinamico>(builder =>
        {
            builder.HasIndex(perfil => perfil.ClienteId).IsUnique();
            builder.Property(perfil => perfil.Pontuacao).HasPrecision(5, 2);
            builder.Property(perfil => perfil.VolumeTotalInvestido).HasPrecision(18, 2);
            builder.HasOne(perfil => perfil.Cliente)
                .WithMany()
                .HasForeignKey(perfil => perfil.ClienteId)
                .OnDelete(DeleteBehavior.ClientCascade);
        });
    }
}
