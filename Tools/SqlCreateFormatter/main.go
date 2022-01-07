package main

import (
	"fmt"
	"os"
	"regexp"
	"strings"

	linq "github.com/ahmetb/go-linq"
)

var (
	patternComment     = regexp.MustCompile("^--")
	patternAnyWordChar = regexp.MustCompile(`\w+`)
)

const (
	SqlFilePath = "./OkayegTeaTime/Resources/SqlCreate.sql"
)

func main() {
	FormatSqCreate()
	fmt.Println("Formatted SqlCreate.sql")
}

func FormatSqCreate() {
	bytes, err := os.ReadFile(SqlFilePath)
	if err == nil {
		fileContent := strings.Split(string(bytes), "\r\n")
		var resultLines []string
		linq.From(fileContent).WhereT(func(s string) bool {
			return patternAnyWordChar.MatchString(s) && !patternComment.MatchString(s)
		}).ToSlice(&resultLines)
		result := strings.Join(resultLines, "\r\n")
		err := os.WriteFile(SqlFilePath, []byte(result), os.ModePerm)
		if err != nil {
			fmt.Println(err)
		}
	} else {
		fmt.Println(err)
	}
}
