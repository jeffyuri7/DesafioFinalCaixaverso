using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DesafioFinalCaixaverso.Dominio.Seguranca;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;
using DesafioFinalCaixaverso.Infraestrutura.Configuracoes;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DesafioFinalCaixaverso.Infraestrutura.Seguranca;

public class GeradorTokenJwt : IGeradorTokenAcesso
{
    private readonly JwtTokenConfiguracao _configuracao;

    public GeradorTokenJwt(IOptions<JwtTokenConfiguracao> configuracoes)
    {
        if (configuracoes is null)
            throw new ConfiguracaoInvalidaException(MensagensDeExcecao.SEGURANCA_CONFIG_JWT_NAO_INFORMADA);

        _configuracao = configuracoes.Value ?? throw new ConfiguracaoInvalidaException(MensagensDeExcecao.SEGURANCA_CONFIG_JWT_NAO_INFORMADA);

        ValidarConfiguracao();
    }

    public TokenAcesso Gerar(Guid clienteId)
    {
        var horaAtual = DateTime.UtcNow;
        var expiracao = horaAtual.AddMinutes(_configuracao.ExpiracaoMinutos);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, clienteId.ToString()),
            new(JwtRegisteredClaimNames.Sub, clienteId.ToString())
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracao.Chave));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuracao.Emissor,
            audience: _configuracao.Audiencia,
            claims: claims,
            notBefore: horaAtual,
            expires: expiracao,
            signingCredentials: credenciais);

        var handler = new JwtSecurityTokenHandler();
        var valorToken = handler.WriteToken(token);

        return new TokenAcesso(valorToken, expiracao);
    }

    private void ValidarConfiguracao()
    {
        if (string.IsNullOrWhiteSpace(_configuracao.Chave))
            throw new ConfiguracaoInvalidaException(MensagensDeExcecao.SEGURANCA_CHAVE_JWT_OBRIGATORIA);

        if (string.IsNullOrWhiteSpace(_configuracao.Emissor))
            throw new ConfiguracaoInvalidaException(MensagensDeExcecao.SEGURANCA_EMISSOR_JWT_OBRIGATORIO);

        if (string.IsNullOrWhiteSpace(_configuracao.Audiencia))
            throw new ConfiguracaoInvalidaException(MensagensDeExcecao.SEGURANCA_AUDIENCIA_JWT_OBRIGATORIA);

        if (_configuracao.ExpiracaoMinutos <= 0)
            throw new ConfiguracaoInvalidaException(MensagensDeExcecao.SEGURANCA_EXPIRACAO_JWT_INVALIDA);
    }
}
