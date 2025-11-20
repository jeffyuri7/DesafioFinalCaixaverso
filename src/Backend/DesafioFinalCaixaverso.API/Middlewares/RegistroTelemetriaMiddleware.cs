using System;
using System.Diagnostics;
using System.Linq;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Telemetria;
using Microsoft.AspNetCore.Http;

namespace DesafioFinalCaixaverso.API.Middlewares;

public class RegistroTelemetriaMiddleware
{
    private readonly RequestDelegate _next;

    public RegistroTelemetriaMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IRegistradorTelemetriaServicos registradorTelemetriaServicos)
    {
        var relogio = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            relogio.Stop();

            var endpoint = context.GetEndpoint();
            if (endpoint is not null)
            {
                var servico = NormalizarNomeServico(context, endpoint.DisplayName);
                var tempoResposta = relogio.ElapsedMilliseconds;
                await registradorTelemetriaServicos.RegistrarAsync(servico, tempoResposta, context.RequestAborted);
            }
        }
    }

    private static string NormalizarNomeServico(HttpContext context, string? displayName)
    {
        var path = context.Request.Path.HasValue ? context.Request.Path.Value : string.Empty;
        if (!string.IsNullOrWhiteSpace(path))
        {
            var segmentos = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segmentos.Length > 0)
            {
                if (segmentos[0].StartsWith("v", StringComparison.OrdinalIgnoreCase))
                    segmentos = segmentos.Skip(1).ToArray();

                segmentos = segmentos
                    .Where(segmento => !EhIdentificadorCliente(segmento))
                    .ToArray();

                if (segmentos.Length > 0)
                    return string.Join('-', segmentos).ToLowerInvariant();
            }
        }

        if (!string.IsNullOrWhiteSpace(displayName))
            return displayName.Trim();

        return "servico-desconhecido";
    }

    private static bool EhIdentificadorCliente(string segmento)
    {
        if (string.IsNullOrWhiteSpace(segmento))
            return false;

        return Guid.TryParse(segmento, out _);
    }
}
