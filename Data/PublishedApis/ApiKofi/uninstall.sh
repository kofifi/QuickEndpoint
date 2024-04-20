#!/bin/bash
echo 'Uninstalling ApiKofi...'
if sudo systemctl stop ApiKofi; then
    echo 'ApiKofi stopped successfully.'
else
    echo 'Failed to stop ApiKofi.'
fi

if sudo systemctl disable ApiKofi; then
    echo 'ApiKofi disabled successfully.'
else
    echo 'Failed to disable ApiKofi.'
fi

SERVICE_FILE_PATH="/etc/systemd/system/ApiKofi.service"
if [ -f "$SERVICE_FILE_PATH" ]; then
    sudo rm "$SERVICE_FILE_PATH"
    echo 'Service file removed successfully.'
else
    echo 'Service file not found.'
fi

sudo systemctl daemon-reload
echo 'Systemd configuration reloaded.'
echo 'Uninstallation completed.'
read -p 'Press any key to continue...'