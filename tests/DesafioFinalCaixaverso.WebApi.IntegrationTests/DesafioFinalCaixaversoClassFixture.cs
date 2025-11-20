using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Seguranca;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests;

public class DesafioFinalCaixaversoClassFixture : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;
    protected CustomWebApplicationFactory Factory { get; }

    protected DesafioFinalCaixaversoClassFixture(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        _httpClient = factory.CreateClient();
    }

    protected async Task<HttpResponseMessage> DoPost(
        string method,
        object request,
        string token = "")
    {
        AuthorizeRequest(token);

        return await _httpClient.PostAsJsonAsync(method, request);
    }

    protected async Task<HttpResponseMessage> DoGet(
        string method,
        string token = "")
    {
        AuthorizeRequest(token);

        return await _httpClient.GetAsync(method);
    }

    protected async Task<HttpResponseMessage> DoPut(
        string method,
        object request,
        string token = "")
    {
        AuthorizeRequest(token);

        return await _httpClient.PutAsJsonAsync(method, request);
    }

    protected async Task<HttpResponseMessage> DoDelete(
        string method,
        string token = "")
    {
        AuthorizeRequest(token);

        return await _httpClient.DeleteAsync(method);
    }

    protected string GerarToken(Guid clienteId)
    {
        using var scope = Factory.Services.CreateScope();
        var gerador = scope.ServiceProvider.GetRequiredService<IGeradorTokenAcesso>();
        var token = gerador.Gerar(clienteId);
        return token.Valor;
    }
    private void AuthorizeRequest(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            return;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
