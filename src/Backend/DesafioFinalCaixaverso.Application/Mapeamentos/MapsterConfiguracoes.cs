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
    }
}