using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using HazeronDiscordBot.XmlDatabase;

namespace HazeronDiscordBot
{
    public class BotCommands : InteractionModuleBase<SocketInteractionContext>
    {
        #region Test
        //[Group("test", "Simple test commands.")]
        [SlashCommand("hail", "Check if the bot is active.")]
        public async Task HailAsync()
        {
            ComponentBuilder compBuilder = new ComponentBuilder();
            compBuilder.WithButton("Announce", customId: "annouce");
            await RespondAsync($"Hailing the Discord. Is anybody out there?", ephemeral: true, components: compBuilder.Build());
        }

        [SlashCommand("annouce", "Check if the bot is active.")]
        [ComponentInteraction("annouce")]
        public async Task AnnounceAsync()
        {
            await RespondAsync($"<@473618908390096916> here. I am watching for commands.", ephemeral: true);
        }
        #endregion

        #region Info
        [SlashCommand("info", "Get information about me.")]
        public async Task InfoAsync()
        {
            string message = $"<@473618908390096916> is created by <@192345831989444608>.{Environment.NewLine}" +
                $"The code is open-source and can be found here:{Environment.NewLine}" +
                $"https://github.com/Deantwo/HazeronDiscordBot";
            await RespondAsync(message, ephemeral: true);
        }
        #endregion

        #region Galactic
        [SlashCommand("galactic", "Setup or change settings or galactic chat.")]
        [RequireUserPermission(GuildPermission.ManageChannels | GuildPermission.ManageWebhooks)]
        [RequireBotPermission(GuildPermission.ManageWebhooks)]
        public async Task GalacticAsync()
        {
            HazeronDiscordBotDatabaseServer server = StaticXmlDatabase.GetServer(this.Context.Guild.Id);
            Discord.Rest.RestWebhook webhook = await GetWebhook(this.Context);

            string message;
            if (server is object && server.HasWebhook && webhook is object)
                message = $"Galactic forwarding is **active**.{Environment.NewLine}" +
                          $"Channel: <#{webhook.ChannelId}>";
            else
                message = $"Galactic forwarding is **disabled**.";

            // Create buttons.
            ComponentBuilder compBuilder = new ComponentBuilder();
            compBuilder.WithButton("Setup here", "galactic-setup", ButtonStyle.Danger);
            compBuilder.WithButton("Reset", "galactic-reset", ButtonStyle.Danger);
            compBuilder.WithButton("Disable", "galactic-delete", ButtonStyle.Danger);

            await RespondAsync(message, ephemeral: true, components: compBuilder.Build());
        }

        [ComponentInteraction("galactic-setup")]
        [RequireUserPermission(GuildPermission.ManageChannels | GuildPermission.ManageWebhooks)]
        [RequireBotPermission(GuildPermission.ManageWebhooks)]
        [RequireContext(ContextType.Guild)]
        public async Task GalacticSetupAsync()
        {
            HazeronDiscordBotDatabaseServer server = StaticXmlDatabase.GetServer(this.Context.Guild.Id);
            IWebhook webhook = await GetWebhook(this.Context);

            string message;
            if (server is object && webhook is object)
            {
                if (webhook.ChannelId == this.Context.Channel.Id)
                    message = $"Galactic forwarding to already configured to this channel, no change needed.{Environment.NewLine}" +
                              $"Channel: <#{this.Context.Channel.Id}>";
                else
                {
                    // Update the webhook.
                    await webhook.ModifyAsync(x => { x.ChannelId = this.Context.Channel.Id; });

                    // Update the database's server webhook entry.
                    server.WebhookId = webhook.Id;
                    server.WebhookToken = webhook.Token;
                    StaticXmlDatabase.Save();

                    message = $"Changing Galactic forwarding to this channel.{Environment.NewLine}" +
                              $"Channel: <#{this.Context.Channel.Id}>";
                }
            }
            else
            {
                // Create or update the webhook.
                if (webhook is null)
                    webhook = await ((ITextChannel)this.Context.Channel).CreateWebhookAsync("HazeronBot");
                else
                    await webhook.ModifyAsync(x => { x.ChannelId = this.Context.Channel.Id; });

                // Update the database's server webhook entry. Create server on the database if it doesn't exist.
                if (server is null)
                    server = StaticXmlDatabase.AddServer(new HazeronDiscordBotDatabaseServer(this.Context.Guild.Id));
                server.WebhookId = webhook.Id;
                server.WebhookToken = webhook.Token;
                StaticXmlDatabase.Save();

                message = $"Created Galactic forwarding to this channel.{Environment.NewLine}" +
                          $"Channel: <#{this.Context.Channel.Id}>";
            }

            await RespondAsync(message, ephemeral: true);
        }

        [ComponentInteraction("galactic-reset")]
        [RequireUserPermission(GuildPermission.ManageChannels | GuildPermission.ManageWebhooks)]
        [RequireBotPermission(GuildPermission.ManageWebhooks)]
        [RequireContext(ContextType.Guild)]
        public async Task GalacticResetAsync()
        {
            HazeronDiscordBotDatabaseServer server = StaticXmlDatabase.GetServer(this.Context.Guild.Id);

            string message;
            if (server is object && server.HasWebhook)
            {
                // Clear the hash of the last transmitted message.
                server.LastGalacticMessageHash = null;
                StaticXmlDatabase.Save();

                message = $"Galactic forwarding has been reset.";
            }
            else
                message = $"Galactic forwarding is not currently active.";

            await RespondAsync(message, ephemeral: true);
        }

        [ComponentInteraction("galactic-delete")]
        [RequireUserPermission(GuildPermission.ManageChannels | GuildPermission.ManageWebhooks)]
        [RequireBotPermission(GuildPermission.ManageWebhooks)]
        [RequireContext(ContextType.Guild)]
        public async Task GalacticDeleteAsync()
        {
            HazeronDiscordBotDatabaseServer server = StaticXmlDatabase.GetServer(this.Context.Guild.Id);
            Discord.Rest.RestWebhook webhook = await GetWebhook(this.Context);

            string message;
            if (webhook is object || (server is object && server.HasWebhook))
            {
                // Delete the webhook.
                if (webhook is object)
                    await webhook.DeleteAsync();

                // Clear the database's webhook entry.
                if (server is object && server.HasWebhook)
                {
                    server.WebhookId = 0;
                    server.WebhookToken = null;
                    StaticXmlDatabase.Save();
                }

                message = $"Galactic forwarding has been disabled.";
            }
            else
                message = $"Galactic forwarding is already not active.";

            await RespondAsync(message, ephemeral: true);
        }
        #endregion

        #region Roles
        //[Group("role", "Commands related to automatic role assignment.")]
        [SlashCommand("show", "Show roles in the database.")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireContext(ContextType.Guild)]
        public async Task RoleShowAsync()
        {
            HazeronDiscordBotDatabaseServer server = StaticXmlDatabase.GetServer(this.Context.Guild.Id);
            string message;
            if (server is object && server.Roles is object && server.Roles.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Role buttons:");
                foreach (HazeronDiscordBotDatabaseServerRole role in server.Roles)
                    sb.AppendLine($"<@&{role.RoleId}> `{role.ButtonText}`");

                message = sb.ToString();
            }
            else
                message = $"No roles configured.";
            await RespondAsync(message, ephemeral: true);
        }

        [SlashCommand("add", "Add a role to the database.")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireContext(ContextType.Guild)]
        public async Task AddRoleAsync(string role, string buttonText)
        {
            HazeronDiscordBotDatabaseServer server = StaticXmlDatabase.GetServer(this.Context.Guild.Id);
            string message;
            if (server is null)
                server = StaticXmlDatabase.AddServer(new HazeronDiscordBotDatabaseServer(this.Context.Guild.Id));
            role = role.Trim();
            if (role.StartsWith("<@&") && role.EndsWith('>'))
                role = role.Substring(3, role.Length - 4);
            if (ulong.TryParse(role, out ulong roleId))
            {
                server.Roles.Add(new HazeronDiscordBotDatabaseServerRole(roleId, buttonText));
                StaticXmlDatabase.Save();
                message = $"Created <@&{roleId}> as a role option with the button text \"{buttonText}\".";
            }
            else
                message = $"The {nameof(role)} parameter was invalid.";
            await RespondAsync(message, ephemeral: true);
        }
        #endregion

        #region Helper Methods
        private async Task<Discord.Rest.RestWebhook> GetWebhook(SocketInteractionContext context)
        {
            IReadOnlyCollection<Discord.Rest.RestWebhook> webhooks = await context.Guild.GetWebhooksAsync();
            Discord.Rest.RestWebhook webhook = null;
            foreach (Discord.Rest.RestWebhook wh in webhooks)
                if (wh.Name == "HazeronBot")
                    webhook = wh;
            return webhook;
        }
        #endregion
    }
}
