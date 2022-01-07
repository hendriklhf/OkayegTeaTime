package main

import (
	"fmt"
	"os"
	"os/exec"
	"regexp"
)

var (
	runtimes = []string{
		"win-x64",
		"linux-arm",
		"linux-x64",
		"osx-x64",
	}
	pattern = []*regexp.Regexp{
		regexp.MustCompile(`(?i)^win(dows)?(-?x64)?$`),
		regexp.MustCompile(`(?i)^((linux-?)?arm)|((raspberry)?pi)$`),
		regexp.MustCompile(`(?i)^linux(-?x64)?$`),
		regexp.MustCompile(`(?i)^((osx)|(mac(-?os)?)(-?x64)?)$`),
	}
	allRegex = regexp.MustCompile(`(?i)^all$`)
	params   = []string{
		"publish",
		"-o",
		".%sBuild%s%s",
		"-c",
		"Release",
		"-r",
		"%s",
		"-p:PublishSingleFile=true",
		"--self-contained",
		"true",
		".%sOkayegTeaTime%sOkayegTeaTime.csproj",
	}
)

const (
	CMD = "dotnet"
)

func main() {
	rts := GetRuntimes()
	if Count(rts) == 0 {
		fmt.Println("No valid publish arguments given! Exiting...")
		os.Exit(0)
	}
	RunCommands(rts)
}

func GetRuntimes() []string {
	result := make([]string, len(runtimes))
	args := os.Args[1:]

	if Any(args, func(s string) bool { return allRegex.MatchString(s) }) {
		for i := 0; i < len(runtimes); i++ {
			result[i] = runtimes[i]
		}
		return result
	}

	for i, reg := range pattern {
		if Any(args, func(s string) bool { return reg.MatchString(s) }) {
			result[i] = runtimes[i]
		}
	}

	return result
}

func RunCommands(rts []string) {
	for _, r := range rts {
		if len(r) == 0 {
			continue
		}
		sep := string(os.PathSeparator)
		param2 := fmt.Sprintf(params[2], sep, sep, r)
		param7 := fmt.Sprintf(params[6], r)
		param11 := fmt.Sprintf(params[10], sep, sep)

		cmd := exec.Command(CMD, params[0], params[1], param2, params[3], params[4], params[5], param7, params[7], params[8], params[9], param11)
		cmd.Stdout = os.Stdout
		cmd.Stderr = os.Stderr
		cmd.Stdin = os.Stdin

		err := cmd.Run()
		if err != nil {
			fmt.Println(err)
		}
	}
}

func Count(arr []string) int {
	var result int
	for _, s := range arr {
		if len(s) > 0 {
			result++
		}
	}
	return result
}

func Any(arr []string, condition func(string) bool) bool {
	for _, s := range arr {
		if condition(s) {
			return true
		}
	}
	return false
}
