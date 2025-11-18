using FluentMigrator;

namespace DesafioFinalCaixaverso.Infraestrutura.Migracoes;

[Migration(BancoDeDadosVersao.TABELA_TELEMETRIA_SERVICO, "Cria tabela de telemetria de serviços.")]
public class Versao000004 : ForwardOnlyMigration
{
    public override void Up()
    {
        Create.Table("TelemetriaServicos")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("Servico").AsString(100).NotNullable()
            .WithColumn("QuantidadeChamadas").AsInt32().NotNullable()
            .WithColumn("UltimaChamada").AsDateTime().NotNullable();

        Create.Index("IX_TelemetriaServicos_Servico")
            .OnTable("TelemetriaServicos")
            .OnColumn("Servico").Ascending().WithOptions().Unique();
    }
}
