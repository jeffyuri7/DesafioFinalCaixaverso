using DesafioFinalCaixaverso.Aplicacao.Servicos.Perfis;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Entidades;
using Mapster;

namespace DesafioFinalCaixaverso.Aplicacao.Mapeamentos;

public static class MapsterConfiguracoes
{
    private static bool _configuracoesRegistradas;

    public static void Registrar()
    {
        if (_configuracoesRegistradas)
            return;

        _configuracoesRegistradas = true;

        TypeAdapterConfig<Simulacao, HistoricoSimulacaoJson>
            .NewConfig()
            .Map(destino => destino.ProdutoNome, origem => origem.Produto != null ? origem.Produto.Nome : string.Empty);

        TypeAdapterConfig<Produto, ProdutoRecomendadoJson>
            .NewConfig()
            .Map(destino => destino.Risco, origem => origem.Risco.ToString());

        TypeAdapterConfig<PerfilInvestidorResultado, PerfilClienteJson>
            .NewConfig()
            .Map(destino => destino.Perfil, origem => origem.Perfil.ToString());

        TypeAdapterConfig<RequisicaoCadastroClienteJson, Cliente>
            .NewConfig()
            .Ignore(destino => destino.Id)
            .Ignore(destino => destino.Password)
            .Ignore(destino => destino.DataCriacao)
            .Map(destino => destino.Nome, origem => origem.Nome.Trim())
            .Map(destino => destino.Email, origem => origem.Email.Trim().ToLowerInvariant());

        TypeAdapterConfig<Cliente, ClienteCadastradoJson>
            .NewConfig()
            .Map(destino => destino.ClienteId, origem => origem.Id)
            .Map(destino => destino.CriadoEm, origem => origem.DataCriacao);

        TypeAdapterConfig<RequisicaoQuestionarioClienteJson, QuestionarioInvestidor>
            .NewConfig()
            .Ignore(destino => destino.Id)
            .Ignore(destino => destino.ClienteId)
            .Ignore(destino => destino.AtualizadoEm);

        TypeAdapterConfig<QuestionarioInvestidor, QuestionarioRespondidoJson>
            .NewConfig();
    }
}