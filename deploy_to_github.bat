@echo off
echo ========================================
echo  Asset Management - GitHub Deployment
echo ========================================
echo.

REM Check if git is installed
git --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Git is not installed or not in PATH
    echo Please install Git from https://git-scm.com/
    pause
    exit /b 1
)

REM Navigate to project directory
cd /d "%~dp0"

REM Check if this is already a git repository
if not exist ".git" (
    echo Initializing Git repository...
    git init
    
    echo Adding remote origin...
    git remote add origin https://github.com/ZetallicA/assets.git
) else (
    echo Git repository already exists.
    
    REM Check if remote exists, if not add it
    git remote get-url origin >nul 2>&1
    if errorlevel 1 (
        echo Adding remote origin...
        git remote add origin https://github.com/ZetallicA/assets.git
    )
)

REM Configure Git user if not already set
echo Checking Git user configuration...

REM Check if user.name is set and not empty
git config user.name > temp_git_name.txt 2>&1
set /p current_git_name=<temp_git_name.txt
del temp_git_name.txt

if "%current_git_name%"=="" (
    echo Git user name not configured.
    set /p git_username="Enter your Git username (or press Enter for 'ZetallicA'): "
    if "%git_username%"=="" set git_username=ZetallicA
    git config user.name "%git_username%"
    echo Git user name set to: %git_username%
) else (
    echo Git user name already configured: %current_git_name%
)

REM Check if user.email is set and not empty
git config user.email > temp_git_email.txt 2>&1
set /p current_git_email=<temp_git_email.txt
del temp_git_email.txt

if "%current_git_email%"=="" (
    echo Git user email not configured.
    set /p git_email="Enter your Git email (or press Enter for 'zetallica@example.com'): "
    if "%git_email%"=="" set git_email=zetallica@example.com
    git config user.email "%git_email%"
    echo Git user email set to: %git_email%
) else (
    echo Git user email already configured: %current_git_email%
)

REM Create .gitignore if it doesn't exist
if not exist ".gitignore" (
    echo Creating .gitignore...
    (
        echo # Build results
        echo [Dd]ebug/
        echo [Dd]ebugPublic/
        echo [Rr]elease/
        echo [Rr]eleases/
        echo x64/
        echo x86/
        echo [Ww][Ii][Nn]32/
        echo [Aa][Rr][Mm]/
        echo [Aa][Rr][Mm]64/
        echo bld/
        echo [Bb]in/
        echo [Oo]bj/
        echo [Ll]og/
        echo [Ll]ogs/
        echo.
        echo # Visual Studio
        echo .vs/
        echo *.user
        echo *.suo
        echo *.userosscache
        echo *.sln.docstates
        echo.
        echo # User-specific files
        echo *.rsuser
        echo *.suo
        echo *.user
        echo *.userosscache
        echo *.sln.docstates
        echo.
        echo # Files built by Visual Studio
        echo *_i.c
        echo *_p.c
        echo *_h.h
        echo *.ilk
        echo *.meta
        echo *.obj
        echo *.iobj
        echo *.pch
        echo *.pdb
        echo *.ipdb
        echo *.pgc
        echo *.pgd
        echo *.rsp
        echo *.sbr
        echo *.tlb
        echo *.tli
        echo *.tlh
        echo *.tmp
        echo *.tmp_proj
        echo *_wpftmp.csproj
        echo *.log
        echo *.tlog
        echo *.vspscc
        echo *.vssscc
        echo.
        echo # NuGet
        echo packages/
        echo *.nupkg
        echo *.snupkg
        echo **/[Pp]ackages/*
        echo !**/[Pp]ackages/build/
        echo.
        echo # Database
        echo *.db
        echo *.sqlite
        echo *.sqlite3
        echo.
        echo # Temporary files
        echo wwwroot/uploads/
        echo temp/
        echo *.tmp
        echo.
        echo # Environment files
        echo .env
        echo appsettings.Development.json
        echo appsettings.Production.json
    ) > .gitignore
)

echo.
echo Adding files to git...
git add .

echo.
echo Committing changes...
set /p commit_message="Enter commit message (or press Enter for default): "
if "%commit_message%"=="" set commit_message=Update Asset Management System

git commit -m "%commit_message%"

echo.
echo Pushing to GitHub...
echo NOTE: You may need to authenticate with GitHub
echo If this is your first push, you might need to set up authentication:
echo   - Personal Access Token: https://github.com/settings/tokens
echo   - Or use GitHub CLI: gh auth login
echo.

git branch -M main
git push -u origin main

if errorlevel 1 (
    echo.
    echo ========================================
    echo  PUSH FAILED - Authentication Required
    echo ========================================
    echo.
    echo Please set up GitHub authentication:
    echo 1. Generate Personal Access Token: https://github.com/settings/tokens
    echo 2. Use token as password when prompted
    echo 3. Or install GitHub CLI: gh auth login
    echo.
    echo Then run this script again.
    pause
    exit /b 1
) else (
    echo.
    echo ========================================
    echo  SUCCESS! Project pushed to GitHub
    echo ========================================
    echo.
    echo Repository URL: https://github.com/ZetallicA/assets
    echo.
)

pause
