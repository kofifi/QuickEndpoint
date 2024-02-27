
param(
    [string]$ServiceName = 'QuickEndpoint_ApiExample',
    [string]$EntryPoint = 'QuickEndpoint_ApiExample.exe'
)

$serviceName = $ServiceName
$serviceDisplayName = $ServiceName
$appDirectory = 'E:\source_code\QuickEndpoint\Installers\QuickEndpoint_ApiExample'  # Application directory
$nssmPath = 'E:\source_code\QuickEndpoint\Tools\nssm-2.24\win64\nssm.exe'

# Function for logging messages
function Log-Message {
    param(
        [string]$Message,
        [string]$Level = 'Info'
    )
    $timeStamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
    Write-Output "[$timeStamp][$Level] $Message"
}

# Ensure the application directory exists
Log-Message 'Ensuring the application directory exists...'
if (-Not (Test-Path $appDirectory)) {
    New-Item -ItemType Directory -Path $appDirectory
    Log-Message 'Application directory created.'
}

$exePath = Join-Path $appDirectory $EntryPoint

# Check if the service already exists with NSSM and remove it if necessary
Log-Message 'Checking if the service already exists...'
$status = & $nssmPath status $serviceName
if ($status -ne 'SERVICE_NOT_FOUND') {
    Log-Message 'Service already exists. Removing existing service...'
    & $nssmPath stop $serviceName
    & $nssmPath remove $serviceName confirm
}

# Use NSSM to install the service
Log-Message 'Installing service with NSSM...'
& $nssmPath install $serviceName `"$exePath`"

# Set service description with NSSM (optional)
& $nssmPath set $serviceName Description `"$serviceDisplayName`"

# Attempt to start the service with NSSM
try {
    & $nssmPath start $serviceName
    Log-Message '$serviceName service installed and started with NSSM.'
} catch {
    Log-Message 'Failed to start the service. Please check the service configuration.'
}

