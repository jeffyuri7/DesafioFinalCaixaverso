using System;
using System.Linq;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;
using Shouldly;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Simulacao;

public class CasoDeUsoConsultarHistoricoSimulacoesTestes
{
    [Fact]
    public async Task Deve_mapear_simulacoes_para_historico()
    {
        var simulacao = new ConstrutorSimulacao().Construir();
        var repositorio = new SimulacaoRepositorioFalso().ComHistorico(new[] { simulacao });
        var casoDeUso = new CasoDeUsoConsultarHistoricoSimulacoes(repositorio);

        var historico = await casoDeUso.Executar();

    historico.ShouldHaveSingleItem();
    historico.First().Produto.ShouldBe(simulacao.Produto!.Nome);
    }

    [Fact]
    public async Task Deve_preservar_campos_financeiros_e_datas()
    {
        var dataSimulacao = new DateTime(2025, 11, 1, 12, 0, 0, DateTimeKind.Utc);
        var simulacao = new ConstrutorSimulacao()
            .ComValorInvestido(8_500m)
            .ComDataSimulacao(dataSimulacao)
            .Construir();
        var repositorio = new SimulacaoRepositorioFalso().ComHistorico(new[] { simulacao });
        var casoDeUso = new CasoDeUsoConsultarHistoricoSimulacoes(repositorio);

        var historico = await casoDeUso.Executar();

        var dto = historico.ShouldHaveSingleItem();
        dto.DataSimulacao.ShouldBe(dataSimulacao);
    dto.ValorInvestido.ShouldBe(8_500m);
    dto.ValorFinal.ShouldBe(simulacao.ValorFinal);
    }
}
