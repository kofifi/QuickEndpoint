@echo off
    echo Uninstalling _hardCoded...
    REM Example command to uninstall service using NSSM
    cd /d %~dp0
    nssm.exe remove _hardCoded confirm
    echo Uninstallation completed.
    pause