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
}
