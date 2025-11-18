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
        historico.First().ProdutoNome.ShouldBe(simulacao.Produto!.Nome);
    }
}
