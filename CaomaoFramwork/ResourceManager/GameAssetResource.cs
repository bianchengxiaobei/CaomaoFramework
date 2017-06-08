using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using CaomaoFramework.Resource;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：GameAssetResource
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
public class GameAssetResource : IDisposable,IAssetResource
{
	#region 字段
    private UnityEngine.Object m_objMainAsset;
    private WWW m_wwwObject;
    private string m_strAssetUrl = string.Empty;
    private bool m_bPrepared;
    private bool m_bAssetInEditor;
    private bool m_bCancel;
    private bool m_bStartedLoad;
    private string m_strError = string.Empty;
    private bool m_bHasCallBacked;
    private List<GameAssetRequest> m_listAssetRequest = new List<GameAssetRequest>();
	#endregion
	#region 属性
    public string URL
    {
        get
        {
            int num = this.m_strAssetUrl.IndexOf("assets/resources");
            if (num < 0)
            {
                return this.m_strAssetUrl;
            }
            string text = this.m_strAssetUrl.Substring(num + 17);
            int num2 = text.IndexOf('.');
            if (num2 >= 0)
            {
                return text.Substring(0, num2);
            }
            return text;
        }
    }
    public bool Canceled
    {
        get
        {
            return this.m_bCancel;
        }
    }
    public bool Started
    {
        get
        {
            return this.m_bStartedLoad;
        }
    }
    public int RefCount
    {
        get
        {
            return this.m_listAssetRequest.Count;
        }
    }
    public bool IsDone
    {
        get
        {
            if (this.m_bAssetInEditor)
            {
                Debug.Log("23432r");
                return this.m_bPrepared;
            }
            return this.m_wwwObject != null && this.m_wwwObject.isDone;
        }
    }
    public bool HasCallBacked
    {
        get
        {
            return this.m_bHasCallBacked;
        }
    }
    public UnityEngine.Object MainAsset
    {
        get
        {
            if (this.m_bAssetInEditor)
            {
                return this.m_objMainAsset;
            }
            if (this.m_wwwObject != null && this.m_wwwObject.isDone)
            {
                return this.m_wwwObject.assetBundle.mainAsset;
            }
            return null;
        }
    }
    public Texture Texture
    {
        get
        {
            if (this.m_wwwObject != null)
            {
                return this.m_wwwObject.texture;
            }
            return null;
        }
    }
    public AudioClip Audio
    {
        get
        {
            if (this.m_wwwObject != null)
            {
                //return this.m_wwwObject.audioClip;
                return WWWAudioExtensions.GetAudioClip(this.m_wwwObject);
            }
            return null;
        }
    }
    public string Error
    {
        get
        {
            if (this.m_bAssetInEditor)
            {
                return this.m_strError;
            }
            if (this.m_wwwObject != null)
            {
                return this.m_wwwObject.error;
            }
            return string.Empty;
        }
    }
    public byte[] Bytes
    {
        get
        {
            if (this.m_wwwObject != null)
            {
                return this.m_wwwObject.bytes;
            }
            return null;
        }
    }
    public string Text
    {
        get
        {
            if (this.m_wwwObject != null)
            {
                return this.m_wwwObject.text;
            }
            return string.Empty;
        }
    }
    public float progress
    {
        get
        {
            return 1f;
        }
    }
    public bool IsAssetInEditor
    {
        get
        {
            return this.m_bAssetInEditor;
        }
        set
        {
            this.m_bAssetInEditor = value;
        }
    }
	#endregion
	#region 构造方法
    public GameAssetResource(string url)
    {
        this.m_strAssetUrl = url;
        this.m_bCancel = false;
    }
	#endregion
	#region 公有方法
    public void BeginLoad()
    {
        if (this.m_bStartedLoad)
        {
            return;
        }
        this.m_bStartedLoad = true;
        if (!this.IsAssetInEditor)
        {
            //Debug.Log(this.m_strAssetUrl);
            //this.m_wwwObject = new WWW(this.m_strAssetUrl);
            this.m_objMainAsset = Resources.Load(this.URL);
            this.m_bPrepared = true;
        }
        else 
        {
            this.m_objMainAsset = Resources.Load(this.URL);
            this.m_bPrepared = true;
        }
    }
    public void OnDownLoadFinished()
    {
        try
        {
            if (!this.m_bHasCallBacked)
            {
                this.m_bHasCallBacked = true;
                List<GameAssetRequest> list = new List<GameAssetRequest>(this.m_listAssetRequest);
                for (int i = 0; i < list.Count; i++)
                {
                    GameAssetRequest localAssetRequest = list[i];
                    localAssetRequest.OnAssetLoadFinshed(this);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }
    public void AddRef(GameAssetRequest assetRequest)
    {
        this.m_listAssetRequest.Add(assetRequest);
    }
    public void DelRef(GameAssetRequest assetRequest)
    {
        if (!this.m_listAssetRequest.Remove(assetRequest))
        {
            Debug.LogError("false == m_listAssetRequest.Remove(assetRequest):");
        }
    }
    public void Dispose()
    {
    }
	#endregion
	#region 私有方法
	#endregion
}
