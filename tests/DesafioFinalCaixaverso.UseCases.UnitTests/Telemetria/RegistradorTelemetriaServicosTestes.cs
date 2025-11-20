using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Telemetria;
using DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;
using Shouldly;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Telemetria;

public class RegistradorTelemetriaServicosTestes
{
    [Fact]
    public async Task Deve_registrar_chamada_no_repositorio()
    {
        var repositorio = new TelemetriaServicoRepositorioFalso();
        var registrador = new RegistradorTelemetriaServicos(repositorio);

        await registrador.RegistrarAsync("v1/investimentos/simulacoes", 250);

        repositorio.UltimoServicoRegistrado.ShouldBe("v1/investimentos/simulacoes");
    }

    [Fact]
    public async Task Deve_acumular_quantidade_para_servicos_repetidos()
    {
        var repositorio = new TelemetriaServicoRepositorioFalso();
        var registrador = new RegistradorTelemetriaServicos(repositorio);

        await registrador.RegistrarAsync("v1/investimentos/simulacoes", 200);
        await registrador.RegistrarAsync("v1/investimentos/simulacoes", 400);

        var registro = repositorio.Registros.ShouldHaveSingleItem();
        registro.QuantidadeChamadas.ShouldBe(2);
        registro.TempoTotalRespostaMs.ShouldBe(600);
    }
}
