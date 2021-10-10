package main

import (
	"fmt"
	"os"
	"regexp"
	"strings"

	linq "github.com/ahmetb/go-linq"
)

const _sqlFilePath = "../../OkayegTeaTimeCSharp/Resources/SqlCreate.sql"

func main() {
	FormatSqCreate()
	fmt.Println("Formatted SqlCreate.sql")
}

func FormatSqCreate() {
	patternComment, _ := regexp.Compile("^--")
	patternAnyWordChar, _ := regexp.Compile(`\w+`)
	bytes, err := os.ReadFile(_sqlFilePath)
	if err == nil {
		fileContent := strings.Split(string(bytes), "\r\n")
		var resultLines []string
		linq.From(fileContent).WhereT(func(s string) bool {
			return patternAnyWordChar.Match([]byte(s)) && !patternComment.Match([]byte(s))
		}).ToSlice(&resultLines)
		result := strings.Join(resultLines, "\r\n")
		err := os.WriteFile(_sqlFilePath, []byte(result), os.ModePerm)
		if err != nil {
			fmt.Println(err)
		}
	} else {
		fmt.Println(err)
	}
}
