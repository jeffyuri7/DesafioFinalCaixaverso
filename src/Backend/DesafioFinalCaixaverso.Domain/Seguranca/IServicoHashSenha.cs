namespace DesafioFinalCaixaverso.Dominio.Seguranca;

public interface IServicoHashSenha
{
    string Gerar(string senha);
    bool Validar(string senha, string hash);
}
