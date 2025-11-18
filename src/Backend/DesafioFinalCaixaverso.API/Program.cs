using DesafioFinalCaixaverso.API.Filters;
using DesafioFinalCaixaverso.API.Middlewares;
using DesafioFinalCaixaverso.Aplicacao;
using DesafioFinalCaixaverso.Infraestrutura;
using DesafioFinalCaixaverso.Infraestrutura.Extensoes;
using DesafioFinalCaixaverso.Infraestrutura.Migracoes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.Filters.Add(typeof(FiltroDeExcecao)));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
