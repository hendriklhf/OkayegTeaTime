dotnet publish -o Build/Tools src/OkayegTeaTime.Tools/OkayegTeaTime.Tools.csproj
mv Build/Tools/OkayegTeaTime.Tools.exe tools.exe
rm -r Build/Tools
