package Json

import (
	"encoding/json"
	"fmt"
	"os"
)

type JsonController struct {
	JsonCommands JsonCommands
	_path        string
}

func New(path string) *JsonController {
	return &JsonController{
		_path: path,
	}
}

func (jsonController *JsonController) LoadData() {
	fileContent, err := os.ReadFile(jsonController._path)
	if err == nil {
		err := json.Unmarshal(fileContent, &jsonController.JsonCommands)
		if err != nil {
			fmt.Println(err)
		}
	} else {
		fmt.Println(err)
	}
}
