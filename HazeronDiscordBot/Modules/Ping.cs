using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HazeronDiscordBot.Modules
{
    public class Haxus : ModuleBase<SocketCommandContext>
    {
        [Command("Haxus")]
        public async Task PingAsync()
        {
            await ReplyAsync("pong");
        }
    }
}
