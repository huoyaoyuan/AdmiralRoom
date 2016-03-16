using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Modules.Akashi
{
    public class Akashi : IModule
    {
        public IEnumerable<IChildView> ChildViews
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IChildWindow> ChildWindows
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public FrameworkElement SettingView
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void OnCultureChanged(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
