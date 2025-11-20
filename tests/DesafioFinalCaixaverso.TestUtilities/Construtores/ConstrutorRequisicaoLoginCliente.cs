using Bogus;
using DesafioFinalCaixaverso.Communications.Requests;

namespace DesafioFinalCaixaverso.TestUtilities.Construtores;

public class ConstrutorRequisicaoLoginCliente
{
    private readonly RequisicaoLoginClienteJson _requisicao;

    public ConstrutorRequisicaoLoginCliente()
    {
        var faker = new Faker("pt_BR");
        _requisicao = new RequisicaoLoginClienteJson
        {
            Email = faker.Internet.Email().ToLowerInvariant(),
            Senha = "Senha@123"
        };
    }

    public ConstrutorRequisicaoLoginCliente ComEmail(string email)
    {
        _requisicao.Email = email;
        return this;
    }

    public ConstrutorRequisicaoLoginCliente ComSenha(string senha)
    {
        _requisicao.Senha = senha;
        return this;
    }

    public RequisicaoLoginClienteJson Construir() => _requisicao;
}
