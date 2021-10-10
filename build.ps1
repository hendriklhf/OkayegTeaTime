dotnet publish -o .\Build\win-x64 -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true .\OkayegTeaTimeCSharp\OkayegTeaTimeCSharp.csproj
dotnet publish -o .\Build\linux-arm -c Release -r linux-arm -p:PublishSingleFile=true --self-contained true .\OkayegTeaTimeCSharp\OkayegTeaTimeCSharp.csproj
dotnet publish -o .\Build\macOS -c Release -r osx-x64 -p:PublishSingleFile=true --self-contained true .\OkayegTeaTimeCSharp\OkayegTeaTimeCSharp.csproj
xcopy .\OkayegTeaTimeCSharp\Resources .\Build\win-x64\Resources /E /Y /I
xcopy .\OkayegTeaTimeCSharp\Resources .\Build\linux-arm\Resources /E /Y /I
xcopy .\OkayegTeaTimeCSharp\Resources .\Build\macOS\Resources /E /Y /I

dotnet test .\Tests\Tests.csproj

node .\Tools\GitHub\ReadMeGenerator.js

cd .\Tools\Database
go run SqlCreateFormatter.go
cd ..\..

cd .\Starter
go env -w GOOS=linux GOARCH=arm
go build -o ..\Build\linux-arm
Write-Output "Built Starter for linux-arm"
go env -w GOOS=darwin GOARCH=amd64
go build -o ..\build\macOS
Write-Output "Built Starter for macOS"
go env -w GOOS=windows
go build -o ..\Build\win-x64
Write-Output "Built Starter for win-x64"
cd ..

Write-Output "=============="
Write-Output "BUILD FINISHED"
Write-Output "=============="
