# DesafioFinalCaixaverso

## Requisitos
- .NET 8 SDK
- Docker Desktop
- PowerShell (para rodar o seed)

## Build e testes
```powershell
cd source
dotnet restore DesafioFinalCaixaverso.slnx
dotnet build DesafioFinalCaixaverso.slnx
dotnet test DesafioFinalCaixaverso.slnx
```

## Subindo com Docker Compose
1. Copie o arquivo de exemplo e defina uma senha forte para o SQL Server:
	```powershell
	cd source
	copy .env.example .env
	# edite o arquivo e altere CAIXAVERSO_SQL_PASSWORD
	```
2. Suba os containers normalmente:
	```powershell
	docker compose up -d --build
	```
A API estará disponível em `http://localhost:8080/swagger`.

### Endpoints principais
- `POST v1/investimentos/simular-investimento` — calcula e persiste uma nova simulação (requer Bearer Token).
- `GET v1/investimentos/simulacoes` — lista o histórico completo de simulações ordenado pela data.
- `GET v1/investimentos/simulacoes/por-produto-dia` — agrega volume diário por produto para dashboards.
- `GET v1/investimentos/{clienteId}` — retorna apenas os investimentos do cliente autenticado.
- `GET v1/telemetria` — exibe consumo dos serviços e período corrente.
- `GET v1/perfil-risco/{clienteId}` — resumo textual do perfil com descrição amigável.
- `GET v1/perfil-risco-completo/{clienteId}` — retorna os detalhes usados na classificação e persiste o histórico.
- `GET v1/produtos-recomendados/{perfil}` — recomenda produtos compatíveis com o perfil informado.
- `POST v1/login` — autentica o cliente e gera o token JWT utilizado nos demais endpoints protegidos.
- `GET v1/clientes` — consulta os clientes cadastrados (apenas rotas administradas).
- `GET v1/clientes/{clienteId}` — consulta detalhada do cliente.
- `POST v1/clientes` — cadastra um novo cliente aplicando hash seguro.
- `PUT v1/clientes/{clienteId}` — atualiza dados básicos e senha opcionalmente.
- `DELETE v1/clientes/{clienteId}` — remove o cliente e relações.
- `POST v1/clientes/{clienteId}/questionario` — registra ou atualiza o questionário suitability.

## Como responder o questionário do investidor
O endpoint `POST v1/clientes/{clienteId}/questionario` aceita um corpo JSON com enumerações representadas por inteiros. Utilize a tabela abaixo ao preencher os campos enumerados:

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

- `horizonteMeses`: inteiro ≥ 1 (meses planejados para manter o investimento).
- `rendaMensal` e `patrimonioTotal`: valores decimais em reais (aceitam duas casas).
- `toleranciaPerdaPercentual`: decimal entre 0 e 100 representando a perda máxima aceitável.
- `fonteRendaEstavel`: booleano (`true`/`false`).

Exemplo de payload válido:

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

Essas descrições também estão visíveis no Swagger UI para facilitar o preenchimento manual.

## Sistema de análise de perfil de risco
- O motor combina **questionário formal** do investidor (liquidez, horizonte, tolerância a perdas, conhecimento, objetivo e situação financeira) com **dados comportamentais** das simulações realizadas (volume, frequência, rentabilidade média e liquidez dos produtos).
- As respostas do questionário são obrigatórias conforme as diretrizes da **ANBIMA**: sem o formulário válido o cliente permanece como “Não classificado” e nenhuma recomendação é liberada.
- Cada dimensão gera notas ponderadas que formam uma pontuação final (0 a 100). A classificação segue as faixas previstas no cálculo:
	- **≤ 40 pontos** → Perfil Conservador
	- **41 a 70 pontos** → Perfil Moderado
	- **> 70 pontos** → Perfil Agressivo
- O histórico salvo em `ClientePerfis` mantém pontuações comportamental e do questionário, método de cálculo e observações para auditoria.

## Seed de dados
```powershell
./seed-data.ps1
```
O script insere dados básicos no banco já criado pelas migrations.

> Use o parâmetro opcional `-HashSecret` caso altere a chave configurada em `Seguranca:HashSenha:Chave` para manter os hashes das senhas em sincronia.

## Parar containers
```powershell
docker compose down
docker compose down -v # remove volume de dados
```