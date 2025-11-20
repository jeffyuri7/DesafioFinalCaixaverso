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
    public async Task Deve_retornar_resumo_do_mes_atual()
    {
        var repositorio = new TelemetriaServicoRepositorioFalso();
        var agora = DateTime.UtcNow;
        repositorio.Registros.Add(new TelemetriaServico
        {
            Id = Guid.NewGuid(),
            Servico = "v1/investimentos/simulacoes",
            AnoReferencia = agora.Year,
            MesReferencia = agora.Month,
            QuantidadeChamadas = 3,
            TempoTotalRespostaMs = 900,
            UltimaChamada = agora
        });

        var casoDeUso = new CasoDeUsoConsultarTelemetriaServicos(repositorio);

        var resposta = await casoDeUso.Executar();

        resposta.Servicos.ShouldHaveSingleItem();
        resposta.Servicos.First().QuantidadeChamadas.ShouldBe(3);
        resposta.Servicos.First().MediaTempoRespostaMs.ShouldBe(300);
        resposta.Periodo.Inicio.Day.ShouldBe(1);
        resposta.Periodo.Inicio.Month.ShouldBe(agora.Month);
        resposta.Periodo.Fim.Month.ShouldBe(agora.Month);
    }

    [Fact]
    public async Task Deve_retornar_lista_vazia_quando_nao_existir_registros_no_periodo()
    {
        var repositorio = new TelemetriaServicoRepositorioFalso();
        var casoDeUso = new CasoDeUsoConsultarTelemetriaServicos(repositorio);

        var resposta = await casoDeUso.Executar();

        resposta.Servicos.ShouldBeEmpty();
    }
}
