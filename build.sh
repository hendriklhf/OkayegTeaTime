dotnet publish -r win-x64 -c Release -o ./Build/Tools/ -p:PublishAot=true -p:IncludeNativeLibrariesForSelfExtract=true --self-contained ./OkayegTeaTime.Tools/OkayegTeaTime.Tools.csproj
mv ./Build/Tools/OkayegTeaTime.Tools.exe ./tools.exe
rm -r ./Build/Tools
