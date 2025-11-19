using System.ComponentModel.DataAnnotations;
using DesafioFinalCaixaverso.Dominio.Enumeradores;
using Swashbuckle.AspNetCore.Annotations;

namespace DesafioFinalCaixaverso.Communications.Requests;

[SwaggerSchema(Description = "Questionário de suitability obrigatório para liberar recomendações.")]
public class RequisicaoQuestionarioClienteJson
{
    [SwaggerSchema(Description = "Preferência de liquidez: 0=Alta, 1=Média, 2=Baixa.")]
    public PreferenciaLiquidez PreferenciaLiquidez { get; set; }

    [SwaggerSchema(Description = "Objetivo do investimento: 0=Preservação, 1=Renda, 2=Equilíbrio, 3=Crescimento.")]
    public ObjetivoInvestimento ObjetivoInvestimento { get; set; }

    [SwaggerSchema(Description = "Nível de conhecimento do investidor: 0=Iniciante, 1=Intermediário, 2=Avançado.")]
    public NivelConhecimentoInvestidor NivelConhecimento { get; set; }

    [SwaggerSchema(Description = "Horizonte de investimento em meses.")]
    [Range(1, int.MaxValue)]
    public int HorizonteMeses { get; set; }

    [SwaggerSchema(Description = "Renda mensal declarada em reais.", Format = "decimal")]
    [Range(0, double.MaxValue)]
    public decimal RendaMensal { get; set; }

    [SwaggerSchema(Description = "Patrimônio total estimado em reais.", Format = "decimal")]
    [Range(0, double.MaxValue)]
    public decimal PatrimonioTotal { get; set; }

    [SwaggerSchema(Description = "Tolerância máxima a perda (%).")]
    [Range(0, 100)]
    public decimal ToleranciaPerdaPercentual { get; set; }

    [SwaggerSchema(Description = "Indica se a principal fonte de renda do cliente é estável.")]
    public bool FonteRendaEstavel { get; set; }
}
