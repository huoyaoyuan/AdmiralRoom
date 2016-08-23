using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom.Notifier
{
    public abstract class NotifierFactory
    {
        public abstract void Show(string title, string detail, string sound = null);
    }
}
