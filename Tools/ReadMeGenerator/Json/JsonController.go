package Json

import (
	"encoding/json"
	"fmt"
	"os"
)

type JsonController struct {
	JsonCommands JsonCommands
	path         string
}

func New(path string) *JsonController {
	return &JsonController{
		path: path,
	}
}

func (jsonController *JsonController) LoadData() {
	fileContent, err := os.ReadFile(jsonController.path)
	if err == nil {
		err := json.Unmarshal(fileContent, &jsonController.JsonCommands)
		if err != nil {
			fmt.Println(err)
		}
	} else {
		fmt.Println(err)
	}
}
