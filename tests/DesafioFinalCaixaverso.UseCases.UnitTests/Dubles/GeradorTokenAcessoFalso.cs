using System;
using DesafioFinalCaixaverso.Dominio.Seguranca;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;

public class GeradorTokenAcessoFalso : IGeradorTokenAcesso
{
    public TokenAcesso TokenRetornado { get; set; } = new("token-de-testes", DateTime.UtcNow.AddMinutes(15));

    public Guid? ClienteSolicitado { get; private set; }

    public TokenAcesso Gerar(Guid clienteId)
    {
        ClienteSolicitado = clienteId;
        return TokenRetornado;
    }
}
