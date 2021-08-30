dotnet publish -o ..\..\bin\Publish\Win-x64 -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true ..\..\OkayegTeaTimeCSharp.csproj
xcopy ..\..\Resources ..\..\bin\Publish\Win-x64\Resources /E /Y /I
node ..\GitHub\ReadMeGenerator.js
