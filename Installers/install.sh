
#!/bin/bash

APP_NAME=QuickEndpoint_ApiExample
SERVICE_FILE=/etc/systemd/system/$APP_NAME.service
APP_DIR=/var/www/$APP_NAME

# Check if the service is already installed
if systemctl --quiet is-active $APP_NAME.service; then
  echo "$APP_NAME service is already installed. Reinstalling..."
  # Stop the service if it's running
  systemctl stop $APP_NAME.service
else
  echo "Installing $APP_NAME service..."
fi

# Kopiowanie aplikacji
mkdir -p $APP_DIR
tar -xzf /home/kofi/Main/VSCProjects/QuickEndpoint/Publish/QuickEndpoint_ApiExample.tar.gz -C $APP_DIR

# Tworzenie pliku usługi systemd
echo "[Unit]
Description=$APP_NAME Service

[Service]
WorkingDirectory=$APP_DIR
ExecStart=/usr/bin/dotnet $APP_DIR/QuickEndpoint_ApiExample.dll
Restart=always

[Install]
WantedBy=multi-user.target" > $SERVICE_FILE

# Reload systemd manager configuration
systemctl daemon-reload

# Start and enable the service
systemctl start $APP_NAME.service
systemctl enable $APP_NAME.service

echo "$APP_NAME service installed and started."
