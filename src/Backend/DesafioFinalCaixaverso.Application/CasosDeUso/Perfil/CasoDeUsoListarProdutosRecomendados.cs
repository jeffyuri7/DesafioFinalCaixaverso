using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Enumeradores;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using Mapster;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Perfil;

public class CasoDeUsoListarProdutosRecomendados : ICasoDeUsoListarProdutosRecomendados
{
    private readonly IProdutoRepositorio _produtoRepositorio;

    public CasoDeUsoListarProdutosRecomendados(IProdutoRepositorio produtoRepositorio)
    {
        _produtoRepositorio = produtoRepositorio;
    }

    public async Task<IReadOnlyCollection<ProdutoRecomendadoJson>> Executar(PerfilInvestidor perfil, CancellationToken cancellationToken = default)
    {
        var produtos = await _produtoRepositorio.ListarPorPerfilAsync(perfil, cancellationToken);

        return produtos.Adapt<IReadOnlyCollection<ProdutoRecomendadoJson>>();
    }
}
