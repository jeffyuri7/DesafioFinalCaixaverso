using System;

namespace DesafioFinalCaixaverso.Communications.Responses;

public class ClienteCadastradoJson
{
    public Guid ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
