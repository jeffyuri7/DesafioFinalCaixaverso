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
```powershell
docker compose up -d --build
```
A API estará disponível em `http://localhost:8080/swagger`.

### Endpoints 
- `POST v1/investimentos/simular-investimento`
- `GET v1/investimentos/simulacoes`
- `GET v1/investimentos/simulacoes/por-produto-dia`
- `GET v1/telemetria`
- `POST v1/clientes`
- `POST v1/clientes/{clienteId}/questionario`

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