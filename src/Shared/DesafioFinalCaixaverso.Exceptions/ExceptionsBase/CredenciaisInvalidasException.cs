using System.Collections.Generic;
using System.Net;

namespace DesafioFinalCaixaverso.Exceptions.ExceptionsBase;

public class CredenciaisInvalidasException : DesafioFinalCaixaversoException
{
    public CredenciaisInvalidasException(string mensagem) : base(mensagem)
    {
    }

    public override IList<string> ObterMensagens() => new List<string> { Message };

    public override HttpStatusCode StatusCode() => HttpStatusCode.Unauthorized;
}
