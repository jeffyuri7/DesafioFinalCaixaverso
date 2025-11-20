using System;
using System.Text.RegularExpressions;
using Dapper;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioFinalCaixaverso.Infraestrutura.Migracoes;

public static class BancoDeDadosMigracao
{
    private static readonly Regex NomeBancoPermitidoRegex = new("^[A-Za-z0-9_]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static void Migracao(string connectionString, IServiceProvider serviceProvider)
    {
        VerificarBancoDeDadosCriado(connectionString);

        MigrarBancoDeDados(serviceProvider);
    }

    private static void VerificarBancoDeDadosCriado(string connectionString)
    {
        var construtorBancoDeDados = new SqlConnectionStringBuilder(connectionString);

    var nomeBancoDeDados = SanitizarNomeBanco(construtorBancoDeDados.InitialCatalog);

        construtorBancoDeDados.Remove("Database");

        using var conexao = new SqlConnection(construtorBancoDeDados.ConnectionString);

        var parametros = new DynamicParameters();
        parametros.Add("name", nomeBancoDeDados);

        var registros = conexao.Query("SELECT * FROM sys.databases WHERE name = @name", parametros);

        if (!registros.Any())
        {
            var nomeEscapado = ObterNomeEscapado(conexao, nomeBancoDeDados);
            conexao.Execute($"CREATE DATABASE {nomeEscapado}");
        }
    }

    private static void MigrarBancoDeDados(IServiceProvider serviceProvider)
    {
        var executor = serviceProvider.GetRequiredService<IMigrationRunner>();

        executor.ListMigrations();

        executor.MigrateUp();
    }

    private static string SanitizarNomeBanco(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do banco de dados inválido.", nameof(nome));

        if (!NomeBancoPermitidoRegex.IsMatch(nome))
            throw new ArgumentException("Nome do banco de dados contém caracteres inválidos. Use apenas letras, números ou underscore.", nameof(nome));

        return nome;
    }

    private static string ObterNomeEscapado(SqlConnection conexao, string nome)
    {
        return conexao.QuerySingle<string>("SELECT QUOTENAME(@nome)", new { nome });
    }
}