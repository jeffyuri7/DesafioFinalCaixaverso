using System.Text;
using DesafioFinalCaixaverso.API.Filters;
using DesafioFinalCaixaverso.API.Middlewares;
using DesafioFinalCaixaverso.Aplicacao;
using DesafioFinalCaixaverso.Infraestrutura;
using DesafioFinalCaixaverso.Infraestrutura.Configuracoes;
using DesafioFinalCaixaverso.Infraestrutura.Extensoes;
using DesafioFinalCaixaverso.Infraestrutura.Migracoes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
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
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Insira apenas o token JWT.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    options.MapType<Guid>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "uuid",
        Example = new OpenApiString(string.Empty)
    });
    options.MapType<Guid?>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "uuid",
        Nullable = true,
        Example = new OpenApiString(string.Empty)
    });
});


builder.Services.Configure<HashSenhaConfiguracao>(builder.Configuration.GetSection(HashSenhaConfiguracao.Secao));
builder.Services.Configure<JwtTokenConfiguracao>(builder.Configuration.GetSection(JwtTokenConfiguracao.Secao));

var jwtConfiguracao = builder.Configuration
    .GetSection(JwtTokenConfiguracao.Secao)
    .Get<JwtTokenConfiguracao>()
    ?? throw new InvalidOperationException("Configuração de JWT não encontrada. Defina Seguranca:Jwt no appsettings.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfiguracao.Emissor,
        ValidAudience = jwtConfiguracao.Audiencia,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguracao.Chave)),
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.RegistrarAplicacao();
builder.Services.RegistrarInfraestrutura(builder.Configuration, builder.Environment);

builder.Services.AddRouting(opcoes => opcoes.LowercaseUrls = true);

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RegistroTelemetriaMiddleware>();

app.MapControllers();

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    AllowCachingResponses = false,
    ResultStatusCodes =
    {
        [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy] = StatusCodes.Status200OK,
        [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

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
