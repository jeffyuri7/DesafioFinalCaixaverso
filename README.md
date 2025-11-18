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

## Seed de dados
```powershell
./seed-data.ps1
```
O script insere dados básicos no banco já criado pelas migrations.

## Parar containers
```powershell
docker compose down
docker compose down -v # remove volume de dados
```