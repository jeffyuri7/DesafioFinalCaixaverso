using System;
using DesafioFinalCaixaverso.Dominio.Enumeradores;

namespace DesafioFinalCaixaverso.Dominio.Entidades;

public class QuestionarioInvestidor
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public PreferenciaLiquidez PreferenciaLiquidez { get; set; }
    public ObjetivoInvestimento ObjetivoInvestimento { get; set; }
    public NivelConhecimentoInvestidor NivelConhecimento { get; set; }
    public int HorizonteMeses { get; set; }
    public decimal RendaMensal { get; set; }
    public decimal PatrimonioTotal { get; set; }
    public decimal ToleranciaPerdaPercentual { get; set; }
    public bool FonteRendaEstavel { get; set; }
    public DateTime AtualizadoEm { get; set; }

    public Cliente? Cliente { get; set; }
}
