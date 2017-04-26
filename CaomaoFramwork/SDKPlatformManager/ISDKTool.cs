using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaomaoFramework.SDK
{
    public interface ISDKTool
    {
        EPlatformType EPlatformType
        {
            get;
        }
        bool Init();
        void Install(string file);
    }
}
