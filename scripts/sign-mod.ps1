# Authenticode-sign Struggler.dll so Windows Smart App Control will load it.
#
# SAC accepts only signatures from a CA in Microsoft's Trusted Root Program.
# Self-signed certs do NOT satisfy SAC. Use one of:
#   - SignPath Foundation (free for eligible open-source projects)
#   - A purchased OV code signing cert (Certum, SSL.com, etc.)
#   - Azure Artifact Signing (paid, cloud HSM)
#
# Usage (PowerShell, from repo root):
#   # Certificate already in CurrentUser\My store (thumbprint from certmgr.msc):
#   .\scripts\sign-mod.ps1 -ModsDir "C:\...\mods\Struggler" -CertThumbprint "ABC123..."
#
#   # Or PFX file (keep secrets out of git):
#   $env:STRUGGLER_SIGN_PFX = "C:\certs\struggler.pfx"
#   $env:STRUGGLER_SIGN_PASSWORD = "your-password"
#   .\scripts\sign-mod.ps1 -ModsDir "C:\...\mods\Struggler"
#
# WSL install.sh calls this automatically when STRUGGLER_SIGN_CERT or
# STRUGGLER_SIGN_PFX is set.

param(
    [Parameter(Mandatory = $true)]
    [string]$ModsDir,

    [string]$CertThumbprint = $env:STRUGGLER_SIGN_CERT,
    [string]$PfxPath = $env:STRUGGLER_SIGN_PFX,
    [string]$PfxPassword = $env:STRUGGLER_SIGN_PASSWORD,
    [string]$TimestampUrl = "http://timestamp.digicert.com"
)

$ErrorActionPreference = "Stop"

$dll = Join-Path $ModsDir "Struggler.dll"
if (-not (Test-Path -LiteralPath $dll)) {
    Write-Error "Struggler.dll not found: $dll"
    exit 1
}

function Find-SignTool {
    $candidates = @(
        "${env:ProgramFiles(x86)}\Windows Kits\10\bin\*\x64\signtool.exe",
        "${env:ProgramFiles}\Windows Kits\10\bin\*\x64\signtool.exe"
    )
    foreach ($pattern in $candidates) {
        $hit = Get-Item $pattern -ErrorAction SilentlyContinue | Sort-Object FullName -Descending | Select-Object -First 1
        if ($hit) { return $hit.FullName }
    }
    $onPath = Get-Command signtool.exe -ErrorAction SilentlyContinue
    if ($onPath) { return $onPath.Source }
    return $null
}

$signtool = Find-SignTool
if (-not $signtool) {
    Write-Error @"
signtool.exe not found. Install the Windows SDK (Signing Tools for Desktop Apps):
  https://developer.microsoft.com/en-us/windows/downloads/windows-sdk/
Or open a "Developer PowerShell for VS" prompt where signtool is on PATH.
"@
    exit 1
}

$signArgs = @("sign", "/fd", "SHA256", "/tr", $TimestampUrl, "/td", "SHA256", "/a")

if ($PfxPath) {
    if (-not (Test-Path -LiteralPath $PfxPath)) {
        Write-Error "PFX not found: $PfxPath"
        exit 1
    }
    $signArgs += @("/f", $PfxPath)
    if ($PfxPassword) {
        $signArgs += @("/p", $PfxPassword)
    }
} elseif ($CertThumbprint) {
    $signArgs += @("/sha1", $CertThumbprint)
} else {
    Write-Error @"
No signing certificate configured.

Set one of:
  STRUGGLER_SIGN_CERT=<thumbprint>   # cert in CurrentUser\My
  STRUGGLER_SIGN_PFX=C:\path\to.pfx  # optional: STRUGGLER_SIGN_PASSWORD

For a trusted cert without buying one, apply to SignPath Foundation (open source):
  https://about.signpath.io/

Cheapest paid option (~`$80/yr): Certum Code Signing in the Cloud.
"@
    exit 1
}

$signArgs += $dll

Write-Host "Signing $dll"
& $signtool @signArgs
if ($LASTEXITCODE -ne 0) {
    Write-Error "signtool failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Signed successfully. SAC should allow this DLL if the cert chains to a Microsoft-trusted root."
Write-Host "New certs may still need download reputation before SmartScreen fully trusts them."
