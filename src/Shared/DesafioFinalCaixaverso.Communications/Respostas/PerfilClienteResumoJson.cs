using System;

namespace DesafioFinalCaixaverso.Communications.Responses;

public class PerfilClienteResumoJson
{
    public Guid ClienteId { get; set; }
    public string Perfil { get; set; } = string.Empty;
    public int Pontuacao { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
