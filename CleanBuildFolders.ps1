#remove all build folders for a truely "CLean" solution
$folderNames = "bin", "obj"
$currentDirectory = Get-Location
$directories = Get-ChildItem -Path $currentDirectory -Recurse -Directory

foreach ($directory in $directories) {
    foreach ($folderName in $folderNames) {
        $path = "$($directory.FullName)\$folderName"
        if (Test-Path -Path $path) {
            Remove-Item -Path $path -Recurse -Force
            Write-Output "Deleted $path"
        }
    }
}
