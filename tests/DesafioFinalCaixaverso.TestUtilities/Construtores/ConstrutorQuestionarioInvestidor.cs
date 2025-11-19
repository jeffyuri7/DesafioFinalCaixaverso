using System;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Enumeradores;

namespace DesafioFinalCaixaverso.TestUtilities.Construtores;

public class ConstrutorQuestionarioInvestidor
{
    private readonly QuestionarioInvestidor _questionario;

    public ConstrutorQuestionarioInvestidor()
    {
        _questionario = new QuestionarioInvestidor
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            PreferenciaLiquidez = PreferenciaLiquidez.Media,
            ObjetivoInvestimento = ObjetivoInvestimento.Equilibrio,
            NivelConhecimento = NivelConhecimentoInvestidor.Intermediario,
            HorizonteMeses = 24,
            RendaMensal = 8_000m,
            PatrimonioTotal = 300_000m,
            ToleranciaPerdaPercentual = 10m,
            FonteRendaEstavel = true,
            AtualizadoEm = DateTime.UtcNow
        };
    }

    public ConstrutorQuestionarioInvestidor ComClienteId(Guid clienteId)
    {
        _questionario.ClienteId = clienteId;
        return this;
    }

    public ConstrutorQuestionarioInvestidor ComPreferenciaLiquidez(PreferenciaLiquidez preferencia)
    {
        _questionario.PreferenciaLiquidez = preferencia;
        return this;
    }

    public ConstrutorQuestionarioInvestidor ComObjetivoInvestimento(ObjetivoInvestimento objetivo)
    {
        _questionario.ObjetivoInvestimento = objetivo;
        return this;
    }

    public ConstrutorQuestionarioInvestidor ComNivelConhecimento(NivelConhecimentoInvestidor nivel)
    {
        _questionario.NivelConhecimento = nivel;
        return this;
    }

    public ConstrutorQuestionarioInvestidor ComHorizonteMeses(int meses)
    {
        _questionario.HorizonteMeses = meses;
        return this;
    }

    public ConstrutorQuestionarioInvestidor ComRendaMensal(decimal renda)
    {
        _questionario.RendaMensal = renda;
        return this;
    }

    public ConstrutorQuestionarioInvestidor ComPatrimonioTotal(decimal patrimonio)
    {
        _questionario.PatrimonioTotal = patrimonio;
        return this;
    }

    public ConstrutorQuestionarioInvestidor ComToleranciaPerdaPercentual(decimal percentual)
    {
        _questionario.ToleranciaPerdaPercentual = percentual;
        return this;
    }

    public ConstrutorQuestionarioInvestidor ComFonteRendaEstavel(bool estavel)
    {
        _questionario.FonteRendaEstavel = estavel;
        return this;
    }

    public ConstrutorQuestionarioInvestidor ComAtualizadoEm(DateTime data)
    {
        _questionario.AtualizadoEm = data;
        return this;
    }

    public QuestionarioInvestidor Construir() => _questionario;
}
