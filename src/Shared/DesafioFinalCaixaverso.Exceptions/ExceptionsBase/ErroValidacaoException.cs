using System.Collections.Generic;
using System.Net;

namespace DesafioFinalCaixaverso.Exceptions.ExceptionsBase;

public class ErroValidacaoException : DesafioFinalCaixaversoException
{
    private readonly IList<string> _erros;

    public ErroValidacaoException(IList<string> erros) : base(string.Empty) => _erros = erros;

    public override IList<string> ObterMensagens() => _erros;

    public override HttpStatusCode StatusCode() => HttpStatusCode.BadRequest;
}
