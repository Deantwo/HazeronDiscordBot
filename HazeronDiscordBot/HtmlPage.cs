using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace HazeronDiscordBot
{
    public class HtmlPage
    {
        #region Static Methods
        public static HtmlPage GetPage(string url)
        {
            return GetPage(url, null);
        }
        public static HtmlPage GetPage(string url, DateTime? lastUpdated)
        {
            string html;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                if (lastUpdated.HasValue)
                    request.IfModifiedSince = lastUpdated.Value;
                request.Timeout = 5000;
                request.Method = "GET";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream receiveStream = response.GetResponseStream())
                using (StreamReader sr = new StreamReader(receiveStream, Encoding.UTF8))
                    html = sr.ReadToEnd();
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                if (response != null && response.StatusCode == HttpStatusCode.NotModified)
                    throw new HtmlPageNotNotModifiedException(url, response.LastModified);

                throw;
            }

            return new HtmlPage(html, lastUpdated.Value);
        }
        #endregion

        #region Properties
        public DateTime LastModified { get; protected set; }
        public string Html { get; protected set; }
        #endregion

        #region Constructor
        protected HtmlPage(string html, DateTime lastModified)
        {
            Html = html;
            LastModified = lastModified;
        }
        #endregion
    }
}