using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DesafioFinalCaixaverso.API.Filters;

public class FiltroDeExcecao : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is DesafioFinalCaixaversoException excecaoDominio)
        {
            TratarExcecaoConhecida(excecaoDominio, context);
            return;
        }

        TratarExcecaoDesconhecida(context);
    }

    private static void TratarExcecaoConhecida(DesafioFinalCaixaversoException excecao, ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = (int)excecao.StatusCode();
        context.Result = new ObjectResult(new RespostaErroJson(excecao.ObterMensagens()));
        context.ExceptionHandled = true;
    }

    private static void TratarExcecaoDesconhecida(ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
    context.Result = new ObjectResult(new RespostaErroJson(MensagensDeExcecao.ERRO_DESCONHECIDO));
        context.ExceptionHandled = true;
    }
}
