# Run this script whenever Windows blocks new DLLs after a build
# Usage: Right-click -> Run with PowerShell
Get-ChildItem -Path $PSScriptRoot -Recurse -Include "*.dll","*.exe" | Unblock-File
Write-Host "✅ All DLLs unblocked. You can now run the app." -ForegroundColor Green
pause
