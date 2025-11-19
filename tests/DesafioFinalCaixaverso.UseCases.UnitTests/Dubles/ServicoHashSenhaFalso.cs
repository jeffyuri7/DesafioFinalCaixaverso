using DesafioFinalCaixaverso.Dominio.Seguranca;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;

public class ServicoHashSenhaFalso : IServicoHashSenha
{
    public string HashGerado { get; private set; } = string.Empty;
    public bool ResultadoValidacao { get; set; } = true;

    public string Gerar(string senha)
    {
        HashGerado = $"hash::{senha}";
        return HashGerado;
    }

    public bool Validar(string senha, string hash)
    {
        return ResultadoValidacao;
    }
}
