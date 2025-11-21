using System;
using System.Collections.Generic;
using DesafioFinalCaixaverso.Dominio.Entidades;

namespace DesafioFinalCaixaverso.Aplicacao.Servicos.Perfis;

public interface ICalculadoraPerfilDinamico
{
    ClientePerfilDinamico Calcular(Guid clienteId, IReadOnlyCollection<Simulacao> simulacoes);
}
