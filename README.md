# DesafioFinalCaixaverso

Plataforma completa para simulação de investimentos, classificação de perfil de risco (compliance ANBIMA) e recomendação de produtos Caixa. O projeto está dividido em camadas com os respectivos projetos: API (`DesafioFinalCaixaverso.API`), Aplicação (`DesafioFinalCaixaverso.Aplicacao`), Domínio (`DesafioFinalCaixaverso.Dominio`), Infraestrutura (`DesafioFinalCaixaverso.Infraestrutura`) e Comunicações (`DesafioFinalCaixaverso.Comunicacoes`), além da suíte de testes.

## ⚙️ Requisitos

- .NET 8 SDK
- Docker Desktop (para executar toda a stack)
- PowerShell 5+ (scripts de seed/local setup)

## 🚀 Execução do Projeto 

```powershell
git clone https://github.com/jeffyuri7/DesafioFinalCaixaverso.git
cd DesafioFinalCaixaverso
docker compose up -d --build
```

Swagger UI: `http://localhost:8080/swagger`

### Seeds

```powershell
./seed-produtos.ps1    # catálogo extendido aplicado ao banco de dados automaticamente pelo docker compose.
```

Use `-HashSecret` nos scripts se personalizar `Seguranca:HashSenha:Chave` no `appsettings` para manter os hashes consistentes.

## 🔐 Autenticação e segurança

- `POST v1/login` retorna JWT utilizado nas rotas protegidas (`Authorization: Bearer <token>`).
- Senhas são persistidas com hash + salt e o token inclui `ClienteId` e perfil.
- Algumas rotas (questionário, simulações do cliente) verificam coerência entre token e `clienteId` informado.

## 🔄 Fluxo de teste da API

1. **Cadastrar cliente** (`POST v1/clientes`) e guardar o `clienteId` retornado.
2. **Autenticar** (`POST v1/login`) e copiar apenas o token JWT (não inclua `Bearer`).
3. Na Swagger UI, clique em **Authorize** e cole somente o token; todos os endpoints protegidos ficarão disponíveis.
4. **Registrar o questionário** (`POST v1/clientes/{clienteId}/questionario`). É obrigatório pelas normas ANBIMA e libera recomendações.
5. **Consultar perfil inicial** em `GET v1/perfil-risco-inicial/{clienteId}` — usa dados do questionário imediatamente após o envio.
6. **Realizar simulações** (`POST v1/investimentos/simular-investimento`). Ao menos uma simulação é necessária para alimentar o perfil dinâmico.
7. **Consultar perfil dinâmico** em `GET v1/perfil-risco/{clienteId}` — só apresentará dados após a primeira simulação e será recalculado a cada nova simulação.

A documentação do endpoint de questionário descreve todos os campos obrigatórios; este README também mantém a tabela de apoio na seção “Questionário do investidor”.

## 🧠 Motor de perfil de risco

- **Questionário suitability**: liquidez, horizonte, tolerância a perda, objetivo, conhecimento e situação financeira. Sem questionário válido o cliente permanece “Não classificado”.
- **Dimensão comportamental**: avalia últimas simulações (volume, frequência, rentabilidade média e liquidez média dos produtos). A lógica compartilha o mesmo algoritmo (`PerfilPontuacaoHelper`).
- **Faixas de pontuação**:
  - ≤ 40 → Conservador
  - 41–70 → Moderado
  - > 70 → Agressivo
- `GET v1/perfil-risco/{clienteId}` entrega perfil dinâmico; `GET v1/perfil-risco-inicial/{clienteId}` retorna dados de cálculo do questionário.


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
- Camada de infraestrutura (mockados nos testes)
- Testes: xUnit + Shouldly + WebApplicationFactory (integração)
- Pipelines: `release-pipeline.yml` e `docker-compose.yml` na raiz

## ✅ Qualidade

- `dotnet test DesafioFinalCaixaverso.slnx` cobre unitários, validators e integração.
- SonarCloud: O código foi validado pelo SonarCloud e não possui nenhuma vulnerabilidade crítica, más práticas ou código repetido. Caso o examinador utilize o SonarCloud para reexaminar o código, favor remover o arquivo ".env" que foi adicionado ao repositório apenas para tornar a execução teste possível. Solicito que remova o arquivo antes de enviar para o Sonar para evitar que ele aponte falha de segurança.

## 🆘 Troubleshooting rápido

- **Erro de acesso ao SQL?** Confirme `CAIXAVERSO_SQL_PASSWORD` no arquivo ".env" e reinicie `docker compose` limpando volumes.
- **JWT expirado**: tokens duram 30 min — refaça o login antes de chamar endpoints protegidos.
- **Nome de container já usado**: se receber erro informando que um container com o mesmo nome já existe, finalize o container antigo (`docker ps -a` + `docker rm -f <nome>`) ou ajuste o nome no `docker-compose.yml` antes de subir novamente.
