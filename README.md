<h1>OkayegTeaTime</h1>

<h2>Commands</h2>

If your channel has a custom prefix set, commands will have to start with the prefix. If your channel has no prefix set,
commands will have to end with "eg", for example: "pingeg".
<br /><br />
Text in "[ ]" is a variable parameter

<table>
    <tr>
        <th>Command</th>
        <th>Alias</th>
        <th>Description [Parameter | Output]</th>
    </tr>
    <tr>
        <td>help</td>
        <td>
            <table>
                <tr>
                    <td>commands</td>
                </tr>
            </table>
        </td>
        <td>
            <table>
                <tr>
                    <td>(none)</td>
                    <td>Posts a link to this page</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>ping</td>
        <td>
            <table>
                <tr>
                    <td>pong</td>
                </tr>
            </table>
        </td>
        <td>
            <table>
                <tr>
                    <td>(none)</td>
                    <td>Sends a ping message, if the bot is online</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>rand</td>
        <td>
            <table>
                <tr>
                    <td>rl</td>
                </tr>
                <tr>
                    <td>randomline</td>
                </tr>
            </table>
        </td>
        <td>
            <table>
                <tr>
                    <td>(none)</td>
                    <td>Sends a random message of any user in the current channel</td>
                </tr>
                <tr>
                    <td>[username]</td>
                    <td>Sends a random message sent across all channels <br /> of the given user</td>
                </tr>
                <tr>
                    <td>[username] #[channel]</td>
                    <td>Sends a random message sent in the given channel <br /> of the given user</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>first</td>
        <td>
            <table>
                <tr>
                    <td>fl</td>
                </tr>
                <tr>
                    <td>firstline</td>
                </tr>
            </table>
        </td>
        <td>
            <table>
                <tr>
                    <td>(none)</td>
                    <td>Sends the first logged message of yourself</td>
                </tr>
                <tr>
                    <td>[username]</td>
                    <td>Sends the first logged message of the given user</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>last</td>
        <td>
            <table>
                <tr>
                    <td>ll</td>
                </tr>
                <tr>
                    <td>lastline</td>
                </tr>
                <tr>
                    <td>stalk</td>
                </tr>
            </table>
        </td>
        <td>
            <table>
                <tr>
                    <td>[username]</td>
                    <td>Sends the last logged message of the given user</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>search</td>
        <td>
            <table>
                <tr>
                    <td>find</td>
                </tr>
            </table>
        </td>
        <td>
            <table>
                <tr>
                    <td>[text]</td>
                    <td>Sends a message that includes the given text.<br />Accepts parameter like: u:[username] or
                        c:[channel]</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>cookie</td>
        <td>
            <table>
                <tr>
                    <td>keks</td>
                </tr>
            </table>
        </td>
        <td>
            <table>
                <tr>
                    <td>(none)</td>
                    <td>Sends a friendly fortune cookie message (in German) :)</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>count</td>
        <td>(none)</td>
        <td>
            <table>
                <tr>
                    <td>(none)</td>
                    <td>Sends the amount of logged messages across all channels</td>
                </tr>
                <tr>
                    <td>[username]</td>
                    <td>Sends the amount of logged messages sent by the given user</td>
                </tr>
                <tr>
                    <td>#[channel]</td>
                    <td>Sends the amount of logged messages in the given channel</td>
                </tr>
                <tr>
                    <td>e [emote]</td>
                    <td>Sends the amount of usages of the given emote</td>
                </tr>
                <tr>
                    <td>-users</td>
                    <td>Sends the amount of distinct logged users</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>suggest</td>
        <td>
            <table>
                <tr>
                    <td>bug</td>
                </tr>
            </table>
        </td>
        <td>
            <table>
                <tr>
                    <td>[text]</td>
                    <td>Suggest a new feature, improvement or report a bug</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>fuck</td>
        <td>(none)</td>
        <td>
            <table>
                <tr>
                    <td>[username]</td>
                    <td>Fucks the given user</td>
                </tr>
                <tr>
                    <td>[username] [emote]</td>
                    <td>Fucks the given user and adds a given emote</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>remind</td>
        <td>
            <table>
                <tr>
                    <td>notify</td>
                </tr>
            </table>
        </td>
        <td>
            <table>
                <tr>
                    <td>[username] [text]</td>
                    <td>Sets a reminder for the given user</td>
                </tr>
                <tr>
                    <td>[username] in [time] [text]</td>
                    <td>Sets a reminder for the given user, that will be triggered <br /> after the given time</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>emote</td>
        <td>(none)</td>
        <td>
            <table>
                <tr>
                    <td>ffz</td>
                    <td>Sends three of the recently added ffz emotes</td>
                </tr>
                <tr>
                    <td>ffz [number]</td>
                    <td>Sends a given amount of recently added ffz emotes</td>
                </tr>
                <tr>
                    <td>bttv</td>
                    <td>Sends three of the recently added ffz emotes</td>
                </tr>
                <tr>
                    <td>bttv [number]</td>
                    <td>Sends a given amount of recently added bttv emotes</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>check</td>
        <td>(none)</td>
        <td>
            <table>
                <tr>
                    <td>afk [username]</td>
                    <td>Checks the afk status of the given user</td>
                </tr>
                <tr>
                    <td>egs</td>
                    <td>Checks how many egs you possess</td>
                </tr>
                <tr>
                    <td>egs [username]</td>
                    <td>Checks how many egs the given user possesses</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>yourmom</td>
        <td>(none)</td>
        <td>
            <table>
                <tr>
                    <td>(none)</td>
                    <td>Sends a "your mom" joke</td>
                </tr>
                <tr>
                    <td>[username]</td>
                    <td>Sends a "your mom" joke to the given user</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>gachi</td>
        <td>(none)</td>
        <td>
            <table>
                <tr>
                    <td>(none)</td>
                    <td>Sends a random gachi song</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>cytube</td>
        <td>(none)</td>
        <td>
            <table>
                <tr>
                    <td>(none)</td>
                    <td>Sends the current song playing in the channel's cytube room, if it is set</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>nuke</td>
        <td>(none)</td>
        <td>
            <table>
                <tr>
                    <td>[word] [timeout time] [duration]</td>
                    <td>Will timeout every user for the given timeout time <br /> for the given time</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>set</td>
        <td>(none)</td>
        <td>
            <table>
                <tr>
                    <td>prefix [prefix]</td>
                    <td>Will set the channel's command prefix to the given prefix. <br /> A prefix can be as long as 10
                        chars</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>unset</td>
        <td>(none)</td>
        <td>
            <table>
                <tr>
                    <td>prefix</td>
                    <td>Will set the channel's prefix to nothing</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>math</td>
        <td>
            <table>
                <tr>
                    <td>calc</td>
                </tr>
            </table>
        </td>
        <td>
            <table>
                <tr>
                    <td>[mathematical expression]</td>
                    <td>Will send the solution of the given mathematical <br /> expression</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>coinflip</td>
        <td>
            <table>
                <tr>
                    <td>coin</td>
                </tr>
                <tr>
                    <td>cf</td>
                </tr>
                <tr>
                    <td>8ball</td>
                </tr>
            </table>
        </td>
        <td>
            <table>
                <tr>
                    <td>(none)</td>
                    <td>Sends the result of coinflip</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>spotify</td>
        <td>
            <table>
                <tr>
                    <td>song</td>
                </tr>
            </table>
        </td>
        <td>
            <table>
                <tr>
                    <td>(none)</td>
                    <td>Sends the current playing spotify song of the broadcaster, if registered</td>
                </tr>
                <tr>
                    <td>me</td>
                    <td>Sends your current playing spotify song, if registered</td>
                </tr>
                <tr>
                    <td>[username]</td>
                    <td>Sends the current playing spotify song of the given user, if registered</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>chatters</td>
        <td>
            <table>
                <tr>
                    <td>chattercount</td>
                </tr>
                <tr>
                    <td>cc</td>
                </tr>
                <tr>
                    <td>offlinechatter</td>
                </tr>
                <tr>
                    <td>oc</td>
                </tr>
                <tr>
                    <td>offlineviewer</td>
                </tr>
                <tr>
                    <td>ov</td>
                </tr>
            </table>
        </td>
        <td>
            <table>
                <tr>
                    <td>(none)</td>
                    <td>Sends the amount of chatters in the current channel</td>
                </tr>
                <tr>
                    <td>#[channel]</td>
                    <td>Sends the amount of chatters in the given channel</td>
                </tr>
            </table>
        </td>
    </tr>
</table>

<h2>AFK-Commands</h2>

<table>
    <tr>
        <th>Command</th>
        <th>Alias</th>
        <th>Parameter</th>
        <th>Description</th>
    </tr>
    <tr>
        <td>rafk</td>
        <td>
            <table>
                <tr>
                    <td>cafk</td>
                </tr>
            </table>
        </td>
        <td>(none)</td>
        <td>Resumes your last status</td>
    </tr>
    <tr>
        <td>gn</td>
        <td>
            <table>
                <tr>
                    <td>sleep</td>
                </tr>
            </table>
        </td>
        <td>[message]</td>
        <td>Sets you status to "sleeping"</td>
    </tr>
    <tr>
        <td>afk</td>
        <td>(none)</td>
        <td>[message]</td>
        <td>Sets your status to "afk"</td>
    </tr>
    <tr>
        <td>brb</td>
        <td>(none)</td>
        <td>[message]</td>
        <td>Sets your status to "brb"</td>
    </tr>
    <tr>
        <td>fat</td>
        <td>
            <table>
                <tr>
                    <td>food</td>
                </tr>
            </table>
        </td>
        <td>[message]</td>
        <td>Sets your status to "eating"</td>
    </tr>
    <tr>
        <td>lurk</td>
        <td>(none)</td>
        <td>[message]</td>
        <td>Sets your status to "lurking"</td>
    </tr>
    <tr>
        <td>shower</td>
        <td>(none)</td>
        <td>[message]</td>
        <td>Sets your status to "showering"</td>
    </tr>
    <tr>
        <td>nap</td>
        <td>(none)</td>
        <td>[message]</td>
        <td>Sets your status to "napping"</td>
    </tr>
    <tr>
        <td>study</td>
        <td>(none)</td>
        <td>[message]</td>
        <td>Sets your status to "studying"</td>
    </tr>
    <tr>
        <td>work</td>
        <td>(none)</td>
        <td>[message]</td>
        <td>Sets your status to "working"</td>
    </tr>
    <tr>
        <td>school</td>
        <td>(none)</td>
        <td>[message]</td>
        <td>Sets your status to "at school"</td>
    </tr>
    <tr>
        <td>poof</td>
        <td>(none)</td>
        <td>[message]</td>
        <td>Sets your status to "poofed away"</td>
    </tr>
    <tr>
        <td>game</td>
        <td>
            <table>
                <tr>
                    <td>gaming</td>
                </tr>
            </table>
        </td>
        <td>[message]</td>
        <td>Sets your status to "gaming"</td>
    </tr>
    <tr>
        <td>movie</td>
        <td>
            <table>
                <tr>
                    <td>film</td>
                </tr>
            </table>
        </td>
        <td>[message]</td>
        <td>Sets your status to "watching a movie"</td>
    </tr>
</table>