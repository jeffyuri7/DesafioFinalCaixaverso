using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;

public class QuestionarioInvestidorRepositorioFalso : IQuestionarioInvestidorRepositorio
{
    public QuestionarioInvestidor? QuestionarioSalvo { get; private set; }

    public QuestionarioInvestidorRepositorioFalso ComQuestionario(QuestionarioInvestidor questionario)
    {
        QuestionarioSalvo = questionario;
        return this;
    }

    public Task<QuestionarioInvestidor?> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        if (QuestionarioSalvo is not null && QuestionarioSalvo.ClienteId == clienteId)
            return Task.FromResult<QuestionarioInvestidor?>(QuestionarioSalvo);

        return Task.FromResult<QuestionarioInvestidor?>(null);
    }

    public Task AdicionarAsync(QuestionarioInvestidor questionario, CancellationToken cancellationToken = default)
    {
        QuestionarioSalvo = questionario;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(QuestionarioInvestidor questionario, CancellationToken cancellationToken = default)
    {
        QuestionarioSalvo = questionario;
        return Task.CompletedTask;
    }
}
