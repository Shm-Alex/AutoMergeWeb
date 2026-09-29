@echo off
chcp 65001 >nul
echo ==========================================
echo  АВТОМАТИЧЕСКИЙ ДЕПЛОЙ AutoMergeWeb
echo ==========================================
echo.

echo [1/3] Очистка и сборка проекта (Release)...
rmdir /s /q .\publish_temp >nul 2>&1
dotnet publish AutoMergeWeb.csproj -c Release -o .\publish_temp
if %errorlevel% neq 0 (
    echo ОШИБКА СБОРКИ!
    pause
    exit /b %errorlevel%
)

echo.
echo [2/3] Копирование файлов на сервер...
scp -r .\publish_temp\* root@72.56.79.118:/var/www/automerge/
if %errorlevel% neq 0 (
    echo ОШИБКА КОПИРОВАНИЯ!
    pause
    exit /b %errorlevel%
)

echo.
echo [3/3] Перезапуск сервиса на сервере...
ssh root@72.56.79.118 "sudo systemctl restart automerge.service"

echo.
echo ==========================================
echo  ДЕПЛОЙ УСПЕШНО ЗАВЕРШЁН! 
echo ==========================================
pause