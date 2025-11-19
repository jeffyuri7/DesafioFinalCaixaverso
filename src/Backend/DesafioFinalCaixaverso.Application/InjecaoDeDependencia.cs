using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Perfil;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Telemetria;
using DesafioFinalCaixaverso.Aplicacao.Mapeamentos;
using DesafioFinalCaixaverso.Aplicacao.Servicos.Perfis;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioFinalCaixaverso.Aplicacao;

public static class InjecaoDeDependencia
{
    public static void RegistrarAplicacao(this IServiceCollection services)
    {
        AdicionarCasosDeUso(services);
        MapsterConfiguracoes.Registrar();
    }

    public static void AdicionarCasosDeUso(IServiceCollection services)
    {
        services.AddScoped<ICasoDeUsoSolicitarSimulacao, CasoDeUsoSolicitarSimulacao>();
        services.AddScoped<ICasoDeUsoConsultarHistoricoSimulacoes, CasoDeUsoConsultarHistoricoSimulacoes>();
        services.AddScoped<ICasoDeUsoConsultarSimulacoesPorProdutoDia, CasoDeUsoConsultarSimulacoesPorProdutoDia>();
        services.AddScoped<ICasoDeUsoConsultarInvestimentosCliente, CasoDeUsoConsultarInvestimentosCliente>();
        services.AddScoped<ICasoDeUsoConsultarTelemetriaServicos, CasoDeUsoConsultarTelemetriaServicos>();
        services.AddScoped<IRegistradorTelemetriaServicos, RegistradorTelemetriaServicos>();
        services.AddScoped<ICasoDeUsoConsultarPerfilCliente, CasoDeUsoConsultarPerfilCliente>();
        services.AddScoped<ICasoDeUsoListarProdutosRecomendados, CasoDeUsoListarProdutosRecomendados>();
        services.AddScoped<ICalculadoraPerfilInvestidor, CalculadoraPerfilInvestidor>();
        services.AddScoped<ICasoDeUsoCadastrarCliente, CasoDeUsoCadastrarCliente>();
        services.AddScoped<ICasoDeUsoResponderQuestionarioCliente, CasoDeUsoResponderQuestionarioCliente>();
    }
}
