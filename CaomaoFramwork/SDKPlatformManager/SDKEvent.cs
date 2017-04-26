using System;
using System.Collections.Generic;

namespace CaomaoFramework.SDK
{
    public enum EnumSDKEventType
    {
        eSDKEventType_Install_Fail,
        eSDKEventType_Install_Success,
        eSDKEventType_Update_FailGetUpdateInfo,
        eSDKEventType_Update_FailParseUpdateInfo,
        eSDKEventType_Update_FailParseLocalUpdateInfo,
        eSDKEventType_Update_LatestVersion,
        eSDKEventType_Update_NeedUpdateApp,
        eSDKEventType_Update_Fail,
        eSDKEventType_Update_Success,
        eSDKEventType_Login_Success,
        eSDKEventType_Login_Fail,
        eSDKEventType_Pay_Success,
        eSDKEventType_Pay_Fail,
        eSDKEventType_Pay_Cancel,
        eSDKEventType_MovieFinish
    }
    public class SDKEvent
    {
        public EnumSDKEventType Type
        {
            get;
            set;
        }

        public string Param
        {
            get;
            set;
        }

        public Action CallBack
        {
            get;
            set;
        }
    }
}
