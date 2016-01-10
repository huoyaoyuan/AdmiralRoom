using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    sealed class LoggerHost
    {
        private LoggerHost() { }
        public static LoggerHost Instance { get; } = new LoggerHost();
        public void Initialize()
        {

        }
    }
}
