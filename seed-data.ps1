param(
    [string]$SqlContainerName = "caixaverso-sqldb",
    [string]$Database = "CaixaVersoDB",
    [string]$SaPassword = "Caixaverso@2025",
    [string]$HashSecret = "DesafioFinalCaixaverso@2025!"
)

function Get-PasswordHash {
    param(
        [string]$Senha,
        [string]$Segredo
    )

    $texto = $Senha + $Segredo
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($texto)
    $hashBytes = [System.Security.Cryptography.SHA256]::Create().ComputeHash($bytes)
    ($hashBytes | ForEach-Object { $_.ToString("X2") }) -join ''
}

function Invoke-ContainerSqlCommand {
    param(
        [string]$DatabaseName = "master",
        [string]$Command
    )

    docker exec $SqlContainerName /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SaPassword -d $DatabaseName -b -Q $Command | Out-Null
}

$containerId = docker ps --filter "name=$SqlContainerName" --format "{{.ID}}"
if (-not $containerId) {
    throw "O container '$SqlContainerName' não está em execução. Execute 'docker compose up -d sqldb' antes de rodar o seed."
}


$senhaPadrao = 'Senha@123'
$senhaHash = Get-PasswordHash -Senha $senhaPadrao -Segredo $HashSecret

$seedScript = @"
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM Cliente WHERE Email = 'cliente.teste@caixa.com.br')
BEGIN
    INSERT INTO Cliente (Id, Nome, Email, Password, DataCriacao)
    VALUES ('6F2F7DE1-1F77-4F1C-AE2E-5C85E1A0D1F1', 'Cliente Teste', 'cliente.teste@caixa.com.br', '$senhaHash', SYSUTCDATETIME());
END;

IF NOT EXISTS (SELECT 1 FROM Cliente WHERE Email = 'cliente.vip@caixa.com.br')
BEGIN
    INSERT INTO Cliente (Id, Nome, Email, Password, DataCriacao)
    VALUES ('A083F1C4-30E9-4E95-AC71-FACB4D7E0A5E', 'Cliente VIP', 'cliente.vip@caixa.com.br', '$senhaHash', SYSUTCDATETIME());
END;

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Tipo = 'CDB')
BEGIN
    INSERT INTO Produto (Id, Nome, Tipo, Rentabilidade, Risco, LiquidezDias, MinimoInvestimento, PrazoMinimoMeses, PrazoMaximoMeses, Ativo)
    VALUES ('F9C2E5DD-44FE-45EB-A98C-6C32C83DE022', 'CDB Pós-Fixado', 'CDB', 0.13, 0, 30, 1000, 6, 48, 1);
END;

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Tipo = 'LCI')
BEGIN
    INSERT INTO Produto (Id, Nome, Tipo, Rentabilidade, Risco, LiquidezDias, MinimoInvestimento, PrazoMinimoMeses, PrazoMaximoMeses, Ativo)
    VALUES ('C7A7D7A5-7802-4A42-9A6C-41D92A7F4CAD', 'LCI Residencial', 'LCI', 0.11, 0, 90, 5000, 12, 60, 1);
END;

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Tipo = 'Fundo Multimercado')
BEGIN
    INSERT INTO Produto (Id, Nome, Tipo, Rentabilidade, Risco, LiquidezDias, MinimoInvestimento, PrazoMinimoMeses, PrazoMaximoMeses, Ativo)
    VALUES ('04C7D1C7-9B00-4D89-8425-7CF4ABE7D3DA', 'Fundo Multimercado XPTO', 'Fundo Multimercado', 0.18, 2, 15, 10000, 3, 36, 1);
END;
"@

$tempFile = New-TemporaryFile
Set-Content -LiteralPath $tempFile.FullName -Value $seedScript -Encoding UTF8

docker cp $tempFile.FullName "$SqlContainerName:/tmp/seed.sql" | Out-Null

docker exec $SqlContainerName /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SaPassword -d $Database -b -i /tmp/seed.sql

docker exec $SqlContainerName rm /tmp/seed.sql | Out-Null
Remove-Item $tempFile.FullName -Force

Write-Host "Seed concluído com sucesso para o banco '$Database'."
