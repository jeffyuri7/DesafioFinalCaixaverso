using DesafioFinalCaixaverso.API.Filters;
using DesafioFinalCaixaverso.API.Middlewares;
using DesafioFinalCaixaverso.Aplicacao;
using DesafioFinalCaixaverso.Infraestrutura;
using DesafioFinalCaixaverso.Infraestrutura.Configuracoes;
using DesafioFinalCaixaverso.Infraestrutura.Extensoes;
using DesafioFinalCaixaverso.Infraestrutura.Migracoes;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.Filters.Add(typeof(FiltroDeExcecao)));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Desafio Final Caixaverso API",
        Version = "v1",
        Description = "Endpoints de simulação de investimentos, consultas históricas e telemetria."
    });
    options.EnableAnnotations();
});


builder.Services.Configure<HashSenhaConfiguracao>(builder.Configuration.GetSection(HashSenhaConfiguracao.Secao));

builder.Services.RegistrarAplicacao();
builder.Services.RegistrarInfraestrutura(builder.Configuration, builder.Environment);

builder.Services.AddRouting(opcoes => opcoes.LowercaseUrls = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<RegistroTelemetriaMiddleware>();

app.MapControllers();

MigrarBancoDeDados();

await app.RunAsync();

void MigrarBancoDeDados()
{
    if (builder.Environment.IsEnvironment("Testing"))
        return;

    var connectionString = builder.Configuration.ConnectionString();

    var servicoEscopo = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

    BancoDeDadosMigracao.Migracao(connectionString, servicoEscopo.ServiceProvider);
}

public partial class Program
{
    protected Program() { }
}
