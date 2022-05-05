using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HazeronDiscordBot.XmlDatabase
{
    /// <summary>
    /// This is basically a static class to hold a singleton reference to the <see cref="HazeronDiscordBotDatabase"/> instance.<br/>
    /// Don't judge me too harshly for making it like this. This would be so much easier with a SQL database.<br/>
    /// Also since the XML file is single-threaded, it doesn't matter too much if this class is too.
    /// </summary>
    public static class StaticXmlDatabase
    {
        private static HazeronDiscordBotDatabase _database;

        private static string _path;

        /// <summary>
        /// Loads the settings XML file.
        /// </summary>
        /// <param name="filePath">File path of the settings XML file.</param>
        public static void Load(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            _path = path;
            Load();
        }
        /// <summary>
        /// Reloads the settings XML file.
        /// </summary>
        public static void Load()
        {
            if (string.IsNullOrEmpty(_path))
                throw new ArgumentNullException(nameof(_path));

            _database = HazeronDiscordBotDatabase.LoadOrCreate(_path);
        }

        /// <summary>
        /// Saves the settings XML file, creating it if it doesn't exist.
        /// </summary>
        public static void Save()
        {
            if (_database is null)
                throw new Exception($"No database loaded. Use the {nameof(Load)}(string) method.");

            _database.Save();
        }

        /// <summary>
        /// Get a list of all servers in the database.
        /// </summary>
        /// <returns>Returns a <see cref="List(HazeronDiscordBotDatabaseServer)"/> with the servers.</returns>
        public static List<HazeronDiscordBotDatabaseServer> GetServers()
        {
            if (_database is null)
                throw new Exception($"No database loaded. Use the {nameof(Load)} method.");

            if (_database.Servers is null)
                return new List<HazeronDiscordBotDatabaseServer>();

            return _database.Servers;
        }

        /// <summary>
        /// Get a specific server from the database. Returns null if not found.
        /// </summary>
        /// <returns>Returns a <see cref="HazeronDiscordBotDatabaseServer"/>.</returns>
        public static HazeronDiscordBotDatabaseServer GetServer(ulong serverId)
        {
            if (_database is null)
                throw new Exception($"No database loaded. Use the {nameof(Load)} method.");

            return _database.Servers.SingleOrDefault(x => x.ServerId == serverId);
        }
        
        /// <summary>
        /// Add a server to the database.
        /// </summary>
        /// <param name="server">The server to be added.</param>
        /// <returns>Returns the newly added server.</returns>
        public static HazeronDiscordBotDatabaseServer AddServer(HazeronDiscordBotDatabaseServer server)
        {
            if (_database is null)
                throw new Exception($"No database loaded. Use the {nameof(Load)} method.");

            if (_database.Servers.Any(x => x.ServerId == server.ServerId))
                throw new ArgumentException($"A server with the {nameof(server.ServerId)} of '{server.ServerId}' already exists.");

            _database.Servers.Add(server);
            return server;
        }
    }
}
