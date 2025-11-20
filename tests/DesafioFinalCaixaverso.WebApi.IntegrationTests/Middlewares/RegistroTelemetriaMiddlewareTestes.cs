using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.API.Middlewares;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Telemetria;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests.Middlewares;

public class RegistroTelemetriaMiddlewareTestes
{
    [Fact]
    public async Task Deve_registrar_servico_utilizando_display_name()
    {
        var registrador = new RegistradorTelemetriaServicosFalso();
        var middleware = new RegistroTelemetriaMiddleware(_ => Task.CompletedTask);
        var context = new DefaultHttpContext();
        context.SetEndpoint(new Endpoint(_ => Task.CompletedTask, new EndpointMetadataCollection(), "GET v1/investimentos/simulacoes"));

        await middleware.InvokeAsync(context, registrador);

        registrador.ServicosRegistrados.ShouldContain(entry => entry.Servico == "GET v1/investimentos/simulacoes" && entry.TempoRespostaMs >= 0);
    }

    [Fact]
    public async Task Deve_normalizar_caminho_quando_display_name_nao_existir()
    {
        var registrador = new RegistradorTelemetriaServicosFalso();
        var middleware = new RegistroTelemetriaMiddleware(_ => Task.CompletedTask);
        var context = new DefaultHttpContext();
        context.Request.Path = "/v1/telemetria";
        context.SetEndpoint(new Endpoint(_ => Task.CompletedTask, new EndpointMetadataCollection(), null));

        await middleware.InvokeAsync(context, registrador);

        registrador.ServicosRegistrados.ShouldContain(entry => entry.Servico == "telemetria");
    }

    [Fact]
    public async Task Deve_remover_clientId_do_nome_do_servico()
    {
        var registrador = new RegistradorTelemetriaServicosFalso();
        var middleware = new RegistroTelemetriaMiddleware(_ => Task.CompletedTask);
        var context = new DefaultHttpContext();
        var clienteId = Guid.NewGuid();
        context.Request.Path = $"/v1/clientes/{clienteId}/investimentos";
        context.SetEndpoint(new Endpoint(_ => Task.CompletedTask, new EndpointMetadataCollection(), null));

        await middleware.InvokeAsync(context, registrador);

        registrador.ServicosRegistrados.ShouldContain(entry => entry.Servico == "clientes-investimentos");
    }

    private sealed class RegistradorTelemetriaServicosFalso : IRegistradorTelemetriaServicos
    {
        public List<(string Servico, long TempoRespostaMs)> ServicosRegistrados { get; } = new();

        public Task RegistrarAsync(string servico, long tempoRespostaMs, CancellationToken cancellationToken = default)
        {
            ServicosRegistrados.Add((servico, tempoRespostaMs));
            return Task.CompletedTask;
        }
    }
}
