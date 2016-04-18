@ECHO OFF
PowerShell -NoProfile -NoLogo -ExecutionPolicy unrestricted -Command "& '%~dp0build.ps1' %*; exit $LASTEXITCODE"