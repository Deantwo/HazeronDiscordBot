using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HazeronDiscordBot
{
    public class SystemMessage : GalacticMessage
    {
        #region Constructor
        public SystemMessage()
        {
        }
        protected internal SystemMessage(string time, string message)
        {
            Time = time;
            Galaxy = Galaxy.Unknown;
            Sender = null;
            Message = message;
        }
        #endregion
    }
}
