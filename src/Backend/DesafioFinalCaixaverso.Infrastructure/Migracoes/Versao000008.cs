using FluentMigrator;

namespace DesafioFinalCaixaverso.Infraestrutura.Migracoes;

[Migration(BancoDeDadosVersao.AJUSTE_CLIENTE_PERFIL_DADOS_SUFICIENTES, "Adiciona coluna DadosSuficientes em ClientePerfis.")]
public class Versao000008 : ForwardOnlyMigration
{
    private const string Tabela = "ClientePerfis";

    public override void Up()
    {
        Alter.Table(Tabela)
            .AddColumn("DadosSuficientes")
            .AsBoolean()
            .NotNullable()
            .WithDefaultValue(false);
    }
}
