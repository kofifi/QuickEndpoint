@echo off
                                  echo Installing test...
                                  REM Example command to install service using NSSM and current directory for paths
                                  cd /d %~dp0
                                  nssm.exe install test "%~dp0test.exe"
                                  echo Installation completed.
                                  pause