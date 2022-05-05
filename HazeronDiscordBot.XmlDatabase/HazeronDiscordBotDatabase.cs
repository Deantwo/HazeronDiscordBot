using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace HazeronDiscordBot.XmlDatabase
{
    /// <summary>
    /// This is a copy of how HazeronDiscordBotSettings work, and it isn't a pretty way of handling this.<br/>
    /// This should be replaced with a SQL database some day.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "HazeronDiscordBotDatabase", Namespace = "")]
    public class HazeronDiscordBotDatabase
    {
        #region Static Methods
        /// <summary>
        /// Loads the settings XML file.
        /// </summary>
        /// <param name="filePath">File path of the settings XML file.</param>
        /// <returns>Returns loaded <see cref="HazeronDiscordBotDatabase"/> object.</returns>
        public static HazeronDiscordBotDatabase Load(string filePath)
        {
            HazeronDiscordBotDatabase settings;
            XmlSerializer serializer = new XmlSerializer(typeof(HazeronDiscordBotDatabase));
            using (TextReader reader = new StreamReader(filePath))
                settings = (HazeronDiscordBotDatabase)serializer.Deserialize(reader);
            settings.FilePath = filePath;

            return settings;
        }

        /// <summary>
        /// Checks if a settings XML file with the name exist and loads it, if one does not exist it is created.
        /// </summary>
        /// <param name="filePath">File path of the settings XML file.</param>
        /// <returns>Returns loaded or created <see cref="HazeronDiscordBotSettings"/> object.</returns>
        public static HazeronDiscordBotDatabase LoadOrCreate(string filePath)
        {
            if (File.Exists(filePath))
                return Load(filePath);
            else
            {
                HazeronDiscordBotDatabase settings;
                settings = new HazeronDiscordBotDatabase();
                settings.FilePath = filePath;
                settings.Save();
                return settings;
            }
        }
        #endregion

        /// <summary>
        /// The filepath to this XML file.
        /// </summary>
        [XmlIgnore]
        public string FilePath { get; private set; }

        /// <summary>
        /// Lock object used to prevent multi-threaded code from saving the XML file at the same time.
        /// </summary>
        [XmlIgnore]
        private object _saveLock = new object();

        /// <summary>
        /// Version number used for future conversion needs.
        /// </summary>
        [XmlAttribute(AttributeName = "Version")]
        public int Version { get; set; }

        [XmlArray(ElementName = "Servers")]
        [XmlArrayItem(ElementName = "Server", Type = typeof(HazeronDiscordBotDatabaseServer))]
        public List<HazeronDiscordBotDatabaseServer> Servers { get; set; }

        public HazeronDiscordBotDatabase()
        {
            Version = 1;
            Servers = new List<HazeronDiscordBotDatabaseServer>();
        }

        #region Methods
        /// <summary>
        /// Saves the database XML file, creating it if it doesn't exist.
        /// </summary>
        public void Save()
        {
            Save(FilePath);
        }
        /// <summary>
        /// Saves the database XML file, creating it if it doesn't exist.
        /// </summary>
        /// <param name="filePath">File path of the database XML file.</param>
        public void Save(string filePath)
        {
            lock (_saveLock)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HazeronDiscordBotDatabase));
                using (TextWriter textWriter = new StreamWriter(filePath))
                    serializer.Serialize(textWriter, this);
            }
        }
        #endregion
    }

    public class HazeronDiscordBotDatabaseServer
    {
        /// <summary>
        /// The GuildId of the Discord server.
        /// </summary>
        [XmlAttribute(AttributeName = "ServerId")]
        public ulong ServerId { get; set; }

        /// <summary>
        /// The webhood's ID. Use together with <see cref="WebhookToken"/>.
        /// </summary>
        [XmlElement(ElementName = "WebhookId")]
        public ulong WebhookId { get; set; }

        /// <summary>
        /// The webhook's token. Use together with <see cref="WebhookId"/>.
        /// </summary>
        [XmlElement(ElementName = "WebhookToken")]
        public string WebhookToken { get; set; }

        /// <summary>
        /// Simple single check for if the webhook is defined.
        /// </summary>
        [XmlIgnore]
        public bool HasWebhook => WebhookId != 0 && !string.IsNullOrEmpty(WebhookToken);

        /// <summary>
        /// The hash value used to compare galactic messages with.<br/>
        /// This is updated a lot if GalacticForwarding is enabled.
        /// </summary>
        [XmlElement(ElementName = "LastGalacticMessageHash")]
        public string LastGalacticMessageHash { get; set; }

        //[XmlElement(ElementName = "PlayersonRecord")]
        //public int? PlayersonRecord { get; set; }

        //[XmlElement(ElementName = "PlayersonRecordLastAttempt")]
        //public DateTime? PlayersonRecordLastAttempt { get; set; }

        //[XmlElement(ElementName = "HaxusLastOnline")]
        //public DateTime? HaxusLastOnline { get; set; }

        /// <summary>
        /// List of <see cref="HazeronDiscordBotDatabaseServerRole"/>s.
        /// </summary>
        [XmlArray(ElementName = "Roles")]
        [XmlArrayItem(ElementName = "Role", Type = typeof(HazeronDiscordBotDatabaseServerRole))]
        public List<HazeronDiscordBotDatabaseServerRole> Roles { get; set; }

        public HazeronDiscordBotDatabaseServer()
        {
            ServerId = 0;
            WebhookId = 0;
            WebhookToken = string.Empty;
            LastGalacticMessageHash = string.Empty;
            //PlayersonRecord = null;
            //PlayersonRecordLastAttempt = null;
            //HaxusLastOnline = null;
            Roles = new List<HazeronDiscordBotDatabaseServerRole>();
        }
        public HazeronDiscordBotDatabaseServer(ulong id)
            : this()
        {
            ServerId = id;
        }
    }

    public class HazeronDiscordBotDatabaseServerRole
    {
        /// <summary>
        /// The ID of the role on the server.
        /// </summary>
        [XmlAttribute(AttributeName = "RoleId")]
        public ulong RoleId { get; set; }

        /// <summary>
        /// The text used for the role.
        /// </summary>
        [XmlElement(ElementName = "ButtonText")]
        public string ButtonText { get; set; }

        public HazeronDiscordBotDatabaseServerRole()
        {
            RoleId = 0;
            ButtonText = "";
        }
        public HazeronDiscordBotDatabaseServerRole(ulong id, string buttonText)
            : this()
        {
            RoleId = id;
            ButtonText = buttonText;
        }
    }
}
