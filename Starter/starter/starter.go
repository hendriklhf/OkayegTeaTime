package starter

import (
	"fmt"
	"os/exec"
	"strings"
	"time"

	linq "github.com/ahmetb/go-linq"
	ps "github.com/mitchellh/go-ps"
)

const _interval = time.Second * 15

type Starter struct {
	_programPath string
}

func New(programPath string) *Starter {
	return &Starter{
		_programPath: programPath,
	}
}

func (starter *Starter) Initialize() {
	for range time.Tick(_interval) {
		programs := starter.GetPrograms()
		running := starter.IsProgramRunning(programs)
		if !running {
			starter.StartProgram()
		} else {
			fmt.Println("Program running")
		}
	}
}

func (starter *Starter) GetPrograms() []string {
	processes, err := ps.Processes()
	var result []string
	if err == nil {
		linq.From(processes).SelectT(func(p ps.Process) string {
			return p.Executable()
		}).ToSlice(&result)
	}
	return result
}

func (starter *Starter) IsProgramRunning(programs []string) bool {
	return linq.From(programs).AnyWithT(func(p string) bool {
		return strings.HasPrefix(p, "OkayegTeaTime")
	})
}

func (starter *Starter) StartProgram() {
	cmd := exec.Command(starter._programPath)
	go cmd.Start()
	fmt.Println("Program started")
}
