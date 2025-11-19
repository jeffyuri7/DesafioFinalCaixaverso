using System;
using DesafioFinalCaixaverso.Dominio.Enumeradores;

namespace DesafioFinalCaixaverso.Aplicacao.Servicos.Perfis;

public class PerfilInvestidorResultado
{
    public Guid ClienteId { get; init; }
    public PerfilInvestidor Perfil { get; init; }
    public int Pontuacao { get; init; }
    public decimal ValorMedioInvestido { get; init; }
    public decimal ValorTotalInvestido { get; init; }
    public int SimulacoesUltimos30Dias { get; init; }
    public decimal RentabilidadeMediaProduto { get; init; }
    public int LiquidezMediaDias { get; init; }
    public DateTime AtualizadoEm { get; init; }
    public decimal PontuacaoComportamental { get; init; }
    public decimal PontuacaoQuestionario { get; init; }
    public bool PermiteRecomendacao { get; init; }
    public string MetodoCalculo { get; init; } = string.Empty;
    public string Observacoes { get; init; } = string.Empty;
    public bool DadosSuficientes { get; init; }
}
