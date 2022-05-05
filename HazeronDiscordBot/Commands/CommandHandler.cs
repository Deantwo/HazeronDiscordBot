using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace HazeronDiscordBot
{
    /// <summary>
    /// This is a slightly altered version of the suggested standard "CommandHandler" class.
    /// See: https://www.gngrninja.com/code/2019/4/1/c-discord-bot-command-handling
    /// </summary>
    public class CommandHandler
    {
        private DiscordSocketClient _botClient;
        private InteractionService _botService;

        public CommandHandler(DiscordSocketClient botClient, InteractionService botService)
        {
            _botClient = botClient;
            _botService = botService;

            // add the public modules that inherit InteractionModuleBase<T> to the InteractionService
            //var test = _botService.AddModulesAsync(System.Reflection.Assembly.GetEntryAssembly(), null);
            var test = _botService.AddModuleAsync<HazeronDiscordBot.BotCommands>(null);

            // process the InteractionCreated payloads to execute Interactions commands
            _botClient.InteractionCreated += HandleInteraction;

            // process the command execution results
            _botService.SlashCommandExecuted += SlashCommandExecuted;
            _botService.ContextCommandExecuted += ContextCommandExecuted;
            _botService.ComponentCommandExecuted += ComponentCommandExecuted;
        }

        #region CommandExecuted
        private async Task ComponentCommandExecuted(ComponentCommandInfo arg1, Discord.IInteractionContext arg2, Discord.Interactions.IResult arg3)
        {
            await CommonCommandExecuted(arg2, arg3);
        }

        private async Task ContextCommandExecuted(ContextCommandInfo arg1, Discord.IInteractionContext arg2, Discord.Interactions.IResult arg3)
        {
            await CommonCommandExecuted(arg2, arg3);
        }

        private async Task SlashCommandExecuted(SlashCommandInfo arg1, Discord.IInteractionContext arg2, Discord.Interactions.IResult arg3)
        {
            await CommonCommandExecuted(arg2, arg3);
        }

        private async Task CommonCommandExecuted(Discord.IInteractionContext arg2, Discord.Interactions.IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        await arg2.Interaction.RespondAsync($"Unmet precondition. {arg3.ErrorReason}", ephemeral: true);
                        break;
                    case InteractionCommandError.UnknownCommand:
                        await arg2.Interaction.RespondAsync($"Unknown command. {arg3.ErrorReason}", ephemeral: true);
                        break;
                    case InteractionCommandError.BadArgs:
                        await arg2.Interaction.RespondAsync($"Bad args. {arg3.ErrorReason}", ephemeral: true);
                        break;
                    case InteractionCommandError.Exception:
                        await arg2.Interaction.RespondAsync($"Exception. {arg3.ErrorReason}", ephemeral: true);
                        break;
                    case InteractionCommandError.Unsuccessful:
                        await arg2.Interaction.RespondAsync($"Unsuccessful. {arg3.ErrorReason}", ephemeral: true);
                        break;
                    default:
                        await arg2.Interaction.RespondAsync($"Unknown error. {arg3.ErrorReason}", ephemeral: true);
                        break;
                }
            }
        }
        #endregion

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                // create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
                var ctx = new SocketInteractionContext(_botClient, arg);
                var result = await _botService.ExecuteCommandAsync(ctx, null);
                if (arg.Type == InteractionType.ApplicationCommand)
                    Console.WriteLine($"!!!!! {arg.User} used the \"/{(arg as SocketCommandBase)?.CommandName}\" command");
                else if (arg.Type == InteractionType.MessageComponent)
                    Console.WriteLine($"!!!!! {arg.User} pressed the \"{(arg as SocketMessageComponent)?.Data?.CustomId}\" button");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // if a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (arg.Type == InteractionType.ApplicationCommand)
                {
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
                }
            }
        }
    }
}
