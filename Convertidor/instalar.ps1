<#
    Instala Convertidor en este equipo para usarlo como una aplicación más, sin Visual Studio.

    Qué hace
      1. Publica el .exe (un solo archivo, no necesita .NET instalado).
      2. Lo copia a la carpeta de instalación (por defecto, Escritorio\Erick\Convertidor).
      3. Crea dos accesos directos:
           - Menú Inicio: buscar "Convertidor" y abrir.
           - "Enviar a" del clic derecho del Explorador: seleccionas archivos > Enviar a > Convertidor.

    Para actualizar a una versión nueva basta con volver a lanzarlo.

    Ejemplos
      powershell -ExecutionPolicy Bypass -File .\instalar.ps1
      powershell -ExecutionPolicy Bypass -File .\instalar.ps1 -AccesoEscritorio
      powershell -ExecutionPolicy Bypass -File .\instalar.ps1 -SinPublicar     # usa el publish\ que ya hay
      powershell -ExecutionPolicy Bypass -File .\instalar.ps1 -Desinstalar

    Todo queda en tu usuario: no pide permisos de administrador ni toca el registro.
#>

[CmdletBinding()]
param(
    [string] $Destino = (Join-Path $env:USERPROFILE "Desktop\Erick\Convertidor"),
    [switch] $SinPublicar,
    [switch] $AccesoEscritorio,
    [switch] $Desinstalar
)

$ErrorActionPreference = "Stop"
$raiz = Split-Path -Parent $MyInvocation.MyCommand.Path
$exeDestino = Join-Path $Destino "Convertidor.exe"

$accesos = @(
    (Join-Path ([Environment]::GetFolderPath("Programs")) "Convertidor.lnk"),
    (Join-Path ([Environment]::GetFolderPath("SendTo")) "Convertidor.lnk")
)
$lnkEscritorio = Join-Path ([Environment]::GetFolderPath("Desktop")) "Convertidor.lnk"

function Cerrar-SiEstaAbierto {
    # Si se está usando, Windows no deja sobrescribir el .exe
    $abiertos = @(Get-Process Convertidor -ErrorAction SilentlyContinue |
        Where-Object { $_.Path -and ($_.Path -ieq $exeDestino) })
    if ($abiertos.Count -gt 0) {
        Write-Host "Convertidor está abierto. Ciérralo y vuelve a lanzar el script." -ForegroundColor Red
        exit 1
    }
}

# ------------------------------------------------------------------
# Desinstalar
# ------------------------------------------------------------------
if ($Desinstalar) {
    Cerrar-SiEstaAbierto
    foreach ($a in $accesos + $lnkEscritorio) {
        if (Test-Path $a) { Remove-Item $a -Force; Write-Host "Quitado: $a" }
    }
    if (Test-Path $exeDestino) { Remove-Item $exeDestino -Force; Write-Host "Quitado: $exeDestino" }
    if ((Test-Path $Destino) -and -not (Get-ChildItem $Destino -Force)) { Remove-Item $Destino -Force }
    Write-Host "Desinstalado." -ForegroundColor Green
    exit 0
}

# ------------------------------------------------------------------
# 1. Publicar
# ------------------------------------------------------------------
$publicado = Join-Path $raiz "publish\Convertidor.exe"
if (-not $SinPublicar) {
    & (Join-Path $raiz "publicar.ps1")
    if ($LASTEXITCODE -ne 0) { exit 1 }
}
if (-not (Test-Path $publicado)) {
    Write-Host "No está $publicado. Lánzalo sin -SinPublicar." -ForegroundColor Red
    exit 1
}

# ------------------------------------------------------------------
# 2. Copiar
# ------------------------------------------------------------------
Cerrar-SiEstaAbierto
New-Item -ItemType Directory -Force $Destino | Out-Null
Copy-Item $publicado $exeDestino -Force
$version = (Get-Item $exeDestino).VersionInfo.ProductVersion

# ------------------------------------------------------------------
# 3. Accesos directos
# ------------------------------------------------------------------
$shell = New-Object -ComObject WScript.Shell
function Crear-Acceso([string] $ruta) {
    $lnk = $shell.CreateShortcut($ruta)
    $lnk.TargetPath = $exeDestino
    $lnk.WorkingDirectory = $Destino
    $lnk.IconLocation = "$exeDestino,0"
    $lnk.Description = "Convertir archivos de un formato a otro"
    $lnk.Save()
    Write-Host "   $ruta"
}

Write-Host ""
Write-Host "Instalado Convertidor $version en $exeDestino" -ForegroundColor Green
Write-Host "Accesos directos:"
$accesos | ForEach-Object { Crear-Acceso $_ }
if ($AccesoEscritorio) { Crear-Acceso $lnkEscritorio }

Write-Host ""
Write-Host "Cómo usarlo:" -ForegroundColor Cyan
Write-Host "   - Menú Inicio > Convertidor"
Write-Host "   - Clic derecho sobre uno o varios archivos > Enviar a > Convertidor"
Write-Host "   - Soltar archivos encima de $exeDestino"
