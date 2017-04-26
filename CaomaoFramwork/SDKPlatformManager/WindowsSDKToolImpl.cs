using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaomaoFramework.SDK
{
    public class WindowsSDKToolImpl : ISDKTool
    {
        public virtual EPlatformType EPlatformType
        {
            get
            {
                return EPlatformType.Platform_None;
            }
        }
        public virtual bool Init()
        {
            return true;
        }
        public void Install(string file)
        {
            SDKTool.Singleton.PushEvent(EnumSDKEventType.eSDKEventType_Install_Success);
        }
    }
}
