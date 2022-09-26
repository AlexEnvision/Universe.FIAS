# инициализация путей
if($true)
{
    $scriptpath = $MyInvocation.MyCommand.Path
    "ScriptPath: {0}" -f $scriptpath
    #$scriptpath = "полный путь к текущему файлу"
    if($scriptpath -eq $null)
    {
        Write-Host "!!! 2. scriptpath - specify the full path to the current file" -ForegroundColor:Red
        return
    }

    $folderPath = split-path -parent $scriptpath
    "FolderPath: {0}" -f $folderPath
}

$srcP = Resolve-Path -Path ([System.IO.Path]::Combine($folderPath, "..\"))

"`$srcP: $srcP"


$nugetPackegesSpec = $folderPath
"NugetPackages Spec: $nugetPackegesSpec"


cd $nugetPackegesSpec 

invoke-expression -Command "$nugetPackegesSpec\nuget.exe pack Universe.FIAS.Normalizer.nuspec"

$outputDir = "$nugetPackegesSpec\Pkgs"
"Nuget packages directory: $outputDir"

if (![System.IO.Directory]::Exists($outputDir))
{
   [System.IO.Directory]::CreateDirectory($outputDir);
}

$nupkgs = [System.IO.Directory]::GetFiles($nugetPackegesSpec, "*.nupkg")

"Replaced nuget packages:"
$nupkgs

for ($index = $nupkgs.Count - 1; $index -ge 0; $index--){
    $item = $nupkgs[$index]

    $filename = [System.IO.Path]::GetFileName($item)
    Copy-Item  -Path $item -Destination $outputDir\$filename
    Remove-Item -Path $item
}

Write-Host "All operations done!" -ForegroundColor Green