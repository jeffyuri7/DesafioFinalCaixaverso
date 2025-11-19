using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Enumeradores;

namespace DesafioFinalCaixaverso.Dominio.Repositorios;

public interface IProdutoRepositorio
{
    Task<IReadOnlyCollection<Produto>> ListarAtivosPorTipoAsync(string tipoProduto);
    Task AdicionarAsync(Produto produto);
    Task<IReadOnlyCollection<Produto>> ListarPorPerfilAsync(PerfilInvestidor perfil, CancellationToken cancellationToken = default);
}
