using FluentMigrator;

namespace DesafioFinalCaixaverso.Infraestrutura.Migracoes;

[Migration(BancoDeDadosVersao.TABELA_QUESTIONARIO_INVESTIDOR, "Cria questionário do investidor e adiciona metadados ao perfil.")]
public class Versao000006 : ForwardOnlyMigration
{
    private const string QuestionarioTabela = "QuestionariosInvestidor";

    public override void Up()
    {
        CriarTabelaQuestionario();
    }

    private void CriarTabelaQuestionario()
    {
        Create.Table(QuestionarioTabela)
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("ClienteId").AsGuid().NotNullable()
            .WithColumn("PreferenciaLiquidez").AsInt32().NotNullable()
            .WithColumn("ObjetivoInvestimento").AsInt32().NotNullable()
            .WithColumn("NivelConhecimento").AsInt32().NotNullable()
            .WithColumn("HorizonteMeses").AsInt32().NotNullable()
            .WithColumn("RendaMensal").AsDecimal(18, 2).NotNullable()
            .WithColumn("PatrimonioTotal").AsDecimal(18, 2).NotNullable()
            .WithColumn("ToleranciaPerdaPercentual").AsDecimal(5, 2).NotNullable()
            .WithColumn("FonteRendaEstavel").AsBoolean().NotNullable()
            .WithColumn("AtualizadoEm").AsDateTime().NotNullable();

        Create.Index("IX_QuestionariosInvestidor_ClienteId")
            .OnTable(QuestionarioTabela)
            .OnColumn("ClienteId").Ascending().WithOptions().Unique();

        Create.ForeignKey("FK_QuestionariosInvestidor_Clientes")
            .FromTable(QuestionarioTabela).ForeignColumn("ClienteId")
            .ToTable("Clientes").PrimaryColumn("Id");
    }

}
