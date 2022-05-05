using System;
using System.Runtime.Serialization;

namespace HazeronDiscordBot
{
    [Serializable]
    internal class HtmlPageNotNotModifiedException : Exception
    {
        public string Url { get; protected set; }
        public DateTime LastModified { get; protected set; }

        public HtmlPageNotNotModifiedException()
        {
        }
        public HtmlPageNotNotModifiedException(string url, DateTime lastModified) : base(DefaultMessage(url, lastModified))
        {
            Url = url;
            LastModified = lastModified;
        }
        public HtmlPageNotNotModifiedException(string url, DateTime lastModified, Exception innerException) : base(DefaultMessage(url, lastModified), innerException)
        {
            Url = url;
            LastModified = lastModified;
        }

        protected static string DefaultMessage(string url, DateTime lastModified)
        {
            return $"HTML page has not been modified since date given.{Environment.NewLine}" +
                $"URL: {url}" +
                $"Last Modified: {lastModified:u}";
        }
    }
}