using System.Collections.Generic;
using DesafioFinalCaixaverso.Dominio.Entidades;

namespace DesafioFinalCaixaverso.Dominio.Repositorios;

public interface IProdutoRepositorio
{
    Task<IReadOnlyCollection<Produto>> ListarAtivosPorTipoAsync(string tipoProduto);
    Task AdicionarAsync(Produto produto);
}
