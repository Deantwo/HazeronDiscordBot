using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Xml;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;

namespace HazeronDiscordBot
{
    class Program
    {
        const string SETTINGS = "HazeronDiscordBot.xml";
        private HazeronDiscordBotSettings _settingsXml;

        const string URL_PLAYERSON = @"http://hazeron.com/playerson.html";
        const string URL_PLAYERHISTORY = @"http://hazeron.com/playerhistory.txt";
        const string URL_GALACTIC = @"http://hazeron.com/galactic.html";

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

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
            _settingsXml = HazeronDiscordBotSettings.LoadOrCreate(SETTINGS);
            string botToken = _settingsXml.BotToken;

            // IF 'BotToken' is null or empty, give error and exit.
            if (string.IsNullOrEmpty(botToken))
            {
                Console.WriteLine($"The 'BotToken' setting is null or empty.");
                Console.WriteLine($"Set the value in the '{SETTINGS}' file.");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }
            
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            // Event subscribtions.
            _client.Log += Log;

            await RegisterCommandsAsync();

            // Login with bot token.
            await _client.LoginAsync(Discord.TokenType.Bot, botToken);

            await _client.StartAsync();

            // Wait forever.
            await Task.Delay(-1);
        }

        /// <summary>
        /// Register commands from the command module.
        /// </summary>
        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandsAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// Handle messages received by the bot and check them for commands.
        /// </summary>
        /// <param name="arg"></param>
        private async Task HandleCommandsAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message == null || message.Author.IsBot) return;

            int argPos = 0;

            if (message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                    WriteLog(result.ErrorReason);
            }
        }

        /// <summary>
        /// Handle Log events from the client.
        /// </summary>
        /// <param name="arg"></param>
        private Task Log(LogMessage arg)
        {
            WriteLog(arg.ToString());

            // Return non-async method.
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
    }
}
