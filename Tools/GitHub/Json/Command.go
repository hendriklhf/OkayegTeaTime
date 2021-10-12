package Json

type Command struct {
	CommandName string
	Alias       []string
	Parameter   []string
	Description []string
	Cooldown    int
}
