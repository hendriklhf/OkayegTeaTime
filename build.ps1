$os = @("win-x64", "linux-arm", "osx-x64")

Write-Output "============="
Write-Output "BUILD STARTED"
Write-Output "============="

for ($i = 0; $i -lt $os.length; $i++)
{
    dotnet publish -o .\Build\$($os[$i]) -c Release -r $($os[$i]) -p:PublishSingleFile=true --self-contained true .\OkayegTeaTime\OkayegTeaTime.csproj
}

dotnet test .\Tests\Tests.csproj

cd .\Tools\GitHub
go run ReadMeGenerator.go
cd ..\..

cd .\Tools\Database
go run SqlCreateFormatter.go
cd ..\..

Write-Output "=============="
Write-Output "BUILD FINISHED"
Write-Output "=============="
