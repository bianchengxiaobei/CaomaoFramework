using System;
using System.Collections.Generic;
using CaomaoFramework.SDK;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
namespace CaomaoFramework
{
    /// <summary>
    /// 平台类型
    /// </summary>
    public enum EPlatformType
    {
        Platform_None,
        Platform_Tencent,
        Platform_Baidu
    }
    [System.Serializable]
    public class SDKPlatformManager : Singleton<SDKPlatformManager>
    {
        [SerializeField]
        private EPlatformType m_ePlatformType = SDKTool.Singleton.EPlatformType;
        public List<EventDelegate> m_installSuccess = new List<EventDelegate>();

        public EPlatformType EPlatformType
        {
            get
            {
                if (this.m_ePlatformType != SDKTool.Singleton.EPlatformType)
                {
                    //改变保存xml
                    
                }
                return this.m_ePlatformType;
            }
        }



        public bool Init()
        {
            ISDKTool sdkTool = this.GetSdkTool(this.EPlatformType);
            if (null == sdkTool)
            {
                Debug.LogError("null == sdkTool");
                return false;
            }
            return SDKTool.Singleton.Init(sdkTool);
        }
        public void Install()
        {
            SDKTool.Singleton.Install("");
        }
        public void Update()
        {
            try
            {
                SDKEvent sdkEvent = null;
                if (SDKTool.Singleton.PopEvent(out sdkEvent))
                {
                    switch (sdkEvent.Type)
                    {
                        case EnumSDKEventType.eSDKEventType_Install_Success:
                            //开始播放视频
                            Debug.Log("播放视频");
                            EventDelegate.Execute(this.m_installSuccess);
                            break;

                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }




        private ISDKTool GetSdkTool(EPlatformType type)
        {
            ISDKTool sdkTool = null;
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    Type toolType = typeof(WindowsSDKToolImpl);
                    if (type == EPlatformType.Platform_Tencent)
                    {
                        toolType = Type.GetType("WindowsTencensSDKToolImpl");
                    } else if (type == EPlatformType.Platform_Baidu)
                    {
                        toolType = Type.GetType("WindowsBaiduSDKToolImpl");
                    }
                    sdkTool = (ISDKTool)Activator.CreateInstance(toolType);
                    break;
            }
            return sdkTool;
        }
    }
}
