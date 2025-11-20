using System;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Clientes;

public class CasoDeUsoExcluirClienteTestes
{
    [Fact]
    public async Task Deve_remover_cliente_existente()
    {
        var cliente = new ConstrutorCliente().Construir();
        var clienteRepositorio = new ClienteRepositorioFalso().ComClienteExistente(cliente);
        var unidadeDeTrabalho = new UnidadeDeTrabalhoFalsa();
        var casoDeUso = new CasoDeUsoExcluirCliente(clienteRepositorio, unidadeDeTrabalho);

        await casoDeUso.Executar(cliente.Id);

        (await clienteRepositorio.ExisteClienteAsync(cliente.Id)).ShouldBeFalse();
        unidadeDeTrabalho.CommitFoiChamado.ShouldBeTrue();
    }

    [Fact]
    public async Task Deve_lancar_excecao_quando_cliente_nao_existir()
    {
        var clienteRepositorio = new ClienteRepositorioFalso().SemCliente();
    var unidadeDeTrabalho = new UnidadeDeTrabalhoFalsa();
    var casoDeUso = new CasoDeUsoExcluirCliente(clienteRepositorio, unidadeDeTrabalho);

        await Should.ThrowAsync<NaoEncontradoException>(() => casoDeUso.Executar(Guid.NewGuid()));
    }
}
