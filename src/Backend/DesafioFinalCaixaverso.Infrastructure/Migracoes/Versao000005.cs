using FluentMigrator;

namespace DesafioFinalCaixaverso.Infraestrutura.Migracoes;

[Migration(BancoDeDadosVersao.TABELA_CLIENTE_PERFIL, "Cria tabela de perfis de clientes.")]
public class Versao000005 : ForwardOnlyMigration
{
    private const string Tabela = "ClientePerfis";

    public override void Up()
    {
        Create.Table(Tabela)
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("ClienteId").AsGuid().NotNullable()
            .WithColumn("Perfil").AsInt32().NotNullable()
            .WithColumn("Pontuacao").AsInt32().NotNullable()
            .WithColumn("ValorMedioInvestido").AsDecimal(18, 2).NotNullable()
            .WithColumn("ValorTotalInvestido").AsDecimal(18, 2).NotNullable()
            .WithColumn("SimulacoesUltimos30Dias").AsInt32().NotNullable()
            .WithColumn("RentabilidadeMediaProduto").AsDecimal(18, 4).NotNullable()
            .WithColumn("LiquidezMediaDias").AsInt32().NotNullable()
            .WithColumn("AtualizadoEm").AsDateTime().NotNullable();

        Create.Index("IX_ClientePerfis_ClienteId")
            .OnTable(Tabela)
            .OnColumn("ClienteId").Ascending().WithOptions().Unique();

        Create.ForeignKey("FK_ClientePerfis_Cliente")
            .FromTable(Tabela).ForeignColumn("ClienteId")
            .ToTable("Cliente").PrimaryColumn("Id");
    }
}
