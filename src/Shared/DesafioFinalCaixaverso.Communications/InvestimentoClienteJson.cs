using System;

namespace DesafioFinalCaixaverso.Communications.Responses;

public class InvestimentoClienteJson
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal Rentabilidade { get; set; }
    public DateTime Data { get; set; }
}
