dotnet publish -o .\Build\Win-x64 -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true .\OkayegTeaTimeCSharp\OkayegTeaTimeCSharp.csproj
dotnet publish -o .\Build\LinuxArm -c Release -r linux-arm -p:PublishSingleFile=true --self-contained true .\OkayegTeaTimeCSharp\OkayegTeaTimeCSharp.csproj
xcopy .\OkayegTeaTimeCSharp\Resources .\Build\Win-x64\Resources /E /Y /I
xcopy .\OkayegTeaTimeCSharp\Resources .\Build\LinuxArm\Resources /E /Y /I

dotnet test .\Tests\Tests.csproj

node .\Tools\GitHub\ReadMeGenerator.js

cd .\Tools\Database
go run SqlCreateFormatter.go
cd ..\..

cd .\Starter
go env -w GOOS=linux GOARCH=arm
go build -o ..\Build\LinuxArm
go env -w GOOS=windows GOARCH=amd64
go build -o ..\Build\Win-x64
Write-Output "Built Starter"
cd ..

Write-Output "=============="
Write-Output "BUILD FINISHED"
Write-Output "=============="
