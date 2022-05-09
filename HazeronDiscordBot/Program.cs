using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Reflection;

using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Discord.Webhook;

using HazeronDiscordBot.XmlDatabase;

namespace HazeronDiscordBot
{
    class Program
    {
        private HazeronDiscordBotSettings _settings;

        private DiscordSocketClient _botClient;
        private InteractionService _botService;
        private bool _initilized = false;

        private CommandHandler _commandHandler;

        private Timer _timer;
        const int TIMER_SECOND_INTERVAL = 2;

        private bool _galacticLock = false;

        // Start the program async.
        static void Main(string[] args) => new Program()
            .RunBotAsync()
            .GetAwaiter()
            .GetResult();

        /// <summary>
        /// Bot running method.
        /// </summary>
        public async Task RunBotAsync()
        {
            // Load settings.
            string settingsFile = Path.ChangeExtension(System.Diagnostics.Process.GetCurrentProcess().ProcessName, "xml");
            _settings = HazeronDiscordBotSettings.LoadOrCreate(settingsFile);
            string botToken = _settings.BotToken;
            string databaseFile = _settings.DatabaseXmlPath;
            // Load database.
            StaticXmlDatabase.Load(databaseFile);

            // IF 'BotToken' is null or empty, give error and exit.
            if (string.IsNullOrEmpty(botToken))
            {
                Console.WriteLine($"The '{nameof(HazeronDiscordBotSettings.BotToken)}' setting is null or empty.");
                Console.WriteLine("Set the value in the settings file.");
                Console.WriteLine($"'{settingsFile}'");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }

            DiscordSocketConfig intent = new DiscordSocketConfig()
            {
                AlwaysDownloadDefaultStickers = false,
                AlwaysDownloadUsers = false,
                AlwaysResolveStickers = false,
                GatewayIntents = GatewayIntents.GuildWebhooks
                               | GatewayIntents.GuildIntegrations
                               | GatewayIntents.GuildMessages
                               | GatewayIntents.DirectMessages
                               | GatewayIntents.Guilds
            };
            _botClient = new DiscordSocketClient(intent);
            _botService = new InteractionService(_botClient);

            // Event subscribtions.
            _botClient.Log += LogAsync;
            _botClient.Ready += ReadyAsync;
            _botService.Log += LogAsync;

            // we get the CommandHandler class here and call the InitializeAsync method to start things up for the CommandHandler service
            _commandHandler = new CommandHandler(_botClient, _botService);

            // Login with bot token and start the conenction.
            await _botClient.LoginAsync(Discord.TokenType.Bot, botToken);
            await _botClient.StartAsync();

            // Start timer rutine.
            {
                Console.WriteLine($"=<>= Starting timer rutine on a {TIMER_SECOND_INTERVAL} second interval.");
                // Timer setup.
                _timer = new Timer();
                _timer.Elapsed += OnTimedEvent;
                _timer.Interval = 1000 * TIMER_SECOND_INTERVAL;
                _timer.Enabled = true;
            }

            // Set the bot's activity.
            await _botClient.SetActivityAsync(new Game("the Galactic communication channel", ActivityType.Listening));

            // Wait forever.
            await Task.Delay(-1);
        }

        /// <summary>
        /// The periodically called method used for constant monitoring or other rutines.
        /// </summary>
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            GalacticChat();
        }

        #region Galatic Chat Rutine
        /// <summary>
        /// The main GalacticForwarding function.
        /// </summary>
        private void GalacticChat()
        {
            if (_galacticLock)
                return;
            _galacticLock = true;

            try
            {
                List<HazeronDiscordBotDatabaseServer> servers = StaticXmlDatabase.GetServers().FindAll(x => x.HasWebhook);
                if (servers.Count == 0)
                    return;

                // Read the galactic messages from the website.
                ICollection<GalacticMessage> galacticMessages = GalacticMessage.GetMessages(_settings.Advanced.UrlGalactic);
                // If page hasn't updated there will be no messages.
                if (galacticMessages is null || galacticMessages.Count == 0)
                    return;

                foreach (HazeronDiscordBotDatabaseServer server in servers)
                {
                    try
                    {
                        // Create WebhookClient for the server if it does not already exist.
                        DiscordWebhookClient webhookClient = new DiscordWebhookClient(server.WebhookId, server.WebhookToken);

                        // Check each message to see if they matches the last recorded message.
                        List<GalacticMessage> unsentGalacticMessages = new List<GalacticMessage>();
                        foreach (GalacticMessage galacticMessage in galacticMessages)
                        {
                            if (galacticMessage.ToHash() == server.LastGalacticMessageHash)
                                unsentGalacticMessages.Clear();
                            else
                                unsentGalacticMessages.Add(galacticMessage);
                        }

                        // If all messages are new, there is a high possibility that there were messages lost since last checked.
                        if (galacticMessages.Count == 16 && galacticMessages.Count == unsentGalacticMessages.Count)
                        {
                            Console.WriteLine($"=<>= Server {server.ServerId} might have missed some galactic messages between updates or downtime.");
                            SendWebhookMessage(webhookClient, $"||Ť͢ẻ̇̈ḿ̃pͯ͂̈oͫ̉̀ȓaͭͥ̀l̎̾ͯ ͥs̓̂͗t̐ͬ̍a̎t͌̓ͥi̵cͩͦ̌! Some galactic messages might have been *lost*, and timestamps on following 16 messages might be wrong.||", "HazeronBot", HazeronPictures.Limbo, false);
                        }

                        // Transmit all unsent messages to the Discord webhook.
                        foreach (GalacticMessage unsentGalacticMessage in unsentGalacticMessages)
                        {
                            if (unsentGalacticMessage is SystemMessage)
                                SendWebhookMessage(webhookClient, $"**{EscapeDiscordFormatting(unsentGalacticMessage.Message)}**", "SYSTEM", HazeronPictures.SohLogo, false);
                            else
                            {
                                string avatar = HazeronPictures.GetGalaxyPicture(unsentGalacticMessage.Galaxy);
                                SendWebhookMessage(webhookClient, unsentGalacticMessage.Message, unsentGalacticMessage.Sender, avatar);
                            }

                            // Update the saved hash of the last galactic message that was sent.
                            server.LastGalacticMessageHash = unsentGalacticMessage.ToHash();
                            StaticXmlDatabase.Save();
                        }
                    }
                    catch (ArgumentException ex) when (ex.ParamName == "webhookToken")
                    {
                        Console.WriteLine($"=<>= Server {server.ServerId} webhook invalid, clearing database webhook entry.");
                        server.WebhookId = 0;
                        server.WebhookToken = null;
                        StaticXmlDatabase.Save();
                    }
                    catch (InvalidOperationException ex) when (ex.Message == "Could not find a webhook with the supplied credentials.")
                    {
                        Console.WriteLine($"=<>= Server {server.ServerId} webhook not found, clearing database webhook entry.");
                        server.WebhookId = 0;
                        server.WebhookToken = null;
                        StaticXmlDatabase.Save();
                    }
                    //catch (AggregateException ex)
                    //{
                    //    bool notUnknownWebhook = false;
                    //    foreach (Exception exx in ex.Flatten().InnerExceptions)
                    //    {
                    //        HttpException exxx = exx as HttpException;
                    //        if (exxx is object && exxx.Message == "The server responded with error 10015: Unknown Webhook")
                    //        {
                    //            Console.WriteLine($"=<>= Server {server.ServerId} webhook lost, clearing database webhook entry.");
                    //            server.WebhookId = 0;
                    //            server.WebhookToken = null;
                    //            StaticXmlDatabase.Save();
                    //        }
                    //        else
                    //            notUnknownWebhook = true;
                    //    }
                    //    if (notUnknownWebhook)
                    //        throw;
                    //}
                }
            }
            catch (OperationCanceledException ex) when (ex.Message == "The operation was canceled.")
            {
                // This is some HTTP stream error, ignore.
                Console.WriteLine($"=<>= OperationCanceledException");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=<>= Unknown error message:{Environment.NewLine}{ex}");
            }
            finally
            {
                _galacticLock = false;
            }
        }

        /// <summary>
        /// Send message to the Webhook's channel.
        /// </summary>
        /// <param name="username">Username to show as sender on the message.</param>
        /// <param name="message">Message to be sent.</param>
        /// <param name="avatar">Optional avatar url. Default use Webhook's avatar.</param>
        /// <param name="stripFormatting">Will escape all Discord formatting if left as true.</param>
        private void SendWebhookMessage(DiscordWebhookClient webhookClient, string message, string username = null, string avatar = null, bool stripFormatting = true)
        {
            if (stripFormatting)
                message = EscapeDiscordFormatting(message);
            webhookClient.SendMessageAsync(
                text: message,
                username: username,
                allowedMentions: AllowedMentions.None,
                flags: MessageFlags.SuppressEmbeds,
                avatarUrl: avatar
                ).Wait();
        }

        /// <summary>
        /// Escape all Discord formatting.
        /// </summary>
        /// <param name="message">The message to change.</param>
        /// <returns>The changed messaged.</returns>
        private string EscapeDiscordFormatting(string message)
        {
            string[] requiredEscapes = new string[]
            {
                "\\", // Escape backslashes. Important to have this one first.
                "*", // Escape bold and italic.
                "_", // Escape underscore and italic.
                "~~", // Escape the strikethrough.
                "||", // Escape the spoiler tag.
                "> ", // Escape quote blocks.
                "`", // Escape code tags and code boxes.
                ":" // Escape emoji codes and links.
            };
            foreach (string requiredEscape in requiredEscapes)
                message = message.Replace($"{requiredEscape}", $"\\{requiredEscape}");
            return message;
        }
        #endregion

        #region Ready
        private async Task ReadyAsync()
        {
            if (_initilized)
                return;

            Console.WriteLine($"=<>= Registering commands.");
            try
            {
                //// Delete all Guild-level commands.
                //foreach (SocketGuild guild in _botClient.Guilds)
                //{
                //    foreach (SocketApplicationCommand command in guild.GetApplicationCommandsAsync().Result)
                //    {
                //        await command.DeleteAsync();
                //    }
                //}

#if DEBUG
                // For testing it is better to use Guild-level command, since they are updated right away.
                ulong? testGuildId = StaticXmlSettings.Settings.Advanced.DeveloperTestServer;
                if (testGuildId.HasValue)
                {
                    Console.WriteLine($"=<>= In debug mode, registering commands to guild {testGuildId.Value}...");
                    await _botService.RegisterCommandsToGuildAsync(testGuildId.Value, true);
                }
#else
                // Register commands with Discord. This can take around an hour before it takes effect on all servers.
                await _botService.RegisterCommandsGloballyAsync(true);
#endif

                Console.WriteLine($"=<>= Commands registered.");
            }
            catch (HttpException exception)
            {
                Console.WriteLine($"=<>= Failed to register commands.");

                // If our command was invalid, we should catch an ApplicationCommandException.
                // This exception contains the path of the error as well as the error message.
                // You can serialize the Error field in the exception to get a visual of where your error is.
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(exception.Errors, Newtonsoft.Json.Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);

                Console.WriteLine($"=<>= Restart the program to attempt registering commands again.");
            }

            _initilized = true;
        }
        #endregion

        #region Logging
        /// <summary>
        /// Handle Log events from the client.
        /// </summary>
        /// <param name="message"></param>
        private Task LogAsync(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                WriteLog($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                    + $" failed to execute in {cmdException.Context.Channel}.");
                WriteLog(cmdException.ToString());
            }
            else
                WriteLog($"[General/{message.Severity}] {message}");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Write the message to multiple different outputs.
        /// </summary>
        /// <param name="message"></param>
        private void WriteLog(string message)
        {
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
        }
        #endregion
    }
}
