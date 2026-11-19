$ErrorActionPreference = "Stop"

$ProjectRoot         = $PSScriptRoot
$AppCsproj           = Join-Path $ProjectRoot "JajoDex.csproj"
$AppPublishDir       = Join-Path $ProjectRoot "publish\win-x64"
$InstallerResources  = Join-Path $ProjectRoot "JajoDex.Installer\Resources"
$PayloadZip          = Join-Path $InstallerResources "payload.zip"
$InstallerCsproj     = Join-Path $ProjectRoot "JajoDex.Installer\JajoDex.Installer.csproj"
$InstallerPublishDir = Join-Path $ProjectRoot "publish\installer-win-x64"
$OutputDir           = Join-Path $ProjectRoot "Installer\Output"
$FinalSetupExe       = Join-Path $OutputDir "JajoDex_Setup_v1.0.0.exe"

Write-Host "============================================================" -ForegroundColor DarkMagenta
Write-Host "  JajoDex Custom WPF Installer Build Pipeline (win-x64)" -ForegroundColor White
Write-Host "============================================================" -ForegroundColor DarkMagenta

# 1. Clean and publish JajoDex self-contained single-file release
if (Test-Path $AppPublishDir) {
    Write-Host "[1/4] Cleaning $AppPublishDir ..." -ForegroundColor Gray
    Remove-Item -Path $AppPublishDir -Recurse -Force
}
Write-Host "[1/4] Publishing JajoDex (win-x64 self-contained single-file)..." -ForegroundColor Cyan

& dotnet publish $AppCsproj `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o $AppPublishDir

if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet publish JajoDex failed with exit code $LASTEXITCODE."
    exit $LASTEXITCODE
}

$PublishedAppExe = Join-Path $AppPublishDir "JajoDex.exe"
if (-not (Test-Path $PublishedAppExe)) {
    Write-Error "Expected published executable not found at: $PublishedAppExe"
    exit 1
}
$AppSizeMb = [Math]::Round((Get-Item $PublishedAppExe).Length / 1MB, 2)
Write-Host "      JajoDex.exe published ($AppSizeMb MB)" -ForegroundColor Green

# 2. Compress publish\win-x64\* into JajoDex.Installer\Resources\payload.zip
Write-Host "[2/4] Compressing payload into $PayloadZip ..." -ForegroundColor Cyan
New-Item -ItemType Directory -Path $InstallerResources -Force | Out-Null
if (Test-Path $PayloadZip) {
    Remove-Item -Path $PayloadZip -Force
}

Compress-Archive -Path (Join-Path $AppPublishDir "*") -DestinationPath $PayloadZip -Force
if (-not (Test-Path $PayloadZip)) {
    Write-Error "Failed to create payload archive at: $PayloadZip"
    exit 1
}
$ZipSizeMb = [Math]::Round((Get-Item $PayloadZip).Length / 1MB, 2)
Write-Host "      payload.zip created ($ZipSizeMb MB)" -ForegroundColor Green

# 3. Clean and publish JajoDex.Installer self-contained single-file executable
if (Test-Path $InstallerPublishDir) {
    Write-Host "[3/4] Cleaning $InstallerPublishDir ..." -ForegroundColor Gray
    Remove-Item -Path $InstallerPublishDir -Recurse -Force
}
Write-Host "[3/4] Publishing JajoDex.Installer (win-x64 self-contained single-file)..." -ForegroundColor Cyan

& dotnet publish $InstallerCsproj `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o $InstallerPublishDir

if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet publish JajoDex.Installer failed with exit code $LASTEXITCODE."
    exit $LASTEXITCODE
}

$PublishedInstallerExe = Join-Path $InstallerPublishDir "JajoDex.Installer.exe"
if (-not (Test-Path $PublishedInstallerExe)) {
    Write-Error "Expected installer binary not found at: $PublishedInstallerExe"
    exit 1
}

# 4. Copy and rename final installer executable to Installer\Output\JajoDex_Setup_v1.0.0.exe
Write-Host "[4/4] Packaging final standalone setup executable..." -ForegroundColor Cyan
New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
Copy-Item -Path $PublishedInstallerExe -Destination $FinalSetupExe -Force

$FinalSizeMb = [Math]::Round((Get-Item $FinalSetupExe).Length / 1MB, 2)
Write-Host ""
Write-Host "Build succeeded! Standalone installer ready:" -ForegroundColor Green
Write-Host "  Path: $FinalSetupExe" -ForegroundColor White
Write-Host "  Size: $FinalSizeMb MB" -ForegroundColor Cyan
