
param(
    [string]$ServiceName = 'QuickEndpoint_ApiExample',
    [string]$TarGzPath = 'E:\source_code\QuickEndpoint\Publish\QuickEndpoint_ApiExample.tar.gz',
    [string]$EntryPoint = 'QuickEndpoint_ApiExample.exe'
)

$serviceName = $ServiceName
$serviceDisplayName = $ServiceName
$appDirectory = Join-Path 'E:\source_code\QuickEndpoint\Publish' $ServiceName
$nssmPath = 'E:\source_code\QuickEndpoint\Tools\nssm-2.24\win64\nssm.exe'

# Ensure the application directory exists
if (-Not (Test-Path $appDirectory)) {
    New-Item -ItemType Directory -Path $appDirectory
}

# Extract the application archive
Write-Output 'Extracting application archive...'
& tar -xzf $TarGzPath -C $appDirectory

$exePath = Join-Path $appDirectory $EntryPoint

# Check if the service already exists with NSSM and remove it if necessary
$status = & $nssmPath status $serviceName
if ($status -ne 'SERVICE_NOT_FOUND') {
    Write-Output 'Service already exists. Removing existing service...'
    & $nssmPath stop $serviceName
    & $nssmPath remove $serviceName confirm
}

# Use NSSM to install the service
Write-Output 'Installing service with NSSM...'
& $nssmPath install $serviceName `"$exePath`"

# Set service description with NSSM (optional)
& $nssmPath set $serviceName Description `"$serviceDisplayName`"

# Attempt to start the service with NSSM
try {
    & $nssmPath start $serviceName
    Write-Output '$serviceName service installed and started with NSSM.'
} catch {
    Write-Output 'Failed to start the service. Please check the service configuration.'
}
