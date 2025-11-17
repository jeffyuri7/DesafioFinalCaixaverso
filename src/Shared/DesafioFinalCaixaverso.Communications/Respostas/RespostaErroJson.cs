using System.Collections.Generic;

namespace DesafioFinalCaixaverso.Communications.Responses;

public class RespostaErroJson
{
    public IList<string> Erros { get; set; }

    public RespostaErroJson(IList<string> erros) => Erros = erros;

    public RespostaErroJson(string erro)
    {
        Erros = new List<string>
        {
            erro
        };
    }
}
