$ContainerName = 'messenger-pgsql-db'
$ContainerLogPatternForDatabaseReady = "database system is ready to accept connections"
        
for(;;) {
    $isDatabaseReady = 
    docker logs --tail 100 $ContainerName | Select-String -Pattern $ContainerLogPatternForDatabaseReady -SimpleMatch -Quiet
        
    if ($isDatabaseReady -eq $true) {
        Write-Output "`n`nDatabase running inside container ""$ContainerName"" is ready to accept incoming connections"
        break;
    }
    
    Write-Output "Database is not ready."
    Start-Sleep -Seconds 1
}