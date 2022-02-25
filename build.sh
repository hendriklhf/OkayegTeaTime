dotnet publish -r win-x64 -c Release -o ./Build/Tools/ -p:PublishSingleFile=true --self-contained false ./OkayegTeaTime.Tools/OkayegTeaTime.Tools.csproj
cp ./Build/Tools/OkayegTeaTime.Tools.exe ./tools.exe
