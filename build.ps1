$os = @("win-x64", "linux-arm", "osx-x64")
$goos = @("windows", "linux", "darwin")
$goarch = @("amd64", "arm", "amd64")

for ($i = 0; $i -lt $folder.length; $i++) {
    dotnet publish -o .\Build\$os[$i] -c Release -r $os[$i] -p:PublishSingleFile=true --self-contained true .\OkayegTeaTimeCSharp\OkayegTeaTimeCSharp.csproj
    xcopy .\OkayegTeaTimeCSharp\Resources .\Build\$os[$i]\Resources /E /Y /I
}

dotnet test .\Tests\Tests.csproj

node .\Tools\GitHub\ReadMeGenerator.js

cd .\Tools\Database
go run SqlCreateFormatter.go
cd ..\..

cd .\Starter
for ($i = 0; $i -lt $goos.length; $i++) {
    go env -w GOOS=$goos[$i] GOARCH=$goarch[$i]
    go build -o ..\Build\$os[$i]
    Write-Output "Built Starter for $($goos[$i])"
}
go env -w GOOS=$goos[0] GOARCH=$goarch[0]
cd ..

Write-Output "=============="
Write-Output "BUILD FINISHED"
Write-Output "=============="
