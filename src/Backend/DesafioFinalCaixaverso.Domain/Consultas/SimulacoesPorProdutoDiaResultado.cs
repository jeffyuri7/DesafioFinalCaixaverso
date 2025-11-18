using System;

namespace DesafioFinalCaixaverso.Dominio.Consultas;

public record SimulacoesPorProdutoDiaResultado(string Produto, DateTime Dia, int Quantidade, decimal ValorTotalInvestido);
