 param (
    [string]$logFile = $( Read-Host "Input full log file path" ),
    [string]$saveFile = $( Read-Host "Input full csv file path including the extension" )
 );

Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Force ;

function hash2obj($data) {
    $stuff = @();
    
    foreach($row in $data) {
        $obj = new-object PSObject
        foreach($key in $row.keys) { 
            $obj | add-member -membertype NoteProperty -name $key -value $row[$key]
        }
        $stuff += $obj
    }
    
    return $stuff;
}

$matches_found = @()
$userConnections = @();
cat $logFile | %{
if ($_ -match '(?:(\d{1,2}\/\d{1,2}\/\d{4} \d{1,2}:\d{1,2}:\d{1,2} (?:A|P)M):.*(\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b).*ID (.*)_(\d*):)'){
    $userConnection = @{};
    $userConnection["DateTime"] = $matches[1];
    $userConnection["IP"] = $matches[2];
    $ip = $matches[2];
    $infoService = "http://freegeoip.net/xml/$ip";
    Try{
        $geoip = Invoke-RestMethod -Method Get -URI $infoService;
        $userConnection["Geolocation"] = $geoip.Response; 
    } Catch {
        $userConnection["Geolocation"]= "Could not get IP geolocation.";
    }   
    echo $geoip.Response;
    $userConnection["Username"] = $matches[3];
    $userConnection["Model"] = $matches[4];
    $userConnections += hash2obj($userConnection);
    }
}

$userConnections | export-csv $saveFile -notypeinformation;

