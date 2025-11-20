using System.Linq;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Clientes;

public class CasoDeUsoListarClientesTestes
{
    [Fact]
    public async Task Deve_ordenar_clientes_por_nome()
    {
        var primeiro = new ConstrutorCliente().ComEmail("a@teste.com").Construir();
        primeiro.Nome = "Carlos";
        var segundo = new ConstrutorCliente().ComEmail("b@teste.com").Construir();
        segundo.Nome = "Ana";

        var clienteRepositorio = new ClienteRepositorioFalso()
            .ComClienteExistente(primeiro)
            .ComClienteExistente(segundo);

        var casoDeUso = new CasoDeUsoListarClientes(clienteRepositorio);

        var resposta = await casoDeUso.Executar();

        resposta.Count.ShouldBe(2);
        resposta.First().Nome.ShouldBe("Ana");
        resposta.Last().Nome.ShouldBe("Carlos");
    }
}
