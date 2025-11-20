using System.Security.Cryptography;
using System.Text;
using DesafioFinalCaixaverso.Infraestrutura.Configuracoes;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;
using DesafioFinalCaixaverso.Dominio.Seguranca;
using Microsoft.Extensions.Options;

namespace DesafioFinalCaixaverso.Infraestrutura.Seguranca;

public class ServicoHashSenha : IServicoHashSenha
{
    private readonly string _chave;

    public ServicoHashSenha(IOptions<HashSenhaConfiguracao> configuracao)
    {
        if (configuracao is null)
            throw new ConfiguracaoInvalidaException(MensagensDeExcecao.SEGURANCA_CONFIG_HASH_NAO_INFORMADA);

        _chave = configuracao.Value.Chave;

        if (string.IsNullOrWhiteSpace(_chave))
            throw new ConfiguracaoInvalidaException(MensagensDeExcecao.SEGURANCA_CHAVE_HASH_OBRIGATORIA);
    }

    public string Gerar(string senha)
    {
        var texto = senha + _chave;
        var bytes = Encoding.UTF8.GetBytes(texto);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes);
    }

    public bool Validar(string senha, string hash)
    {
        var hashComparacao = Gerar(senha);
        return string.Equals(hashComparacao, hash, StringComparison.Ordinal);
    }
}
