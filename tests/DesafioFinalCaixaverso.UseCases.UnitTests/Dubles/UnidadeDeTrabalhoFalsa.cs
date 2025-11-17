using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Repositorios;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;

public class UnidadeDeTrabalhoFalsa : IUnidadeDeTrabalho
{
    public bool CommitFoiChamado { get; private set; }

    public Task Commit()
    {
        CommitFoiChamado = true;
        return Task.CompletedTask;
    }
}
