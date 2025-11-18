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

    [Fact]
    public async Task Deve_preservar_data_da_ultima_chamada()
    {
        var ultimaChamada = new DateTime(2025, 11, 17, 8, 30, 0, DateTimeKind.Utc);
        var repositorio = new TelemetriaServicoRepositorioFalso();
        repositorio.Registros.Add(new TelemetriaServico
        {
            Id = Guid.NewGuid(),
            Servico = "v1/telemetria",
            QuantidadeChamadas = 1,
            UltimaChamada = ultimaChamada
        });

        var casoDeUso = new CasoDeUsoConsultarTelemetriaServicos(repositorio);

        var resposta = await casoDeUso.Executar();

        resposta.ShouldHaveSingleItem().UltimaChamada.ShouldBe(ultimaChamada);
    }
}
