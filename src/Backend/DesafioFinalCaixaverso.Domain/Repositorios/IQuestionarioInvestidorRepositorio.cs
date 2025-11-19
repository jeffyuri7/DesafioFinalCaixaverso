using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;

namespace DesafioFinalCaixaverso.Dominio.Repositorios;

public interface IQuestionarioInvestidorRepositorio
{
    Task<QuestionarioInvestidor?> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task AdicionarAsync(QuestionarioInvestidor questionario, CancellationToken cancellationToken = default);
    Task AtualizarAsync(QuestionarioInvestidor questionario, CancellationToken cancellationToken = default);
}
