using System;

namespace DesafioFinalCaixaverso.Communications.Requests;

public class RequisicaoSimulacaoJson
{
    public Guid ClienteId { get; set; }
    public decimal Valor { get; set; }
    public int PrazoMeses { get; set; }
    public string TipoProduto { get; set; } = string.Empty;
}
