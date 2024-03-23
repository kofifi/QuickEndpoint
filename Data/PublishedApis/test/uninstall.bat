@echo off
                                  echo Uninstalling test...
                                  REM Example command to uninstall service using NSSM
                                  cd /d %~dp0
                                  nssm.exe remove test confirm
                                  echo Uninstallation completed.
                                  pause