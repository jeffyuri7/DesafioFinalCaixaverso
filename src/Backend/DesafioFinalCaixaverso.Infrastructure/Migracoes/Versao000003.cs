using FluentMigrator;

namespace DesafioFinalCaixaverso.Infraestrutura.Migracoes;

[Migration(BancoDeDadosVersao.TABELA_SIMULACAO, "Inicia a tabela para salvar as informações de simulações.")]
public class Versao000003 : ForwardOnlyMigration
{
    public override void Up()
    {
    Create.Table("Simulacoes")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("ClienteId").AsGuid().NotNullable()
            .WithColumn("ProdutoId").AsGuid().NotNullable()
            .WithColumn("ValorInvestido").AsDecimal().NotNullable()
            .WithColumn("ValorFinal").AsDecimal().NotNullable()
            .WithColumn("RentabilidadeEfetiva").AsDecimal().NotNullable()
            .WithColumn("PrazoMeses").AsInt32().NotNullable()
            .WithColumn("DataSimulacao").AsDateTime().NotNullable();
    }
} 