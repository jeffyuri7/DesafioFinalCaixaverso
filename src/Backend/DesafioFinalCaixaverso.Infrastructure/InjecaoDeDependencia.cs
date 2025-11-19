using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Dominio.Seguranca;
using DesafioFinalCaixaverso.Infraestrutura.AcessoDados;
using DesafioFinalCaixaverso.Infraestrutura.Extensoes;
using DesafioFinalCaixaverso.Infraestrutura.Repositorios;
using DesafioFinalCaixaverso.Infraestrutura.Seguranca;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace DesafioFinalCaixaverso.Infraestrutura;

public static class InjecaoDeDependencia
{
    public static void RegistrarInfraestrutura(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        AdicionarRepositorios(services);

        if (environment.IsEnvironment("Testing"))
            return;

        AdicionarDbContext(services, configuration);
        AdicionarFluentMigrator(services, configuration);
    }

    private static void AdicionarFluentMigrator(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.ConnectionString();

        services.AddFluentMigratorCore()
            .ConfigureRunner(options => options
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.Load("DesafioFinalCaixaverso.Infraestrutura")).For.All());
    }

    private static void AdicionarDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.ConnectionString();

        services.AddDbContext<CaixaversoDbContext>(dbContextOptions =>
        {
            dbContextOptions.UseSqlServer(connectionString);
        });
    }

    private static void AdicionarRepositorios(IServiceCollection services)
    {
        services.AddScoped<IUnidadeDeTrabalho, UnidadeDeTrabalho>();
        services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
        services.AddScoped<IProdutoRepositorio, ProdutoRepositorio>();
        services.AddScoped<ISimulacaoRepositorio, SimulacaoRepositorio>();
        services.AddScoped<ITelemetriaServicoRepositorio, TelemetriaServicoRepositorio>();
        services.AddScoped<IClientePerfilRepositorio, ClientePerfilRepositorio>();
        services.AddScoped<IQuestionarioInvestidorRepositorio, QuestionarioInvestidorRepositorio>();
        services.AddScoped<IServicoHashSenha, ServicoHashSenha>();
    }
}
