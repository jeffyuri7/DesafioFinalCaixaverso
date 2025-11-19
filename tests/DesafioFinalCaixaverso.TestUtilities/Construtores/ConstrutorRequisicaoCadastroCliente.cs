using Bogus;
using DesafioFinalCaixaverso.Communications.Requests;

namespace DesafioFinalCaixaverso.TestUtilities.Construtores;

public class ConstrutorRequisicaoCadastroCliente
{
    private readonly RequisicaoCadastroClienteJson _requisicao;

    public ConstrutorRequisicaoCadastroCliente()
    {
        var faker = new Faker("pt_BR");
        _requisicao = new RequisicaoCadastroClienteJson
        {
            Nome = faker.Person.FullName,
            Email = faker.Internet.Email().ToLowerInvariant(),
            Senha = "Senha@123"
        };
    }

    public ConstrutorRequisicaoCadastroCliente ComNome(string nome)
    {
        _requisicao.Nome = nome;
        return this;
    }

    public ConstrutorRequisicaoCadastroCliente ComEmail(string email)
    {
        _requisicao.Email = email;
        return this;
    }

    public ConstrutorRequisicaoCadastroCliente ComSenha(string senha)
    {
        _requisicao.Senha = senha;
        return this;
    }

    public RequisicaoCadastroClienteJson Construir() => _requisicao;
}
