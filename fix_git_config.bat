@echo off
echo ========================================
echo  Fix Git User Configuration
echo ========================================
echo.

echo Setting Git user configuration...
git config user.name "ZetallicA"
git config user.email "rabi.hamsi@gmail.com"

echo.
echo Git configuration set:
echo Username: %git config user.name%
echo Email: %git config user.email%
echo.

echo Configuration complete! You can now run deploy_to_github.bat
pause
