using System.Collections.Generic;
using System.Net;

namespace DesafioFinalCaixaverso.Exceptions.ExceptionsBase;

public abstract class DesafioFinalCaixaversoException : SystemException
{
    protected DesafioFinalCaixaversoException(string message) : base(message)
    {
    }

    public abstract IList<string> ObterMensagens();
    public abstract HttpStatusCode StatusCode();
}
