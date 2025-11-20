namespace DesafioFinalCaixaverso.Communications.Requests;

public class RequisicaoAtualizacaoClienteJson
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Senha { get; set; }
}
