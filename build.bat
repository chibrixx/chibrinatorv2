@echo off
echo ============================================
echo   Chibinator - Build Script
echo ============================================
echo.

:: Check for dotnet
where dotnet >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] .NET SDK not found. Install from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo [1/3] Restoring NuGet packages...
dotnet restore Chibinator.sln
if %ERRORLEVEL% NEQ 0 ( echo [ERROR] Restore failed. & pause & exit /b 1 )

echo.
echo [2/3] Building solution (Release, x64)...
dotnet build Chibinator.sln -c Release -r win-x64 --no-restore
if %ERRORLEVEL% NEQ 0 ( echo [ERROR] Build failed. & pause & exit /b 1 )

echo.
echo [3/3] Publishing self-contained executable...
dotnet publish Chibinator.UI\Chibinator.UI.csproj ^
    -c Release ^
    -r win-x64 ^
    --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:IncludeNativeLibrariesForSelfExtract=true ^
    -o publish\

if %ERRORLEVEL% NEQ 0 ( echo [ERROR] Publish failed. & pause & exit /b 1 )

echo.
echo ============================================
echo   BUILD COMPLETE
echo   Output: publish\Chibinator.exe
echo ============================================
pause
