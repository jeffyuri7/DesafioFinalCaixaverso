using System.Data;
using FluentMigrator;

namespace DesafioFinalCaixaverso.Infraestrutura.Migracoes;

[Migration(BancoDeDadosVersao.TABELA_CLIENTE_PERFIL_DINAMICO, "Cria tabela de perfis dinâmicos de clientes.")]
public class Versao000007 : ForwardOnlyMigration
{
    private const string Tabela = "ClientePerfisDinamicos";

    public override void Up()
    {
        Create.Table(Tabela)
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("ClienteId").AsGuid().NotNullable()
            .WithColumn("Perfil").AsInt32().NotNullable()
            .WithColumn("Pontuacao").AsDecimal(10,2).NotNullable()
            .WithColumn("VolumeTotalInvestido").AsDecimal(18,2).NotNullable()
            .WithColumn("FrequenciaMovimentacoes").AsInt32().NotNullable()
            .WithColumn("PreferenciaLiquidez").AsBoolean().NotNullable()
            .WithColumn("AtualizadoEm").AsDateTime().NotNullable();

        Create.Index("IX_ClientePerfilDinamico_ClienteId")
            .OnTable(Tabela)
            .OnColumn("ClienteId").Ascending().WithOptions().Unique();

        Create.ForeignKey("FK_ClientePerfilDinamico_Clientes")
            .FromTable(Tabela).ForeignColumn("ClienteId")
            .ToTable("Clientes").PrimaryColumn("Id")
            .OnDelete(Rule.Cascade);
    }
}
