using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace HazeronDiscordBot
{
    [Serializable]
    [XmlRoot(ElementName = "HazeronDiscordBotSettings", Namespace = "")]
    public class HazeronDiscordBotSettings
    {
        #region Static Methods
        /// <summary>
        /// Loads the settings XML file.
        /// </summary>
        /// <param name="filePath">File path of the settings XML file.</param>
        /// <returns>Returns loaded <see cref="HazeronDiscordBotSettings"/> object.</returns>
        public static HazeronDiscordBotSettings Load(string filePath)
        {
            HazeronDiscordBotSettings settings;
            XmlSerializer serializer = new XmlSerializer(typeof(HazeronDiscordBotSettings));
            using (TextReader reader = new StreamReader(filePath))
                settings = (HazeronDiscordBotSettings)serializer.Deserialize(reader);
            settings.FilePath = filePath;

            return settings;
        }

        /// <summary>
        /// Checks if a settings XML file with the name exist and loads it, if one does not exist it is created.
        /// </summary>
        /// <param name="filePath">File path of the settings XML file.</param>
        /// <returns>Returns loaded or created <see cref="HazeronDiscordBotSettings"/> object.</returns>
        public static HazeronDiscordBotSettings LoadOrCreate(string filePath)
        {
            if (File.Exists(filePath))
                return Load(filePath);
            else
            {
                HazeronDiscordBotSettings settings;
                settings = new HazeronDiscordBotSettings();
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

        /// <summary>
        /// The Discord bot token. Do not publically share this anywhere.
        /// </summary>
        [XmlElement(ElementName = "BotToken")]
        public string BotToken { get; set; }

        /// <summary>
        /// Filepath to the Database XML file.<br/>
        /// Default: "HazeronDiscordBotDatabase.xml"
        /// </summary>
        [XmlElement(ElementName = "DatabaseXmlPath")]
        public string DatabaseXmlPath { get; set; }

        /// <summary>
        /// More advanced settings.
        /// </summary>
        [XmlElement(ElementName = "Advanced")]
        public HazeronDiscordBotSettingsAdvanced Advanced { get; set; }

        public HazeronDiscordBotSettings()
        {
            Version = 1;
            BotToken = "";
            DatabaseXmlPath = Path.ChangeExtension(System.Diagnostics.Process.GetCurrentProcess().ProcessName + "Database", "xml");
            Advanced = new HazeronDiscordBotSettingsAdvanced();
        }

        #region Methods
        /// <summary>
        /// Saves the settings XML file, creating it if it doesn't exist.
        /// </summary>
        public void Save()
        {
            Save(FilePath);
        }
        /// <summary>
        /// Saves the settings XML file, creating it if it doesn't exist.
        /// </summary>
        /// <param name="filePath">File path of the settings XML file.</param>
        public void Save(string filePath)
        {
            lock (_saveLock)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HazeronDiscordBotSettings));
                using (TextWriter textWriter = new StreamWriter(filePath))
                    serializer.Serialize(textWriter, this);
            }
        }
        #endregion
    }

    public class HazeronDiscordBotSettingsAdvanced
    {
        /// <summary>
        /// The webpage used to check current online avatars.<br/>
        /// Default: "https://hazeron.com/playerson.php"
        /// </summary>
        [XmlElement(ElementName = "UrlPlayerson")]
        public string UrlPlayerson { get; set; }

        /// <summary>
        /// The webpage used to see historical online avatar counts.<br/>
        /// Default: "https://hazeron.com/playerhistory.txt"
        /// </summary>
        [XmlElement(ElementName = "UrlPlayerhistory")]
        public string UrlPlayerhistory { get; set; }

        /// <summary>
        /// The webpage used to read the galactic chat.<br/>
        /// Default: "https://hazeron.com/galactic.html"
        /// </summary>
        [XmlElement(ElementName = "UrlGalactic")]
        public string UrlGalactic { get; set; }

        /// <summary>
        /// Optional Discord server's GuildId used for development testing.
        /// </summary>
        [XmlElement(ElementName = "DeveloperTestServer")]
        public ulong? DeveloperTestServer { get; set; }

        public HazeronDiscordBotSettingsAdvanced()
        {
            UrlPlayerson = @"https://hazeron.com/playerson.php";
            UrlPlayerhistory = @"https://hazeron.com/playerhistory.txt";
            UrlGalactic = @"https://hazeron.com/galactic.html";
        }
    }
}
