using System;

namespace DesafioFinalCaixaverso.Communications.Responses;

public class TokenAcessoJson
{
    public string Valor { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
}
