using System;
using System.Collections.Generic;

namespace DesafioFinalCaixaverso.Dominio.Entidades;

public class Produto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Rentabilidade { get; set; }
    public Enumeradores.Risco Risco { get; set; }
    public int LiquidezDias { get; set; }
    public decimal MinimoInvestimento { get; set; }
    public int PrazoMinimoMeses { get; set; }
    public int PrazoMaximoMeses { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<Simulacao> Simulacoes { get; set; } = new List<Simulacao>();
}
