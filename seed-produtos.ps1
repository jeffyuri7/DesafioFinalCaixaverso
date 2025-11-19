<#[
SEED SCRIPT: seed-produtos.ps1

Uso (ex.: em CI / docker-compose init):
powershell -ExecutionPolicy Bypass -File .\seed-produtos.ps1 \
    -SqlServerHost "localhost" -SqlServerPort 1433 -SqlAdminUser "sa" -SqlAdminPassword "Caixaverso@2025" -DatabaseName "CaixaVersoDB"

Parâmetros (todos opcionais; também podem ser preenchidos via variáveis de ambiente):
 -SqlServerHost     = $env:SQL_SERVER_HOST   | Default: "localhost"
 -SqlServerPort     = $env:SQL_SERVER_PORT   | Default: 1433
 -SqlAdminUser      = $env:SQL_ADMIN_USER    | Default: "sa"
 -SqlAdminPassword  = $env:SQL_ADMIN_PW      | Default: "Caixaverso@2025"
 -DatabaseName      = $env:SQL_DB_NAME       | Default: "CaixaVersoDB"
 -MaxRetries        = tentativas para aguardar o banco/tabela (default 60)
 -RetryDelaySeconds = intervalo entre tentativas em segundos (default 5)
#>

param(
    [string]$SqlServerHost    = $(if ($env:SQL_SERVER_HOST) { $env:SQL_SERVER_HOST } else { "localhost" }),
    [int]   $SqlServerPort    = $(if ($env:SQL_SERVER_PORT) { [int]$env:SQL_SERVER_PORT } else { 1433 }),
    [string]$SqlAdminUser     = $(if ($env:SQL_ADMIN_USER) { $env:SQL_ADMIN_USER } else { "sa" }),
    [string]$SqlAdminPassword = $(if ($env:SQL_ADMIN_PW) { $env:SQL_ADMIN_PW } else { "Caixaverso@2025" }),
    [string]$DatabaseName     = $(if ($env:SQL_DB_NAME) { $env:SQL_DB_NAME } else { "CaixaVersoDB" }),
    [int]   $MaxRetries       = 60,
    [int]   $RetryDelaySeconds = 5
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Data

$SqlServerInstance = "$SqlServerHost,$SqlServerPort"

$builder = [System.Data.SqlClient.SqlConnectionStringBuilder]::new()
$builder.DataSource = $SqlServerInstance
$builder.InitialCatalog = $DatabaseName
$builder.UserID = $SqlAdminUser
$builder.Password = $SqlAdminPassword
$builder.TrustServerCertificate = $true
$builder.Encrypt = $false

$TargetConnectionString = $builder.ConnectionString
$masterBuilder = [System.Data.SqlClient.SqlConnectionStringBuilder]::new($builder.ConnectionString)
$masterBuilder.InitialCatalog = "master"
$MasterConnectionString = $masterBuilder.ConnectionString

Write-Host "=== Seed Produtos CaixaVerso ==="
Write-Host "Servidor: $SqlServerInstance | Banco: $DatabaseName | Usuário: $SqlAdminUser"

function Invoke-DbCommand {
    param(
        [string]$Sql,
        [string]$ConnectionString = $TargetConnectionString,
        [switch]$Scalar
    )

    $connection = [System.Data.SqlClient.SqlConnection]::new($ConnectionString)
    $command = $connection.CreateCommand()
    $command.CommandTimeout = 60
    $command.CommandText = $Sql

    try {
        $connection.Open()
        if ($Scalar) {
            return $command.ExecuteScalar()
        }
        else {
            $command.ExecuteNonQuery() | Out-Null
        }
    }
    catch {
        Write-Error "Falha ao executar SQL: $($_.Exception.Message)`nComando: $Sql"
        throw
    }
    finally {
        if ($command) { $command.Dispose() }
        if ($connection.State -eq 'Open') { $connection.Close() }
        $connection.Dispose()
    }
}

function Wait-ForDatabase {
    param(
        [int]$RetryCount,
        [int]$DelaySeconds
    )

    for ($attempt = 1; $attempt -le $RetryCount; $attempt++) {
        try {
            $dbId = Invoke-DbCommand -Sql "SELECT DB_ID(N'$DatabaseName')" -ConnectionString $MasterConnectionString -Scalar
            if ($dbId) {
                Write-Host "Banco '$DatabaseName' disponível (tentativa $attempt)."
                return
            }
        }
        catch {
            Write-Warning "Tentativa ${attempt}: aguardando banco. Motivo: $($_.Exception.Message)"
        }
        Start-Sleep -Seconds $DelaySeconds
    }

    throw "Banco '$DatabaseName' não ficou disponível após $RetryCount tentativas."
}

function Wait-ForProdutosTable {
    param(
        [int]$RetryCount,
        [int]$DelaySeconds
    )

    $sql = "SELECT COUNT(1) FROM sys.tables WHERE name = 'Produtos' AND schema_id = SCHEMA_ID('dbo');"

    for ($attempt = 1; $attempt -le $RetryCount; $attempt++) {
        try {
            $exists = Invoke-DbCommand -Sql $sql -Scalar
            if ($exists -gt 0) {
                Write-Host "Tabela dbo.Produtos localizada (tentativa $attempt)."
                return
            }
        }
        catch {
            Write-Warning "Tentativa ${attempt}: aguardando tabela dbo.Produtos. Motivo: $($_.Exception.Message)"
        }

        Start-Sleep -Seconds $DelaySeconds
    }

    throw "Tabela dbo.Produtos não encontrada após $RetryCount tentativas. Garanta que as migrações foram executadas."
}

Wait-ForDatabase -RetryCount $MaxRetries -DelaySeconds $RetryDelaySeconds
Wait-ForProdutosTable -RetryCount $MaxRetries -DelaySeconds $RetryDelaySeconds

$sqlSeed = @'
DECLARE @SeedProdutos TABLE(
    Id UNIQUEIDENTIFIER NOT NULL,
    Nome NVARCHAR(200) NOT NULL,
    Tipo NVARCHAR(100) NOT NULL,
    Rentabilidade DECIMAL(18,4) NOT NULL,
    Risco INT NOT NULL,
    LiquidezDias INT NOT NULL,
    MinimoInvestimento DECIMAL(18,2) NOT NULL,
    PrazoMinimoMeses INT NOT NULL,
    PrazoMaximoMeses INT NOT NULL,
    Ativo BIT NOT NULL
);

INSERT INTO @SeedProdutos (Id, Nome, Tipo, Rentabilidade, Risco, LiquidezDias, MinimoInvestimento, PrazoMinimoMeses, PrazoMaximoMeses, Ativo)
VALUES
    ('00000000-0000-0000-0000-000000000001','Poupança Caixa','Poupança',0.0350,0,0,0.01,0,0,1),
    ('00000000-0000-0000-0000-000000000002','CDB Caixa Flex Liquidez Diária','CDB',0.0400,0,0,1000.00,6,6,1),
    ('00000000-0000-0000-0000-000000000003','CDB Caixa Pré-Fixado 36 meses','CDB',0.1000,1,0,1000.00,36,36,1),
    ('00000000-0000-0000-0000-000000000004','CDB Caixa Pós-Fixado CDI 24 meses','CDB',0.0850,1,0,500.00,24,24,1),
    ('00000000-0000-0000-0000-000000000005','LCI Caixa 24 meses','LCI',0.0950,1,90,5000.00,24,24,1),
    ('00000000-0000-0000-0000-000000000006','LCA Caixa 36 meses','LCA',0.0920,1,90,5000.00,36,36,1),
    ('00000000-0000-0000-0000-000000000007','Tesouro Selic (via Caixa)','Tesouro Direto',0.0600,0,0,100.00,0,0,1),
    ('00000000-0000-0000-0000-000000000008','Tesouro IPCA+ 5 anos (via Caixa)','Tesouro Direto',0.0800,1,30,100.00,60,60,1),
    ('00000000-0000-0000-0000-000000000009','Fundo Caixa Renda Fixa Simples','Fundo (Renda Fixa)',0.0600,0,0,100.00,3,3,1),
    ('00000000-0000-0000-0000-000000000010','Fundo Caixa Renda Fixa Longo Prazo','Fundo (Renda Fixa)',0.0650,1,30,1000.00,12,12,1),
    ('00000000-0000-0000-0000-000000000011','Fundo Caixa Referenciado DI','Fundo Referenciado',0.0550,0,0,100.00,3,3,1),
    ('00000000-0000-0000-0000-000000000012','Fundo Caixa Referenciado IPCA','Fundo Referenciado',0.0700,1,30,1000.00,12,12,1),
    ('00000000-0000-0000-0000-000000000013','Fundo Caixa Multimercado Estratégico','Fundo (Multimercado)',0.0950,1,7,5000.00,12,12,1),
    ('00000000-0000-0000-0000-000000000014','Fundo Caixa Multimercado Macro','Fundo (Multimercado)',0.1200,2,30,10000.00,36,36,1),
    ('00000000-0000-0000-0000-000000000015','Fundo Caixa Multimercado Alocacao Macro','Fundo (Multimercado)',0.1100,2,30,8000.00,24,24,1),
    ('00000000-0000-0000-0000-000000000016','Fundo Caixa Acoes Brasil','Fundo (Ações)',0.1500,2,90,2000.00,60,60,1),
    ('00000000-0000-0000-0000-000000000017','Fundo Caixa Acoes Global','Fundo (Ações)',0.1300,2,90,2000.00,60,60,1),
    ('00000000-0000-0000-0000-000000000018','Fundo Caixa Cambial Moeda','Fundo (Cambial)',0.1200,2,30,5000.00,24,24,1),
    ('00000000-0000-0000-0000-000000000019','Fundo Caixa Imobiliario (FII)','Fundo Imobiliario',0.0850,1,30,1000.00,12,12,1),
    ('00000000-0000-0000-0000-000000000020','Oferta Pública Debênture Infra (Caixa)','Oferta Pública',0.1100,1,365,10000.00,60,60,1),
    ('00000000-0000-0000-0000-000000000021','Oferta Pública Nota Promissória Caixa','Oferta Pública',0.0900,0,90,5000.00,6,6,1),
    ('00000000-0000-0000-0000-000000000022','Operacao Compromissada Caixa Curto Prazo','Compromissada',0.0450,0,0,10000.00,1,1,1),
    ('00000000-0000-0000-0000-000000000023','Operacao Compromissada Caixa Longo Prazo','Compromissada',0.0550,1,30,5000.00,12,12,1),
    ('00000000-0000-0000-0000-000000000024','Previdencia Caixa VGBL Conservador','Previdencia',0.0500,0,0,100.00,60,60,1),
    ('00000000-0000-0000-0000-000000000025','Previdencia Caixa VGBL Moderado','Previdencia',0.0800,1,30,1000.00,120,120,1),
    ('00000000-0000-0000-0000-000000000026','COE Caixa Capital Protegido (Simulado)','COE',0.1000,1,365,5000.00,12,12,1),
    ('00000000-0000-0000-0000-000000000027','COE Caixa Alavancado Brasil (Simulado)','COE',0.1800,2,365,10000.00,36,36,1),
    ('00000000-0000-0000-0000-000000000028','LCI Caixa Longo Prazo 60 mes','LCI',0.0980,1,90,5000.00,60,60,1),
    ('00000000-0000-0000-0000-000000000029','LCA Caixa Longo Prazo 60 mes','LCA',0.0960,1,90,5000.00,60,60,1),
    ('00000000-0000-0000-0000-000000000030','Fundo Caixa Multimercado Juros e Moedas','Fundo (Multimercado)',0.0900,1,30,5000.00,12,12,1),
    ('00000000-0000-0000-0000-000000000031','ETF Caixa Brasil (simulado)','ETF',0.1400,2,2,1000.00,12,12,1),
    ('00000000-0000-0000-0000-000000000032','Titulo Corporativo Caixa (simulado) - curto','Titulo Corporativo',0.0650,0,30,2000.00,6,6,1);

;MERGE dbo.Produtos AS Target
USING @SeedProdutos AS Source
    ON Target.Id = Source.Id
WHEN MATCHED THEN
    UPDATE SET
        Nome = Source.Nome,
        Tipo = Source.Tipo,
        Rentabilidade = Source.Rentabilidade,
        Risco = Source.Risco,
        LiquidezDias = Source.LiquidezDias,
        MinimoInvestimento = Source.MinimoInvestimento,
        PrazoMinimoMeses = Source.PrazoMinimoMeses,
        PrazoMaximoMeses = Source.PrazoMaximoMeses,
        Ativo = Source.Ativo
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, Nome, Tipo, Rentabilidade, Risco, LiquidezDias, MinimoInvestimento, PrazoMinimoMeses, PrazoMaximoMeses, Ativo)
    VALUES (Source.Id, Source.Nome, Source.Tipo, Source.Rentabilidade, Source.Risco, Source.LiquidezDias, Source.MinimoInvestimento, Source.PrazoMinimoMeses, Source.PrazoMaximoMeses, Source.Ativo);

SELECT
    ProdutosSeed = (SELECT COUNT(1) FROM @SeedProdutos),
    ProdutosNaBase = (SELECT COUNT(1) FROM dbo.Produtos);
'@

Invoke-DbCommand -Sql $sqlSeed

$totalProdutos = Invoke-DbCommand -Sql "SELECT COUNT(1) FROM dbo.Produtos" -Scalar
Write-Host "Seed finalizado com sucesso. Total de produtos disponíveis: $totalProdutos"
