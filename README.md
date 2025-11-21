# DesafioFinalCaixaverso

Plataforma completa para simulação de investimentos, classificação de perfil de risco (compliance ANBIMA) e recomendação de produtos Caixa. O projeto está dividido em camadas (`API`, `Application`, `Domain`, `Infrastructure` e `Communications`) mais a suíte de testes.

## ⚙️ Requisitos

- .NET 8 SDK
- Docker Desktop (para executar toda a stack)
- PowerShell 5+ (scripts de seed/local setup)

## 🚀 Rodando localmente

### Build e testes rápidos

```powershell
cd source
dotnet restore DesafioFinalCaixaverso.slnx
dotnet build DesafioFinalCaixaverso.slnx
dotnet test DesafioFinalCaixaverso.slnx
```

### Subindo com Docker Compose

1. Configure variáveis sensíveis:

	```powershell
	cd source
	copy .env.example .env
	# edite o arquivo e altere CAIXAVERSO_SQL_PASSWORD
	```

2. Start da stack (API + SQL Server):

	```powershell
	docker compose up -d --build
	```

3. A Swagger UI sobe em `http://localhost:8080/swagger`.

4. Para encerrar:

	```powershell
	docker compose down
	docker compose down -v # remove volume de dados
	```

### Seeds

```powershell
./seed-data.ps1        # clientes, produtos, perfis
./seed-produtos.ps1    # catálogo extendido opcional
```

Use `-HashSecret` nos scripts se personalizar `Seguranca:HashSenha:Chave` no `appsettings` para manter os hashes consistentes.

## 🔐 Autenticação e segurança

- `POST v1/login` retorna JWT utilizado nas rotas protegidas (`Authorization: Bearer <token>`).
- Senhas são persistidas com hash + salt e o token inclui `ClienteId` e perfil.
- Algumas rotas (questionário, simulações do cliente) verificam coerência entre token e `clienteId` informado.

## 🧠 Motor de perfil de risco

- **Questionário suitability**: liquidez, horizonte, tolerância a perda, objetivo, conhecimento e situação financeira. Sem questionário válido o cliente permanece “Não classificado”.
- **Dimensão comportamental**: avalia últimas simulações (volume, frequência, rentabilidade média e liquidez média dos produtos). A lógica compartilha o mesmo algoritmo (`PerfilPontuacaoHelper`).
- **Faixas de pontuação**:
  - ≤ 40 → Conservador
  - 41–70 → Moderado
  - > 70 → Agressivo
- `GET v1/perfil-risco/{clienteId}` entrega resumo enxuto; `GET v1/perfil-risco-completo/{clienteId}` retorna dados de cálculo e histórico.

## 📡 Endpoints principais

- `POST v1/investimentos/simular-investimento` — valida cliente, encontra produtos compatíveis e retorna `{ produtoValidado, resultadoSimulacao, dataSimulacao }`.
- `GET v1/investimentos/simulacoes` — histórico completo.
- `GET v1/investimentos/simulacoes/por-produto-dia` — métricas para dashboards.
- `GET v1/investimentos/{clienteId}` — lista compacta `{ id, tipo, valor, rentabilidade, data }` do cliente autenticado.
- `GET v1/produtos-recomendados/{perfil}` — responde apenas `{ id, nome, tipo, rentabilidade, risco }`.
- `GET v1/telemetria` — uso de serviços externos (OpenAI, Service Bus etc.).
- `POST v1/clientes/{clienteId}/questionario` — atualiza o suitability obrigatório.
- `POST v1/clientes` / `PUT v1/clientes/{id}` — CRUD de clientes com hash de senha.

> Consulte `next-steps` e `instrucoes.md` para backlog adicional de endpoints.

## 📝 Questionário do investidor

O payload usa enums numéricos; tabela de apoio:

| Campo | Valor | Significado |
| --- | --- | --- |
| `preferenciaLiquidez` | 0 | Alta |
|  | 1 | Média |
|  | 2 | Baixa |
| `objetivoInvestimento` | 0 | Preservação |
|  | 1 | Renda |
|  | 2 | Equilíbrio |
|  | 3 | Crescimento |
| `nivelConhecimento` | 0 | Iniciante |
|  | 1 | Intermediário |
|  | 2 | Avançado |

Outros campos:

- `horizonteMeses` ≥ 1
- `rendaMensal`, `patrimonioTotal`: decimal (duas casas)
- `toleranciaPerdaPercentual`: decimal 0–100
- `fonteRendaEstavel`: booleano

```json
{
	"preferenciaLiquidez": 0,
	"objetivoInvestimento": 3,
	"nivelConhecimento": 1,
	"horizonteMeses": 24,
	"rendaMensal": 8000.00,
	"patrimonioTotal": 120000.00,
	"toleranciaPerdaPercentual": 15.5,
	"fonteRendaEstavel": true
}
```

As mesmas descrições aparecem no Swagger para facilitar testes manuais.

## 🧱 Arquitetura e tecnologias

- ASP.NET Core 8 + Mapster + FluentValidation
- EF Core + SQL Server (migrations via FluentMigrator)
- Camada de infraestrutura com Service Bus, OpenAI, Blob Storage (mockados nos testes)
- Testes: xUnit + Shouldly + WebApplicationFactory (integração)
- Pipelines: `release-pipeline.yml` e `docker-compose.yml` na raiz

## ✅ Qualidade

- `dotnet test DesafioFinalCaixaverso.slnx` cobre unitários, validators e integração.
- SonarCloud acompanha smells (Dockerfile, SQL injection, payloads) — ajustes recentes já atendem aos alertas.
- `next-steps` documenta melhorias futuras (telemetria, dashboards, ajuste de payloads).

## 🆘 Troubleshooting rápido

- **Login falhou?** Garanta que rodou `seed-data.ps1` para criar usuário demo e revise `Seguranca:Jwt`.
- **Erro de acesso ao SQL?** Confirme `CAIXAVERSO_SQL_PASSWORD` e reinicie `docker compose` limpando volumes.
- **JWT expirado**: tokens duram 30 min — refaça o login antes de chamar endpoints protegidos.