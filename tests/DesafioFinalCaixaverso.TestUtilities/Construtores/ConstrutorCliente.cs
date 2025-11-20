using System;
using System.Security.Cryptography;
using Bogus;
using DesafioFinalCaixaverso.Dominio.Entidades;

namespace DesafioFinalCaixaverso.TestUtilities.Construtores;

public class ConstrutorCliente
{
    private readonly Cliente _cliente;

    public ConstrutorCliente()
    {
        var faker = new Faker("pt_BR");
        _cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = faker.Person.FullName,
            Email = faker.Internet.Email().ToLowerInvariant(),
            Password = $"hash::{GerarHashAleatorio()}",
            DataCriacao = DateTime.UtcNow
        };
    }

    public ConstrutorCliente ComEmail(string email)
    {
        _cliente.Email = email;
        return this;
    }

    public ConstrutorCliente ComSenhaHash(string hash)
    {
        _cliente.Password = hash;
        return this;
    }

    public Cliente Construir() => _cliente;

    private static string GerarHashAleatorio()
    {
        Span<byte> buffer = stackalloc byte[32];
        RandomNumberGenerator.Fill(buffer);
        return Convert.ToHexString(buffer);
    }
}
