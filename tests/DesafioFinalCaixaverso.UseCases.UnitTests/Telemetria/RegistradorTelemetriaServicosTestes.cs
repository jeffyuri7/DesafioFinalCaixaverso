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

        await registrador.RegistrarAsync("v1/investimentos/simulacoes");

        repositorio.UltimoServicoRegistrado.ShouldBe("v1/investimentos/simulacoes");
    }
}
