using FluentMigrator;

namespace DesafioFinalCaixaverso.Infraestrutura.Migracoes;

[Migration(BancoDeDadosVersao.TABELA_CLIENTE, "Inicia a tabela para salvar as informações dos clientes.")]
public class Versao000001 : ForwardOnlyMigration
{
    public override void Up()
    {
        Create.Table("Cliente")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("Nome").AsString(200).NotNullable()
            .WithColumn("Email").AsString(200).NotNullable()
            .WithColumn("Password").AsString(200).NotNullable()
            .WithColumn("DataCriacao").AsDateTime().NotNullable();
    }
}