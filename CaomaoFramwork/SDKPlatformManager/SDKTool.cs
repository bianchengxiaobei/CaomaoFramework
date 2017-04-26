using System;
using System.Collections.Generic;

namespace CaomaoFramework.SDK
{
    public class SDKTool
    {
        private static SDKTool mInst = null;
        private ISDKTool m_sdkToolImpl = null;
        private Queue<SDKEvent> m_queueSDKEvent = new Queue<SDKEvent>();
        public static SDKTool Singleton
        {
            get
            {
                if (SDKTool.mInst == null)
                {
                    SDKTool.mInst = new SDKTool();
                }
                return SDKTool.mInst;
            }
        }
        public EPlatformType EPlatformType
        {
            get
            {
                if (null == this.m_sdkToolImpl)
                {
                    return EPlatformType.Platform_None;
                }
                else
                {
                    return this.m_sdkToolImpl.EPlatformType;
                }
            }
        }


        public bool Init(ISDKTool sdktool)
        {
            if (null == sdktool)
            {
                return false;
            }
            this.m_sdkToolImpl = sdktool;
            return this.m_sdkToolImpl.Init();
        }
        public void Install(string file)
        {
            if (this.m_sdkToolImpl != null)
            {
                this.m_sdkToolImpl.Install(file);
            }
        }
        public bool PopEvent(out SDKEvent sdkEvent)
        {
            bool result;
            if (this.m_queueSDKEvent.Count > 0)
            {
                sdkEvent = this.m_queueSDKEvent.Dequeue();
                result = true;
            }
            else
            {
                sdkEvent = null;
                result = false;
            }
            return result;
        }
        public SDKEvent PushEvent(EnumSDKEventType eSdkEventType)
        {
            SDKEvent e = new SDKEvent();
            e.Type = eSdkEventType;
            this.m_queueSDKEvent.Enqueue(e);
            return e;
        }

    }
}
