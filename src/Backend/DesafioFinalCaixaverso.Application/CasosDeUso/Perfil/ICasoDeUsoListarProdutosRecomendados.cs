using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Enumeradores;
using System.Collections.Generic;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Perfil;

public interface ICasoDeUsoListarProdutosRecomendados
{
    Task<IReadOnlyCollection<ProdutoRecomendadoJson>> Executar(PerfilInvestidor perfil, CancellationToken cancellationToken = default);
}
