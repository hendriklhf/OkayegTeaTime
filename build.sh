ending=""
if [[ "$OSTYPE" == "msys" ]]; then
    ending=".exe"
fi

go build -o ./publisher${ending} ./Tools/Publisher/main.go

cd ./Tools/ReadMeGenerator/
go build -o ../../readmegenerator${ending} ./main.go
cd ../SqlCreateFormatter/
go build -o ../../formatter${ending} ./main.go
cd ../..
