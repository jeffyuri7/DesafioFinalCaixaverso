using System;
using DesafioFinalCaixaverso.Dominio.Enumeradores;

namespace DesafioFinalCaixaverso.Dominio.Entidades;

public class ClientePerfil
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public PerfilInvestidor Perfil { get; set; }
    public int Pontuacao { get; set; }
    public decimal ValorMedioInvestido { get; set; }
    public decimal ValorTotalInvestido { get; set; }
    public int SimulacoesUltimos30Dias { get; set; }
    public decimal RentabilidadeMediaProduto { get; set; }
    public int LiquidezMediaDias { get; set; }
    public decimal PontuacaoComportamental { get; set; }
    public decimal PontuacaoQuestionario { get; set; }
    public bool PermiteRecomendacao { get; set; }
    public string MetodoCalculo { get; set; } = string.Empty;
    public string Observacoes { get; set; } = string.Empty;
    public DateTime AtualizadoEm { get; set; }

    public Cliente? Cliente { get; set; }
}
