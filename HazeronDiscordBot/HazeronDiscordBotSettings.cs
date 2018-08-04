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
        [XmlAttribute]
        public int Version { get; set; }

        [XmlElement]
        public string BotToken { get; set; }

        [XmlElement]
        public HazeronDiscordBotSettingsMemory Memory { get; set; }
        
        public HazeronDiscordBotSettings()
        {
            Version = 1;
            BotToken = "";
            Memory = new HazeronDiscordBotSettingsMemory();
        }

        /// <summary>
        /// Saves the settings XML file, creating it if it doesn't exist.
        /// </summary>
        /// <param name="filePath">File path of the settings XML file.</param>
        public void Save(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(HazeronDiscordBotSettings));
            TextWriter textWriter = new StreamWriter(filePath);
            serializer.Serialize(textWriter, this);
            textWriter.Close();
        }

        /// <summary>
        /// Loads the settings XML file.
        /// </summary>
        /// <param name="filePath">File path of the settings XML file.</param>
        /// <returns>Returns loaded <see cref="HazeronDiscordBotSettings"/> object.</returns>
        public static HazeronDiscordBotSettings Load(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(HazeronDiscordBotSettings));
            TextReader reader = new StreamReader(filePath);
            HazeronDiscordBotSettings settings = (HazeronDiscordBotSettings)serializer.Deserialize(reader);
            reader.Close();

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
                settings.Save(filePath);
                return settings;
            }
        }
    }

    public class HazeronDiscordBotSettingsMemory
    {
        [XmlAttribute]
        public int PlayersOnlineRecord { get; set; }

        [XmlElement]
        public DateTime HaxusLastOnline { get; set; }

        public HazeronDiscordBotSettingsMemory()
        {
            PlayersOnlineRecord = 25;
            HaxusLastOnline = DateTime.MinValue;
        }
    }
}
