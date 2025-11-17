using System;
using System.Collections.Generic;

namespace DesafioFinalCaixaverso.Dominio.Entidades;

public class Cliente
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public ICollection<Simulacao> Simulacoes { get; set; } = new List<Simulacao>();
}
