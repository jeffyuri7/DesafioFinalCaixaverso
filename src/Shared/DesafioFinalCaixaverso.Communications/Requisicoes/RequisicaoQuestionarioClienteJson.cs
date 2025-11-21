using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DesafioFinalCaixaverso.Dominio.Enumeradores;

namespace DesafioFinalCaixaverso.Communications.Requests;

public class RequisicaoQuestionarioClienteJson
{
    public PreferenciaLiquidez PreferenciaLiquidez { get; set; }

    public ObjetivoInvestimento ObjetivoInvestimento { get; set; }

    public NivelConhecimentoInvestidor NivelConhecimento { get; set; }

    [Range(1, int.MaxValue)]
    [DefaultValue(12)]
    public int HorizonteMeses { get; set; }

    [Range(0, double.MaxValue)]
    public decimal RendaMensal { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PatrimonioTotal { get; set; }

    [Range(0, 100)]
    [DefaultValue(25)]
    public decimal ToleranciaPerdaPercentual { get; set; }

    public bool FonteRendaEstavel { get; set; }
}
