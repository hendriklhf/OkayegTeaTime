dotnet publish -o ./Build/Tools/ ./OkayegTeaTime.Tools/OkayegTeaTime.Tools.csproj
mv ./Build/Tools/OkayegTeaTime.Tools.exe ./tools.exe
rm -r ./Build/Tools
