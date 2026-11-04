<#
    Publica Convertidor como UN SOLO .exe autocontenido (no necesita .NET instalado).

    Ejemplos
      .\publicar.ps1
      .\publicar.ps1 -Destino "$env:USERPROFILE\Desktop"

    Sin -Destino solo deja el .exe en publish\. Con -Destino además lo copia allí.
    Para usarlo arrastrando: deja el .exe (o un acceso directo) en el escritorio y suelta
    los archivos encima.
#>

[CmdletBinding()]
param(
    [string] $Destino = "",
    [ValidateSet("win-x64", "win-x86")]
    [string] $Plataforma = "win-x64"
)

$ErrorActionPreference = "Stop"

$raiz = Split-Path -Parent $MyInvocation.MyCommand.Path
$proyecto = Join-Path $raiz "src\Convertidor\Convertidor.csproj"
$salida = Join-Path $raiz "publish"

Write-Host "Publicando Release / $Plataforma ..." -ForegroundColor Cyan
if (Test-Path $salida) { Remove-Item $salida -Recurse -Force }

dotnet publish $proyecto -c Release -r $Plataforma -o $salida --nologo
if ($LASTEXITCODE -ne 0) {
    Write-Host "La publicación ha fallado." -ForegroundColor Red
    exit 1
}

$exe = Join-Path $salida "Convertidor.exe"
$mb = [math]::Round((Get-Item $exe).Length / 1MB)
Write-Host ""
Write-Host "Publicado: $exe ($mb MB)" -ForegroundColor Green

if ($Destino) {
    Copy-Item $exe $Destino -Force
    Write-Host "Copiado a $Destino" -ForegroundColor Green
}
