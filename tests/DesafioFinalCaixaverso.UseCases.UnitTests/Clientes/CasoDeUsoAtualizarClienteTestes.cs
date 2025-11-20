using System;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Clientes;

public class CasoDeUsoAtualizarClienteTestes
{
    [Fact]
    public async Task Deve_atualizar_dados_basicos_sem_alterar_senha_quando_nao_informada()
    {
        var clienteExistente = new ConstrutorCliente().Construir();
        var senhaAnterior = clienteExistente.Password;
        var clienteRepositorio = new ClienteRepositorioFalso().ComClienteExistente(clienteExistente);
        var unidadeDeTrabalho = new UnidadeDeTrabalhoFalsa();
        var servicoHash = new ServicoHashSenhaFalso();
        var casoDeUso = new CasoDeUsoAtualizarCliente(clienteRepositorio, unidadeDeTrabalho, servicoHash);

        var requisicao = new RequisicaoAtualizacaoClienteJson
        {
            Nome = "Novo Nome",
            Email = "novo@email.com",
            Senha = null
        };

        var resposta = await casoDeUso.Executar(clienteExistente.Id, requisicao);

        resposta.Nome.ShouldBe("Novo Nome");
        resposta.Email.ShouldBe("novo@email.com");
        clienteExistente.Password.ShouldBe(senhaAnterior);
        unidadeDeTrabalho.CommitFoiChamado.ShouldBeTrue();
    }

    [Fact]
    public async Task Deve_atualizar_senha_quando_nova_senha_for_informada()
    {
        var clienteExistente = new ConstrutorCliente().Construir();
        var clienteRepositorio = new ClienteRepositorioFalso().ComClienteExistente(clienteExistente);
        var unidadeDeTrabalho = new UnidadeDeTrabalhoFalsa();
        var servicoHash = new ServicoHashSenhaFalso();
        var casoDeUso = new CasoDeUsoAtualizarCliente(clienteRepositorio, unidadeDeTrabalho, servicoHash);

        var requisicao = new RequisicaoAtualizacaoClienteJson
        {
            Nome = clienteExistente.Nome,
            Email = clienteExistente.Email,
            Senha = "novaSenha123"
        };

        var resposta = await casoDeUso.Executar(clienteExistente.Id, requisicao);

        resposta.ShouldNotBeNull();
        servicoHash.HashGerado.ShouldBe("hash::novaSenha123");
        clienteExistente.Password.ShouldBe("hash::novaSenha123");
    }

    [Fact]
    public async Task Deve_lancar_excecao_quando_cliente_nao_existir()
    {
        var clienteRepositorio = new ClienteRepositorioFalso().SemCliente();
        var unidadeDeTrabalho = new UnidadeDeTrabalhoFalsa();
        var servicoHash = new ServicoHashSenhaFalso();
        var casoDeUso = new CasoDeUsoAtualizarCliente(clienteRepositorio, unidadeDeTrabalho, servicoHash);

        var requisicao = new RequisicaoAtualizacaoClienteJson
        {
            Nome = "Teste",
            Email = "teste@teste.com",
            Senha = null
        };

        await Should.ThrowAsync<NaoEncontradoException>(() => casoDeUso.Executar(Guid.NewGuid(), requisicao));
    }
}
