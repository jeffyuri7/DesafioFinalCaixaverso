using Microsoft.Extensions.Configuration;

namespace DesafioFinalCaixaverso.Infraestrutura.Extensoes;

public static class ExtensaoDeConfiguracao
{
    public static string ConnectionString(this IConfiguration configuration)
    {
        return configuration.GetConnectionString("CaixaversoDatabase") 
            ?? throw new InvalidOperationException("Connection string 'CaixaversoDatabase' not found.");
    }
}
