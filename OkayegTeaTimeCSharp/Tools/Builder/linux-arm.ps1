dotnet publish -o ..\..\bin\Publish\LinuxArm -c Release -r linux-arm -p:PublishSingleFile=true --self-contained true ..\..\OkayegTeaTimeCSharp.csproj
xcopy ..\..\Resources ..\..\bin\Publish\LinuxArm\Resources /E /Y /I
.\SubScripts\runTests.ps1
.\SubScripts\generateReadme.ps1
.\SubScripts\formatSqlCreate.ps1

.\SubScripts\finishedBuild.ps1
