if (-not (Test-Path "build")) {
    New-Item "build" -ItemType Directory
}

dotnet publish Sn.AsciiArtApp\Sn.AsciiArtApp.csproj `
    -c Release `
    -r win-x64 `
    --no-self-contained `
    -p:PublishSingleFile=true `
    -o build

dotnet publish Sn.AsciiArtPlayer\Sn.AsciiArtPlayer.csproj `
    -c Release `
    -r win-x64 `
    --no-self-contained `
    -p:PublishSingleFile=true `
    -o build