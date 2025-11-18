using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Telemetria;
using Microsoft.AspNetCore.Http;

namespace DesafioFinalCaixaverso.API.Middlewares;

public class RegistroTelemetriaMiddleware
{
    private readonly RequestDelegate _next;

    public RegistroTelemetriaMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IRegistradorTelemetriaServicos registradorTelemetriaServicos)
    {
        await _next(context);

        var endpoint = context.GetEndpoint();
        if (endpoint is null)
            return;

        var servico = NormalizarNomeServico(context, endpoint.DisplayName);
        await registradorTelemetriaServicos.RegistrarAsync(servico, context.RequestAborted);
    }

    private static string NormalizarNomeServico(HttpContext context, string? displayName)
    {
        if (!string.IsNullOrWhiteSpace(displayName))
            return displayName.Trim();

        var path = context.Request.Path.HasValue ? context.Request.Path.Value : "/desconhecido";
        return path!.Trim('/').Replace('/', ':');
    }
}
