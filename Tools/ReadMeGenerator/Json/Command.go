package Json

type Command struct {
	Name        string
	Alias       []string
	Parameter   []string
	Description []string
	Cooldown    int
	Document    bool
}
