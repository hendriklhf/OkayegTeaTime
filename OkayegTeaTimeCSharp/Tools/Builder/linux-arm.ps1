dotnet publish -o ..\..\bin\Publish\LinuxArm -c Release -r linux-arm -p:PublishSingleFile=true --self-contained true ..\..\OkayegTeaTimeCSharp.csproj
xcopy ..\..\Resources ..\..\bin\Publish\LinuxArm\Resources /E /Y /I
xcopy ..\..\JsonData\Commands.json ..\..\bin\Publish\LinuxArm\Resources /E /Y /I
Remove-Item ..\..\bin\Publish\LinuxArm\Resources\JsonClasses -Recurse -Force -Confirm:$false
