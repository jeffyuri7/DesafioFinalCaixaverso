using System;
using System.Linq;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Telemetria;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;
using Shouldly;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Telemetria;

public class CasoDeUsoConsultarTelemetriaServicosTestes
{
    [Fact]
    public async Task Deve_retornar_registros_de_telemetria()
    {
        var repositorio = new TelemetriaServicoRepositorioFalso();
        repositorio.Registros.Add(new TelemetriaServico
        {
            Id = Guid.NewGuid(),
            Servico = "v1/investimentos/simulacoes",
            QuantidadeChamadas = 3,
            UltimaChamada = DateTime.UtcNow
        });

        var casoDeUso = new CasoDeUsoConsultarTelemetriaServicos(repositorio);

        var resposta = await casoDeUso.Executar();

        resposta.ShouldHaveSingleItem();
        resposta.First().QuantidadeChamadas.ShouldBe(3);
    }
}
