using DesafioFinalCaixaverso.Dominio.Entidades;

namespace DesafioFinalCaixaverso.Dominio.Repositorios;

public interface ISimulacaoRepositorio
{
    Task AdicionarAsync(Simulacao simulacao);
}
