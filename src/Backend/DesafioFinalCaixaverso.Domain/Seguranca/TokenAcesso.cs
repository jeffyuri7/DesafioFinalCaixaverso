using System;

namespace DesafioFinalCaixaverso.Dominio.Seguranca;

public readonly record struct TokenAcesso(string Valor, DateTime ExpiraEm);
