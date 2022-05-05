using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HazeronDiscordBot
{
    public class GalacticMessage
    {
        #region Static Properties
        private static DateTime _lastUpdated = new DateTime(2022, 03, 01);
        #endregion

        #region Static Methods
        /// <summary>
        /// Returns all galactic messages from the galactic.html webpage.
        /// </summary>
        /// <param name="url">URL to the galactic.html webpage.</param>
        /// <returns>A </returns>
        public static ICollection<GalacticMessage> GetMessages(string url)
        {
            List<GalacticMessage> messages = new List<GalacticMessage>();

            HtmlPage page;
            try
            {
                page = HtmlPage.GetPage(url, _lastUpdated);
                _lastUpdated = DateTime.Now;
            }
            catch (HtmlPageNotNotModifiedException)
            {
                // If the page wasn't updated since last read, just return nothing.
                return messages;
            }
            catch (InvalidOperationException)
            {
                // The stream is already in use by a previous call.
                return messages;
            }

            const string USER_MESSAGE_REGEX = @"(?<time>\d\d:\d\d).*[^;](> (?<galaxy>[^<]*)<.*[^;]\"">(?<sender>[^<]*)|>Hazeron<).*\"">(?<message>[^<]*)";
            MatchCollection pageMessages = Regex.Matches(page.Html, USER_MESSAGE_REGEX, RegexOptions.ExplicitCapture);

            foreach (Match match in pageMessages)
            {
                string message = match.Groups["message"].Value.Replace("&gt", ">").Replace("&lt", "<");
                if (match.Groups["galaxy"].Success && match.Groups["sender"].Success)
                    messages.Add(new GalacticMessage(match.Groups["time"].Value, match.Groups["galaxy"].Value, match.Groups["sender"].Value, message));
                else
                    messages.Add(new SystemMessage(match.Groups["time"].Value, message));
            }

            return messages;
        }
        #endregion

        #region Properties
        public string Time { get; protected set; }
        public Galaxy Galaxy { get; protected set; }
        public string Sender { get; protected set; }
        public string Message { get; protected set; }
        #endregion

        #region Constructor
        public GalacticMessage()
        {
        }
        protected GalacticMessage(string time, Galaxy galaxy, string sender, string message)
        {
            Time = time;
            Galaxy = galaxy;
            Sender = sender;
            Message = message;
        }
        protected GalacticMessage(string time, string galaxy, string sender, string message)
            : this(time, Galaxy.Unknown, sender, message)
        {
            switch (galaxy)
            {
                case "Andromeda Rising":
                    Galaxy = Galaxy.AndromedaRising;
                    break;
                case "Black Hole":
                    Galaxy = Galaxy.BlackHole;
                    break;
                case "Core":
                    Galaxy = Galaxy.Core;
                    break;
                case "Crown of Othon":
                    Galaxy = Galaxy.CrownOfOthon;
                    break;
                case "Dyrathon's Retreat":
                    Galaxy = Galaxy.DyrathonsRetreat;
                    break;
                case "Edge of the Rift":
                    Galaxy = Galaxy.EdgeOfTheRift;
                    break;
                case "Falla's Embrace":
                    Galaxy = Galaxy.FallasEmbrace;
                    break;
                case "Fallen Legions of Muturon":
                    Galaxy = Galaxy.FallenLegionsOfMuturon;
                    break;
                case "Heart of Victorus":
                    Galaxy = Galaxy.HeartOfVictorus;
                    break;
                case "House Zanathar":
                    Galaxy = Galaxy.HouseZanathar;
                    break;
                case "Indigo Sea":
                    Galaxy = Galaxy.IndigoSea;
                    break;
                case "In'kar Border Region":
                    Galaxy = Galaxy.InkarBorderRegion;
                    break;
                case "Milky Way":
                    Galaxy = Galaxy.MilkyWay;
                    break;
                case "Muturon Encounter":
                    Galaxy = Galaxy.MuturonEncounter;
                    break;
                case "Ransuul's Flaming Sword":
                    Galaxy = Galaxy.RansuulsFlamingSword;
                    break;
                case "Seven Ten":
                    Galaxy = Galaxy.SevenTen;
                    break;
                case "Shores of Hazeron":
                    Galaxy = Galaxy.ShoresOfHazeron;
                    break;
                case "Thustra's Eye":
                    Galaxy = Galaxy.ThustrasEye;
                    break;
                case "Veil of Targoss":
                    Galaxy = Galaxy.VeilOfTargoss;
                    break;
                case "Vreenox Eclipse":
                    Galaxy = Galaxy.VreenoxEclipse;
                    break;
                case "Vulcan's Forge":
                    Galaxy = Galaxy.VulcansForge;
                    break;
            }
        }
        #endregion

        #region Methods
        public string ToHash()
        {
            string input = $"{Time}+{Sender}+{Message}";

            // Use input string to calculate MD5 hash, baed on: https://stackoverflow.com/a/24031467
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return BitConverter.ToString(hashBytes).Replace("-", "");
            }
        }
        #endregion
    }
}
