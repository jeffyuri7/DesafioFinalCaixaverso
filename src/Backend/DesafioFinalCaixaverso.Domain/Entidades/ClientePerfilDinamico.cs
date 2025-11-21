using System;
using DesafioFinalCaixaverso.Dominio.Enumeradores;

namespace DesafioFinalCaixaverso.Dominio.Entidades
{
    public class ClientePerfilDinamico
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public PerfilInvestidor Perfil { get; set; }
        public decimal Pontuacao { get; set; }
        public decimal VolumeTotalInvestido { get; set; }
        public int FrequenciaMovimentacoes { get; set; }
        public bool PreferenciaLiquidez { get; set; }
        public DateTime AtualizadoEm { get; set; }

        public Cliente? Cliente { get; set; }
    }
}
