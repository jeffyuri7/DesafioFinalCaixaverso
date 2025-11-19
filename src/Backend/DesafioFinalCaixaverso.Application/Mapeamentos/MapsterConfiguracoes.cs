using DesafioFinalCaixaverso.Aplicacao.Servicos.Perfis;
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
    }
}