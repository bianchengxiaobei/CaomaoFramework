using UnityEngine;
using System;
using System.Collections;
using CaomaoFramework.Resource;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：GameAssetRequest
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
public class GameAssetRequest : IDisposable,IAssetRequest
{
	#region 字段
    private bool m_bFinished;
    private bool m_bDisposed;
    private bool m_bIsError;
    private GameAssetResource m_localAssetResource;
    private AssetRequestFinishedEventHandler m_eventHandlerAssetRequestFinished;
	#endregion
	#region 属性
    public IAssetResource AssetResource
    {
        get
        {
            return this.m_localAssetResource;
        }
    }
    public bool IsError
    {
        get
        {
            return this.m_bIsError;
        }
    }
    public bool IsFinished
    {
        get
        {
            return this.m_bFinished;
        }
    }
    public object Data
    {
        get;
        set;
    }
    public bool RemoveQuickly
    {
        get;
        set;
    }
	#endregion
	#region 构造方法
    public GameAssetRequest(GameAssetResource assetResource, AssetRequestFinishedEventHandler eventHandler)
    {
        if (assetResource == null)
        {
            this.m_bIsError = true;
            return;
        }
        this.m_localAssetResource = assetResource;
        this.m_localAssetResource.AddRef(this);
        this.m_eventHandlerAssetRequestFinished = eventHandler;
        if (eventHandler != null && this.m_localAssetResource.HasCallBacked)
        {         
            GameResourceManager.Instance.StartCoroutine(this.DelayCallBack(eventHandler, this));
        }
    }
	#endregion
	#region 公有方法
    public void OnAssetLoadFinshed(IAssetResource assetResource)
	{
		this.m_bFinished = true;
		if (this.m_eventHandlerAssetRequestFinished != null)
		{
			this.m_eventHandlerAssetRequestFinished(this);
		}
	}
	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}
	protected void Dispose(bool disposing)
	{
		if (this.m_bDisposed)
		{
			return;
		}
		if (disposing)
		{
			if (this.m_localAssetResource != null)
			{
				this.m_localAssetResource.DelRef(this);
				this.m_localAssetResource = null;
			}
			this.m_eventHandlerAssetRequestFinished = null;
		}
		this.m_bDisposed = true;
	}
	~GameAssetRequest()
	{
		this.Dispose(false);
	}
	#endregion
	#region 私有方法
    private IEnumerator DelayCallBack(AssetRequestFinishedEventHandler eventHandler, IAssetRequest assetRequest)
    {
        yield return new WaitForSeconds(0.01f);
        this.m_bFinished = true;
        eventHandler(assetRequest);
        yield break;
    }
	#endregion
}
