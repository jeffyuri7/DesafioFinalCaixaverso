using System;

namespace DesafioFinalCaixaverso.Communications.Responses;

public class PerfilClienteJson
{
    public Guid ClienteId { get; set; }
    public string Perfil { get; set; } = string.Empty;
    public int Pontuacao { get; set; }
    public decimal ValorMedioInvestido { get; set; }
    public decimal ValorTotalInvestido { get; set; }
    public int SimulacoesUltimos30Dias { get; set; }
    public decimal RentabilidadeMediaProduto { get; set; }
    public int LiquidezMediaDias { get; set; }
    public DateTime AtualizadoEm { get; set; }
    public decimal PontuacaoComportamental { get; set; }
    public decimal PontuacaoQuestionario { get; set; }
    public bool PermiteRecomendacao { get; set; }
    public string MetodoCalculo { get; set; } = string.Empty;
    public string Observacoes { get; set; } = string.Empty;
    public bool DadosSuficientes { get; set; }
}
