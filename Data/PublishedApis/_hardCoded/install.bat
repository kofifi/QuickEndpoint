@echo off
                                  echo Installing _hardCoded...
                                  REM Example command to install service using NSSM and current directory for paths
                                  cd /d %~dp0
                                  nssm.exe install _hardCoded "%~dp0_hardCoded.exe"
                                  echo Installation completed.
                                  pause