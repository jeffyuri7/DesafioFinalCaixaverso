namespace DesafioFinalCaixaverso.Infraestrutura.Configuracoes;

public class JwtTokenConfiguracao
{
    public const string Secao = "Seguranca:Jwt";
    public string Chave { get; set; } = string.Empty;
    public string Emissor { get; set; } = string.Empty;
    public string Audiencia { get; set; } = string.Empty;
    public int ExpiracaoMinutos { get; set; } = 60;
}
