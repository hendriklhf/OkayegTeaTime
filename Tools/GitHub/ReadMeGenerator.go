package main

import (
	"Documents/Git/OkayegTeaTimeCSharp/Tools/GitHub/Json"
	"fmt"
	"os"
	"strings"

	linq "github.com/ahmetb/go-linq"
)

var (
	header1Text = []string{
		"If your channel has a custom prefix set, commands will have to start with the prefix.",
		"If your channel has no prefix set, commands will have to end with \"eg\", for example: \"pingeg\".",
		"Text in \"[ ]\" is a variable parameter",
	}
	cmdTableHeader = []string{
		"Command",
		"Alias",
		"Description [Parameter | Output]",
	}
	afkCmdTableHeader = []string{
		"Command",
		"Alias",
		"Parameter",
		"Description",
	}
)

const (
	ReadMePath = "../../README.md"
	JsonPath   = "../../OkayegTeaTime/Resources/Commands.json"
	Title      = "OkayegTeaTime"
	Header1    = "Commands"
	Header2    = "AFK-Commands"
)

func main() {
	jsonController := Json.New(JsonPath)
	jsonController.LoadData()
	readMe := GenerateReadMe(jsonController)
	WriteToFile(readMe)
	fmt.Println("Generated README.md")
}

func WriteToFile(input string) {
	err := os.WriteFile(ReadMePath, []byte(input), os.ModePerm)
	if err != nil {
		fmt.Println(err)
	}
}

func GenerateReadMe(jsonController *Json.JsonController) string {
	builder := new(strings.Builder)
	builder.WriteString(fmt.Sprintf("<h1>%s</h1><h2>%s</h2>", Title, Header1))
	linq.From(header1Text).ForEachT(func(s string) {
		builder.WriteString(fmt.Sprintf("%s<br />", s))
	})
	builder.WriteString("<br /><table><tr>")
	linq.From(cmdTableHeader).ForEachT(func(s string) {
		builder.WriteString(fmt.Sprintf("<th>%s</th>", s))
	})
	builder.WriteString("<tr/>")
	linq.From(jsonController.JsonCommands.Commands).SortT(func(c1 Json.Command, c2 Json.Command) bool {
		return c1.CommandName < c2.CommandName
	}).ForEachT(func(c Json.Command) {
		if c.Document {
			builder.WriteString(fmt.Sprintf("<tr><td>%s</td><td><table>", c.CommandName))
			linq.From(c.Alias).ForEachT(func(a string) {
				builder.WriteString(fmt.Sprintf("<tr><td>%s</td></tr>", a))
			})
			builder.WriteString("</table></td><td><table>")
			for i := 0; i < len(c.Parameter); i++ {
				builder.WriteString(fmt.Sprintf("<tr><td>%s</td><td>%s</td></tr>", c.Parameter[i], c.Description[i]))
			}
			builder.WriteString("</table></td></tr>")
		}
	})
	builder.WriteString(fmt.Sprintf("</table><h2>%s</h2><table><tr>", Header2))
	linq.From(afkCmdTableHeader).ForEachT(func(s string) {
		builder.WriteString(fmt.Sprintf("<th>%s</th>", s))
	})
	builder.WriteString("</tr>")
	linq.From(jsonController.JsonCommands.AfkCommands).SortT(func(c1 Json.AfkCommand, c2 Json.AfkCommand) bool {
		return c1.CommandName < c2.CommandName
	}).ForEachT(func(c Json.AfkCommand) {
		builder.WriteString(fmt.Sprintf("<tr><td>%s</td><td><table>", c.CommandName))
		linq.From(c.Alias).ForEachT(func(s string) {
			builder.WriteString(fmt.Sprintf("<tr><td>%s</td></tr>", s))
		})
		builder.WriteString("</table></td>")
		for i := 0; i < len(c.Parameter); i++ {
			builder.WriteString(fmt.Sprintf("<td>%s</td><td>%s</td>", c.Parameter[i], c.Description[i]))
		}
		builder.WriteString("</td></tr>")
	})
	builder.WriteString("</table>")
	return builder.String()
}
