ending=""
if [[ "$OSTYPE" == "msys" ]]; then
    ending=".exe"
fi

go build -o ./publisher${ending} ./Tools/Publisher/main.go
