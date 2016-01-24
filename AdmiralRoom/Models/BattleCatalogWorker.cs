using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom.Models
{
    public class BattleCatalogWorker
    {
        public class BattleCatalogViewModel
        {
            public string Title { get; set; }
            public string Total { get; set; }
            public string Win { get; set; }
            public string S { get; set; }
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
            public string D { get; set; }
            public BattleCatalogViewModel[] Items { get; set; }
        }
        public BattleCatalogViewModel[] Items { get; }
    }
}
