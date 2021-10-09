package main

import (
	"Documents/Git/OkayegTeaTimeCSharp/Starter/starter"
	"fmt"
)

const _programPath = "./OkayegTeaTimeCSharp"

func main() {
	fmt.Println("Starter initialized")
	starter := starter.New(_programPath)
	starter.Initialize()
}
