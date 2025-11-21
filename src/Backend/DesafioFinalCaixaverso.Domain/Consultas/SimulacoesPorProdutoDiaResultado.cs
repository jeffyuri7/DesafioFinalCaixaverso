using System;

namespace DesafioFinalCaixaverso.Dominio.Consultas;

public record SimulacoesPorProdutoDiaResultado(string Produto, DateTime Data, int QuantidadeSimulacoes, decimal MediaValorFinal);
