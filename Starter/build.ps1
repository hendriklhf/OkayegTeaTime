go env -w GOOS=linux GOARCH=arm
go build -o ..\OkayegTeaTimeCSharp\bin\Publish\LinuxArm
go env -w GOOS=windows GOARCH=amd64