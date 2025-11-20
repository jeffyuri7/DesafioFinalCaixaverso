using System;

namespace DesafioFinalCaixaverso.Dominio.Seguranca;

public interface IGeradorTokenAcesso
{
    TokenAcesso Gerar(Guid clienteId);
}
