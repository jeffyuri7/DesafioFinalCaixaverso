using System;

namespace DesafioFinalCaixaverso.Communications.Responses;

public class ClienteAutenticadoJson
{
    public Guid ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TokenAcessoJson Token { get; set; } = new();
}
