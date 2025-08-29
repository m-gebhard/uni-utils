@echo off

REM Uninstall previous and use version 2.61.0 of DoxFX to generate metadata
dotnet tool uninstall -g docfx || echo "DocFX not installed, skipping uninstall..."
dotnet tool install -g docfx --version 2.61.0
docfx metadata

REM Upgrade to version 2.78.3 to build PDF
dotnet tool install -g docfx --version 2.78.3
docfx build
docfx pdf

REM Move to destination
move _site\api\toc.pdf OfflineDocumentation.pdf

pause
