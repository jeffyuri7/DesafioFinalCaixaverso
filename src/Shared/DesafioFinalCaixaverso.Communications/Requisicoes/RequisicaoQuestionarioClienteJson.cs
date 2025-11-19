using DesafioFinalCaixaverso.Dominio.Enumeradores;

namespace DesafioFinalCaixaverso.Communications.Requests;

public class RequisicaoQuestionarioClienteJson
{
    public PreferenciaLiquidez PreferenciaLiquidez { get; set; }
    public ObjetivoInvestimento ObjetivoInvestimento { get; set; }
    public NivelConhecimentoInvestidor NivelConhecimento { get; set; }
    public int HorizonteMeses { get; set; }
    public decimal RendaMensal { get; set; }
    public decimal PatrimonioTotal { get; set; }
    public decimal ToleranciaPerdaPercentual { get; set; }
    public bool FonteRendaEstavel { get; set; }
}
