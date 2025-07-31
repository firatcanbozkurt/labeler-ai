@echo off
echo OPC UA CSV Server - Management Script
echo =====================================

:menu
echo.
echo Select an option:
echo 1. Run OPC UA Server
echo 2. Run OPC UA Server with custom CSV file
echo 3. Run Test Client
echo 4. Generate Sample Data
echo 5. Build Project
echo 6. Exit
echo.
set /p choice="Enter your choice (1-6): "

if "%choice%"=="1" goto run_server
if "%choice%"=="2" goto run_server_custom
if "%choice%"=="3" goto run_client
if "%choice%"=="4" goto generate_data
if "%choice%"=="5" goto build
if "%choice%"=="6" goto exit
goto menu

:run_server
echo.
echo Starting OPC UA Server...
dotnet run
goto menu

:run_server_custom
echo.
set /p csvfile="Enter CSV file path: "
echo Starting OPC UA Server with CSV file: %csvfile%
dotnet run "%csvfile%"
goto menu

:run_client
echo.
echo Starting Test Client...
dotnet run --project TestClient.cs
goto menu

:generate_data
echo.
echo Starting Sample Data Generator...
dotnet run --project GenerateSampleData.cs
goto menu

:build
echo.
echo Building project...
dotnet build
echo Build completed!
goto menu

:exit
echo.
echo Goodbye!
pause