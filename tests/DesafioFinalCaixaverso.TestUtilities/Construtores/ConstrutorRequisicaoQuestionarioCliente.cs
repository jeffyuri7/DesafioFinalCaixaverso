using Bogus;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Dominio.Enumeradores;

namespace DesafioFinalCaixaverso.TestUtilities.Construtores;

public class ConstrutorRequisicaoQuestionarioCliente
{
    private readonly RequisicaoQuestionarioClienteJson _requisicao;

    public ConstrutorRequisicaoQuestionarioCliente()
    {
        var faker = new Faker("pt_BR");
        _requisicao = new RequisicaoQuestionarioClienteJson
        {
            PreferenciaLiquidez = faker.PickRandom<PreferenciaLiquidez>(),
            ObjetivoInvestimento = faker.PickRandom<ObjetivoInvestimento>(),
            NivelConhecimento = faker.PickRandom<NivelConhecimentoInvestidor>(),
            HorizonteMeses = faker.Random.Int(6, 60),
            RendaMensal = faker.Random.Decimal(2_000m, 20_000m),
            PatrimonioTotal = faker.Random.Decimal(10_000m, 250_000m),
            ToleranciaPerdaPercentual = faker.Random.Decimal(0m, 50m),
            FonteRendaEstavel = faker.Random.Bool()
        };
    }

    public ConstrutorRequisicaoQuestionarioCliente ComPreferenciaLiquidez(PreferenciaLiquidez preferencia)
    {
        _requisicao.PreferenciaLiquidez = preferencia;
        return this;
    }

    public ConstrutorRequisicaoQuestionarioCliente ComObjetivoInvestimento(ObjetivoInvestimento objetivo)
    {
        _requisicao.ObjetivoInvestimento = objetivo;
        return this;
    }

    public ConstrutorRequisicaoQuestionarioCliente ComNivelConhecimento(NivelConhecimentoInvestidor nivel)
    {
        _requisicao.NivelConhecimento = nivel;
        return this;
    }

    public ConstrutorRequisicaoQuestionarioCliente ComHorizonte(int meses)
    {
        _requisicao.HorizonteMeses = meses;
        return this;
    }

    public ConstrutorRequisicaoQuestionarioCliente ComRenda(decimal renda)
    {
        _requisicao.RendaMensal = renda;
        return this;
    }

    public ConstrutorRequisicaoQuestionarioCliente ComPatrimonio(decimal patrimonio)
    {
        _requisicao.PatrimonioTotal = patrimonio;
        return this;
    }

    public ConstrutorRequisicaoQuestionarioCliente ComTolerancia(decimal tolerancia)
    {
        _requisicao.ToleranciaPerdaPercentual = tolerancia;
        return this;
    }

    public ConstrutorRequisicaoQuestionarioCliente ComFonteRendaEstavel(bool estavel)
    {
        _requisicao.FonteRendaEstavel = estavel;
        return this;
    }

    public RequisicaoQuestionarioClienteJson Construir() => _requisicao;
}
