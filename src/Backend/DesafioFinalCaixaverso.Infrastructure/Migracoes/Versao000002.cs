using FluentMigrator;

namespace DesafioFinalCaixaverso.Infraestrutura.Migracoes;

[Migration(BancoDeDadosVersao.TABELA_PRODUTO, "Inicia a tabela para salvar as informações dos clientes.")]
public class Versao000002 : ForwardOnlyMigration
{
    public override void Up()
    {
    Create.Table("Produtos")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("Nome").AsString(200).NotNullable()
            .WithColumn("Tipo").AsString(100).NotNullable()
            .WithColumn("Rentabilidade").AsDecimal().NotNullable()
            .WithColumn("Risco").AsInt32().NotNullable()
            .WithColumn("LiquidezDias").AsInt32().NotNullable()
            .WithColumn("MinimoInvestimento").AsDecimal().NotNullable()
            .WithColumn("PrazoMinimoMeses").AsInt32().NotNullable()
            .WithColumn("PrazoMaximoMeses").AsInt32().NotNullable()
            .WithColumn("Ativo").AsBoolean().NotNullable();
    }
}