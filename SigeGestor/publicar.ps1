<#
    Publica SigeGestor y, si se le da un destino, lo copia al servidor.

    NO HACE NADA POR SU CUENTA: sin -Destino solo deja la carpeta de publicación en local
    para poder revisarla antes de tocar el servidor compartido.

    Ejemplos
      .\publicar.ps1
      .\publicar.ps1 -Destino \\172.31.100.13\Total\SigeGestor
      .\publicar.ps1 -Destino \\172.31.100.13\Total\SigeGestor -Confirmar

    POR QUÉ AUTOCONTENIDO: se empaqueta el runtime de .NET dentro, así que las máquinas del
    equipo no necesitan tener instalado el .NET 8 Desktop Runtime. Es lo mismo que se hace hoy
    con ActualizaPrecios, que publica win-x86 autocontenido. El precio son unos 180 MB.

    LO QUE NUNCA SE PISA EN EL SERVIDOR:
      Config\Entornos.json   credenciales de los tres entornos
      Config\Empresas.json   bases de clientes, con credenciales
      Config\Usuarios.json   qué novedades ha visto cada uno
      Logs\                  el histórico de ejecuciones del equipo

    Los tres JSON ya no salen en la publicación (ver SigeGestor.App.csproj), pero la exclusión
    se repite aquí a propósito: si algún día alguien vuelve a incluirlos en el proyecto, esto
    sigue protegiendo al servidor.
#>

[CmdletBinding()]
param(
    # Carpeta del servidor. Sin esto, solo se publica en local.
    [string] $Destino = "",

    # Copiar sin preguntar. Sin esto se pide confirmación antes de escribir en el servidor.
    [switch] $Confirmar,

    [ValidateSet("win-x64", "win-x86")]
    [string] $Plataforma = "win-x64",

    [string] $Configuracion = "Release"
)

$ErrorActionPreference = "Stop"

$raiz = Split-Path -Parent $MyInvocation.MyCommand.Path
$proyecto = Join-Path $raiz "src\SigeGestor.App\SigeGestor.App.csproj"
$salida = Join-Path $raiz "publish\$Plataforma"

if (-not (Test-Path $proyecto)) {
    Write-Host "No encuentro el proyecto en $proyecto" -ForegroundColor Red
    exit 1
}

# ------------------------------------------------------------------
# 1. Publicar
# ------------------------------------------------------------------
Write-Host "Publicando $Configuracion / $Plataforma ..." -ForegroundColor Cyan

if (Test-Path $salida) { Remove-Item $salida -Recurse -Force }

dotnet publish $proyecto `
    -c $Configuracion `
    -r $Plataforma `
    --self-contained true `
    -o $salida `
    --nologo

if ($LASTEXITCODE -ne 0) {
    Write-Host "La publicación ha fallado." -ForegroundColor Red
    exit 1
}

# ------------------------------------------------------------------
# 2. Comprobar que no se ha colado ninguna credencial
# ------------------------------------------------------------------
$prohibidos = @("Entornos.json", "Empresas.json", "Usuarios.json")
$colados = @()

foreach ($nombre in $prohibidos) {
    $ruta = Join-Path $salida "Config\$nombre"
    if (Test-Path $ruta) { $colados += $nombre }
}

if ($colados.Count -gt 0) {
    Write-Host ""
    Write-Host "ALTO: la publicación lleva ficheros de configuración reales:" -ForegroundColor Red
    $colados | ForEach-Object { Write-Host "   Config\$_" -ForegroundColor Red }
    Write-Host "Se borran de la carpeta de publicación. Revisa el csproj: alguien los ha vuelto a incluir." -ForegroundColor Yellow
    $colados | ForEach-Object { Remove-Item (Join-Path $salida "Config\$_") -Force }
}

$sql = @(Get-ChildItem (Join-Path $salida "Consultas") -Filter *.sql -ErrorAction SilentlyContinue).Count
$mb = [math]::Round((Get-ChildItem $salida -Recurse -File | Measure-Object -Property Length -Sum).Sum / 1MB)

Write-Host ""
Write-Host "Publicado en $salida" -ForegroundColor Green
Write-Host "   $mb MB · $sql plantillas .sql · Config con solo las plantillas"

if ([string]::IsNullOrWhiteSpace($Destino)) {
    Write-Host ""
    Write-Host "No se ha indicado -Destino, así que no se ha tocado ningún servidor." -ForegroundColor Yellow
    Write-Host "Para copiar:  .\publicar.ps1 -Destino \\172.31.100.13\Total\SigeGestor"
    exit 0
}

# ------------------------------------------------------------------
# 3. Copiar al servidor
# ------------------------------------------------------------------
$primeraVez = -not (Test-Path (Join-Path $Destino "Config\Entornos.json"))

Write-Host ""
Write-Host "Destino: $Destino" -ForegroundColor Cyan

if ($primeraVez) {
    Write-Host "Es la primera vez: allí no hay Config\Entornos.json." -ForegroundColor Yellow
    Write-Host "Tras copiar habrá que crearlo a mano a partir de Entornos.ejemplo.json," -ForegroundColor Yellow
    Write-Host "o traer el EmpresasBD.json de ActualizaPrecios como Config\Empresas.json." -ForegroundColor Yellow
} else {
    Write-Host "Ya hay configuración allí: se conserva." -ForegroundColor Green
}

if (-not $Confirmar) {
    Write-Host ""
    Write-Host "Van a escribirse $mb MB en una carpeta que usa todo el equipo."
    $r = Read-Host "Escribe SI para continuar"
    if ($r -ne "SI") {
        Write-Host "Cancelado. No se ha copiado nada." -ForegroundColor Yellow
        exit 0
    }
}

# /MIR deja el destino igual que el origen, así que las exclusiones son imprescindibles:
# sin ellas /MIR BORRARÍA la configuración y los logs del servidor.
$argumentos = @(
    $salida, $Destino,
    "/MIR",
    "/XF", "Entornos.json", "Empresas.json", "Usuarios.json", "Empresas.json.bak",
    "/XD", (Join-Path $Destino "Logs"),
    "/R:2", "/W:3", "/NP", "/NDL"
)

Write-Host ""
Write-Host "Copiando..." -ForegroundColor Cyan
& robocopy @argumentos | Select-Object -Last 12

# Robocopy usa códigos de salida por bits: 0-7 es correcto, 8 o más es error de verdad.
if ($LASTEXITCODE -ge 8) {
    Write-Host "Robocopy ha devuelto $LASTEXITCODE. Revisa lo de arriba." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Hecho." -ForegroundColor Green
Write-Host "Comprueba que en el servidor siguen estando:" -ForegroundColor Cyan
foreach ($n in $prohibidos) {
    $ruta = Join-Path $Destino "Config\$n"
    $estado = if (Test-Path $ruta) { "sí" } else { "NO" }
    Write-Host "   Config\$n : $estado"
}
$logs = Join-Path $Destino "Logs"
Write-Host "   Logs\ : $(if (Test-Path $logs) { 'sí' } else { 'no (se creará al primer uso)' })"
