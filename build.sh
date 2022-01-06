os=("win-x64" "linux-arm" "linux-x64")

echo "============="
echo "BUILD STARTED"
echo "============="

for item in ${os}
do
    dotnet publish -o ./Build/${item} -c Release -r ${item} -p:PublishSingleFile=true --self-contained true ./OkayegTeaTime/OkayegTeaTime.csproj
done

dotnet test ./Tests/Tests.csproj

cd ./Tools/GitHub
go run ReadMeGenerator.go
cd ../..

cd ./Tools/Database
go run SqlCreateFormatter.go
cd ../..

echo "=============="
echo "BUILD FINISHED"
echo "=============="
