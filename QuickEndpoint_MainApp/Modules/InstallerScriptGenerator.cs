using System;
using System.IO;

namespace QuickEndpoint_MainApp.Modules
{
    public static class InstallerScriptGenerator
    {
        public static void GenerateScript(string appName, string entryPoint, string installersDir, string nssmExecutablePath)
        {
            // PowerShell script template with parameters for customization
            string scriptTemplate = @$"
param(
    [string]$ServiceName = '{appName}',
    [string]$EntryPoint = '{entryPoint}'
)

$serviceName = $ServiceName
$serviceDisplayName = $ServiceName
$appDirectory = '{installersDir}'  # Application directory
$nssmPath = '{nssmExecutablePath}'

# Function for logging messages
function Log-Message {{
    param(
        [string]$Message,
        [string]$Level = 'Info'
    )
    $timeStamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
    Write-Output ""[$timeStamp][$Level] $Message""
}}

# Ensure the application directory exists
Log-Message 'Ensuring the application directory exists...'
if (-Not (Test-Path $appDirectory)) {{
    New-Item -ItemType Directory -Path $appDirectory
    Log-Message 'Application directory created.'
}}

$exePath = Join-Path $appDirectory $EntryPoint

# Check if the service already exists with NSSM and remove it if necessary
Log-Message 'Checking if the service already exists...'
$status = & $nssmPath status $serviceName
if ($status -ne 'SERVICE_NOT_FOUND') {{
    Log-Message 'Service already exists. Removing existing service...'
    & $nssmPath stop $serviceName
    & $nssmPath remove $serviceName confirm
}}

# Use NSSM to install the service
Log-Message 'Installing service with NSSM...'
& $nssmPath install $serviceName `""$exePath`""

# Set service description with NSSM (optional)
& $nssmPath set $serviceName Description `""$serviceDisplayName`""

# Attempt to start the service with NSSM
try {{
    & $nssmPath start $serviceName
    Log-Message '$serviceName service installed and started with NSSM.'
}} catch {{
    Log-Message 'Failed to start the service. Please check the service configuration.'
}}

";

            string scriptPath = Path.Combine(installersDir, $"{appName}_install.ps1");
            File.WriteAllText(scriptPath, scriptTemplate);
            Console.WriteLine($"Installer script has been saved at: {scriptPath}");
        }

        public static void GenerateUninstallScript(string appName, string uninstallersDir, string serviceName)
        {
            // PowerShell script template for uninstallation
            string uninstallScriptTemplate = @$"
param(
    [string]$ServiceName = '{serviceName}'
)

# Use sc.exe to stop and then delete the service
& sc.exe stop $ServiceName
& sc.exe delete $ServiceName
";
            string uninstallScriptPath = Path.Combine(uninstallersDir, $"{appName}_uninstall.ps1");
            File.WriteAllText(uninstallScriptPath, uninstallScriptTemplate);
            Console.WriteLine($"Uninstaller script has been saved at: {uninstallScriptPath}");
        }
    }
}
