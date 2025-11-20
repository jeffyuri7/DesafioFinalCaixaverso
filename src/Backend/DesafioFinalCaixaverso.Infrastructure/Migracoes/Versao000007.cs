using FluentMigrator;

namespace DesafioFinalCaixaverso.Infraestrutura.Migracoes;

[Migration(BancoDeDadosVersao.AJUSTES_TELEMETRIA_SERVICO, "Adiciona período e tempo médio à telemetria de serviços.")]
public class Versao000007 : ForwardOnlyMigration
{
    public override void Up()
    {
        Alter.Table("TelemetriaServicos")
            .AddColumn("AnoReferencia").AsInt32().Nullable()
            .AddColumn("MesReferencia").AsInt32().Nullable()
            .AddColumn("TempoTotalRespostaMs").AsInt64().NotNullable().WithDefaultValue(0);

        Execute.Sql(@"
            UPDATE TelemetriaServicos
            SET AnoReferencia = DATEPART(year, UltimaChamada),
                MesReferencia = DATEPART(month, UltimaChamada)
        ");

        Alter.Column("AnoReferencia").OnTable("TelemetriaServicos").AsInt32().NotNullable();
        Alter.Column("MesReferencia").OnTable("TelemetriaServicos").AsInt32().NotNullable();

        Delete.Index("IX_TelemetriaServicos_Servico").OnTable("TelemetriaServicos");

        Create.Index("IX_TelemetriaServicos_Servico_Ano_Mes")
            .OnTable("TelemetriaServicos")
            .OnColumn("Servico").Ascending()
            .OnColumn("AnoReferencia").Ascending()
            .OnColumn("MesReferencia").Ascending()
            .WithOptions().Unique();
    }
}
