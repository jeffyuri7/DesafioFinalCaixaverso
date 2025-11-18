using System;
using System.Collections.Generic;
using System.Linq;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;
using DesafioFinalCaixaverso.Dominio.Consultas;
using DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;
using Shouldly;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Simulacao;

public class CasoDeUsoConsultarSimulacoesPorProdutoDiaTestes
{
    [Fact]
    public async Task Deve_retornar_agrupamentos_de_simulacoes()
    {
        var agrupado = new List<SimulacoesPorProdutoDiaResultado>
        {
            new("CDB", DateTime.UtcNow.Date, 2, 5_000m)
        };

        var repositorio = new SimulacaoRepositorioFalso().ComAgrupado(agrupado);
        var casoDeUso = new CasoDeUsoConsultarSimulacoesPorProdutoDia(repositorio);

        var resposta = await casoDeUso.Executar();

        resposta.ShouldHaveSingleItem();
        resposta.First().Produto.ShouldBe("CDB");
    }

    [Fact]
    public async Task Deve_preservar_quantidade_total_e_valor_por_produto()
    {
        var diaAtual = DateTime.UtcNow.Date;
        var agrupado = new List<SimulacoesPorProdutoDiaResultado>
        {
            new("LCI", diaAtual, 3, 12_000m),
            new("CDB", diaAtual.AddDays(-1), 1, 2_500m)
        };

        var repositorio = new SimulacaoRepositorioFalso().ComAgrupado(agrupado);
        var casoDeUso = new CasoDeUsoConsultarSimulacoesPorProdutoDia(repositorio);

        var resposta = await casoDeUso.Executar();

        resposta.Count.ShouldBe(2);
        var lci = resposta.First(r => r.Produto == "LCI");
        lci.Quantidade.ShouldBe(3);
        lci.ValorTotalInvestido.ShouldBe(12_000m);

        var cdb = resposta.First(r => r.Produto == "CDB");
        cdb.Quantidade.ShouldBe(1);
        cdb.ValorTotalInvestido.ShouldBe(2_500m);
    }
}
