
param(
    [string]$ServiceName = 'QuickEndpoint_ApiExample'
)

# Use sc.exe to stop and then delete the service
& sc.exe stop $ServiceName
& sc.exe delete $ServiceName
