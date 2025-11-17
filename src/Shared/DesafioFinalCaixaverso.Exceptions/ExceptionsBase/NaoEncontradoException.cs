using System.Collections.Generic;
using System.Net;

namespace DesafioFinalCaixaverso.Exceptions.ExceptionsBase;

public class NaoEncontradoException : DesafioFinalCaixaversoException
{
    private readonly string _mensagem;

    public NaoEncontradoException(string mensagem) : base(mensagem) =>_mensagem = mensagem;

    public override IList<string> ObterMensagens() => new List<string> { _mensagem };

    public override HttpStatusCode StatusCode() => HttpStatusCode.NotFound;
}
