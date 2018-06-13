using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace log4net
{
    [Serializable]
    public class LogConfig
    {
        public string LogRepostory;
        public LogLevel Level;
        public string Path;
    }
}
