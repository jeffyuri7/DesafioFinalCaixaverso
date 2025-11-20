
using Dapper;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioFinalCaixaverso.Infraestrutura.Migracoes;

public static class BancoDeDadosMigracao
{
    public static void Migracao(string connectionString, IServiceProvider serviceProvider)
    {
        VerificarBancoDeDadosCriado(connectionString);

        MigrarBancoDeDados(serviceProvider);
    }

    private static void VerificarBancoDeDadosCriado(string connectionString)
    {
        var construtorBancoDeDados = new SqlConnectionStringBuilder(connectionString);

        var nomeBancoDeDados = construtorBancoDeDados.InitialCatalog;

        construtorBancoDeDados.Remove("Database");

        using var conexao = new SqlConnection(construtorBancoDeDados.ConnectionString);

        var parametros = new DynamicParameters();
        parametros.Add("name", nomeBancoDeDados);

        var registros = conexao.Query("SELECT * FROM sys.databases WHERE name = @name", parametros);

        if (!registros.Any())
        {
            using var commandBuilder = new SqlCommandBuilder();
            var nomeBancoSeguro = commandBuilder.QuoteIdentifier(nomeBancoDeDados);
            conexao.Execute($"CREATE DATABASE {nomeBancoSeguro}");
        }
    }

    private static void MigrarBancoDeDados(IServiceProvider serviceProvider)
    {
        var executor = serviceProvider.GetRequiredService<IMigrationRunner>();

        executor.ListMigrations();

        executor.MigrateUp();
    }
}