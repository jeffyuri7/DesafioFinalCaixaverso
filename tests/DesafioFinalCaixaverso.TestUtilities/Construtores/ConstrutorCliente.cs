using System;
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
            Password = "hash::senha",
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
}
