using System;
using System.IO;

public class InstallerScriptGenerator
{
    public static void GenerateScript(string appName, string pathToTarGz, string entryPoint)
    {
string scriptTemplate = @"
#!/bin/bash

APP_NAME={{AppName}}
SERVICE_FILE=/etc/systemd/system/$APP_NAME.service
APP_DIR=/var/www/$APP_NAME

# Check if the service is already installed
if systemctl --quiet is-active $APP_NAME.service; then
  echo ""$APP_NAME service is already installed. Reinstalling...""
  # Stop the service if it's running
  systemctl stop $APP_NAME.service
else
  echo ""Installing $APP_NAME service...""
fi

# Kopiowanie aplikacji
mkdir -p $APP_DIR
tar -xzf {{PathToTarGz}} -C $APP_DIR

# Tworzenie pliku usługi systemd
echo ""[Unit]
Description=$APP_NAME Service

[Service]
WorkingDirectory=$APP_DIR
ExecStart=/usr/bin/dotnet $APP_DIR/{{EntryPoint}}
Restart=always

[Install]
WantedBy=multi-user.target"" > $SERVICE_FILE

# Reload systemd manager configuration
systemctl daemon-reload

# Start and enable the service
systemctl start $APP_NAME.service
systemctl enable $APP_NAME.service

echo ""$APP_NAME service installed and started.""
";

        scriptTemplate = scriptTemplate.Replace("{{AppName}}", appName)
                                       .Replace("{{PathToTarGz}}", pathToTarGz)
                                       .Replace("{{EntryPoint}}", entryPoint);

        // Określenie ścieżki do folderu Installers
        string installersDirPath = Path.Combine("/home/kofi/Main/VSCProjects/QuickEndpoint", "Installers");
        
        // Sprawdzenie, czy folder Installers istnieje, jeśli nie - utworzenie go
        if (!Directory.Exists(installersDirPath))
        {
            Directory.CreateDirectory(installersDirPath);
        }

        // Zapisywanie skryptu w folderze Installers
        string scriptPath = Path.Combine(installersDirPath, "install.sh");
        File.WriteAllText(scriptPath, scriptTemplate);
        
        Console.WriteLine($"Skrypt instalacyjny został zapisany w: {scriptPath}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Przykładowe użycie
        InstallerScriptGenerator.GenerateScript("QuickEndpoint_ApiExample", "/home/kofi/Main/VSCProjects/QuickEndpoint/Publish/QuickEndpoint_ApiExample.tar.gz", "QuickEndpoint_ApiExample.dll");
    }
}
