using FluentMigrator;

namespace DesafioFinalCaixaverso.Infraestrutura.Migracoes;

[Migration(BancoDeDadosVersao.TABELA_TELEMETRIA_SERVICO, "Cria tabela de telemetria de serviços.")]
public class Versao000004 : ForwardOnlyMigration
{
    public override void Up()
    {
        Create.Table("TelemetriaServicos")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("Servico").AsString(300).NotNullable()
            .WithColumn("AnoReferencia").AsInt32().NotNullable()
            .WithColumn("MesReferencia").AsInt32().NotNullable()
            .WithColumn("QuantidadeChamadas").AsInt32().NotNullable()
            .WithColumn("TempoTotalRespostaMs").AsInt64().NotNullable().WithDefaultValue(0)
            .WithColumn("UltimaChamada").AsDateTime().NotNullable();

        Create.Index("IX_TelemetriaServicos_Servico_Ano_Mes")
            .OnTable("TelemetriaServicos")
            .OnColumn("Servico").Ascending()
            .OnColumn("AnoReferencia").Ascending()
            .OnColumn("MesReferencia").Ascending()
            .WithOptions().Unique();
    }
}
