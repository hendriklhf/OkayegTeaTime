dotnet publish -c Release -o ./Build/Tools/  --self-contained ./OkayegTeaTime.Tools/OkayegTeaTime.Tools.csproj
mv ./Build/Tools/OkayegTeaTime.Tools.exe ./tools.exe
rm -r ./Build/Tools
