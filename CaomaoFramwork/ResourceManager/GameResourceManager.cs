using UnityEngine;
using System;
using System.Collections.Generic;
using CaomaoFramework;
using CaomaoFramework.Resource;
using System.IO;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：GameResourceManager
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
public class GameResourceManager : MonoBehaviour, IResourceManager
{
	#region 字段
    private int m_nSizeNeedToLoadFromStart;
    private int m_nSizeHasLoadedFromStart;
    private Dictionary<string, GameAssetResource> m_dicAllAssetRes = new Dictionary<string, GameAssetResource>();
    private Action<bool> m_allLoadFinishedEventHandler;
    private Action<bool> m_allUnLoadFinishedEventHandler;
    private bool m_bLoadError;
    private Queue<GameAssetResource> m_WWWqueue = new Queue<GameAssetResource>();
    private string m_strBaseResDir = string.Empty;
    private static GameResourceManager s_instance;
	#endregion
	#region 属性
    public static GameResourceManager Instance
    {
        get
        {
            return GameResourceManager.s_instance;
        }
    }
    /// <summary>
    /// 加载进度
    /// </summary>
    public float ProgressValue
    {
        get
        {
            if (this.m_nSizeNeedToLoadFromStart > 0)
            {
                return (float)this.m_nSizeHasLoadedFromStart / (float)this.m_nSizeNeedToLoadFromStart;
            }
            return 0f;
        }
    }
	#endregion
	#region 构造方法
	#endregion
	#region 公有方法
    public void Init(string strBaseResDir, string strBaseResWWWDir)
    {
        Resources.UnloadUnusedAssets();
        this.m_strBaseResDir = strBaseResWWWDir;
        //this.m_strBaseResDir = "Assets/Resource_AssetBundle";
        this.m_strBaseResDir = "Assets/Resources";
    }
    public void Clear()
    {
    }
    public IAssetRequest LoadUI(string url, AssetRequestFinishedEventHandler callBackFun)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("string.IsNullOrEmpty(url) == true");
            return null;
        }
        string text = string.Format("{0}/{1}", this.m_strBaseResDir, url);
        text = Path.ChangeExtension(text, ".prefab");
        text = GameResourceManager.StandardlizePath(text);
        Debug.Log("fwef:" + text);
        GameAssetResource assetResource = this.GetAssetResource(text, true);
        return new GameAssetRequest(assetResource, callBackFun);
    }
    public IAssetRequest LoadAtlas(string url, AssetRequestFinishedEventHandler callBackFun)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("string.IsNullOrEmpty(url) == true");
            return null;
        }
        string text = string.Format("{0}/{1}", this.m_strBaseResDir, url);
        text = Path.ChangeExtension(text, ".prefab");
        text = GameResourceManager.StandardlizePath(text);
        GameAssetResource assetResource = this.GetAssetResource(text, true);
        return new GameAssetRequest(assetResource, callBackFun);
    }
    public IAssetRequest LoadTexture(string url, AssetRequestFinishedEventHandler callBackFun)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("string.IsNullOrEmpty(url) == true");
            return null;
        }
        string text = string.Format("{0}/{1}", this.m_strBaseResDir, url);
        text = GameResourceManager.StandardlizePath(text);
        GameAssetResource assetResource = this.GetAssetResource(text, true);
        return new GameAssetRequest(assetResource, callBackFun);
    }
    public IAssetRequest LoadShader(string url, AssetRequestFinishedEventHandler callBackFun)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("string.IsNullOrEmpty(url) == true");
            return null;
        }
        string text = string.Format("{0}/{1}.shader", this.m_strBaseResDir, url);
        text = GameResourceManager.StandardlizePath(text);
        GameAssetResource assetResource = this.GetAssetResource(text, true);
        return new GameAssetRequest(assetResource, callBackFun);
    }
    public IAssetRequest LoadEffect(string url, AssetRequestFinishedEventHandler callBackFun)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("string.IsNullOrEmpty(url) == true");
            return null;
        }
        string text = string.Format("{0}/{1}.prefab", this.m_strBaseResDir, url);
        text = GameResourceManager.StandardlizePath(text);
        GameAssetResource assetResource = this.GetAssetResource(text, true);
        return new GameAssetRequest(assetResource, callBackFun);
    }
    public IAssetRequest LoadModel(string url, AssetRequestFinishedEventHandler callBackFun)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("string.IsNullOrEmpty(url) == true");
            return null;
        }
        string text = string.Format("{0}/{1}.prefab", this.m_strBaseResDir, url);
        text = GameResourceManager.StandardlizePath(text);
        Debug.Log(text);
        GameAssetResource assetResource = this.GetAssetResource(text, true);
        return new GameAssetRequest(assetResource, callBackFun);
    }
    public IAssetRequest LoadAudio(string url, AssetRequestFinishedEventHandler callBackFun)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("string.IsNullOrEmpty(url) == true");
            return null;
        }
        string text = string.Format("{0}/{1}", this.m_strBaseResDir, url);
        try
        {
            if (!Path.HasExtension(text))
            {
                text = Path.ChangeExtension(text, ".mp3");
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        text = GameResourceManager.StandardlizePath(text);
        GameAssetResource assetResource = this.GetAssetResource(text, true);
        return new GameAssetRequest(assetResource, callBackFun);
    }
    public IAssetRequest LoadScene(string url, AssetRequestFinishedEventHandler callBackFun)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("string.IsNullOrEmpty(url) == true");
            return null;
        }
        string url2 = string.Format("{0}/{1}.unity3d", this.m_strBaseResDir, url);
        GameAssetResource assetResource = this.GetAssetResource(url2, true);
        return new GameAssetRequest(assetResource, callBackFun);
    }
    public IAssetRequest CreateAssetRequest(string url, AssetRequestFinishedEventHandler callBackFun)
    {
        return this.CreateAssetRequest(url, null, callBackFun);
    }
    public IAssetRequest CreateAssetRequest(string assetUrl, List<string> dependes, AssetRequestFinishedEventHandler callBackFun)
    {
        if (string.IsNullOrEmpty(assetUrl))
        {
            Debug.LogError("string.IsNullOrEmpty(assetUrl) == true");
            return null;
        }
        assetUrl = string.Format("{0}/{1}", this.m_strBaseResDir, assetUrl);
      
        assetUrl = GameResourceManager.StandardlizePath(assetUrl);
        GameAssetResource assetResource = this.GetAssetResource(assetUrl, true);
        return new GameAssetRequest(assetResource, callBackFun);
    }
    public void SetAllLoadFinishedEventHandler(Action<bool> eventHandler)
    {
        this.m_allLoadFinishedEventHandler = (Action<bool>)Delegate.Combine(this.m_allLoadFinishedEventHandler, eventHandler);
    }
    public void SetAllUnLoadFinishedEventHandler(Action<bool> eventHandler)
    {
        this.m_allUnLoadFinishedEventHandler = (Action<bool>)Delegate.Combine(this.m_allUnLoadFinishedEventHandler, eventHandler);
    }
    private GameAssetResource GetAssetResource(string url, bool bAssetInEditor)
    {
        GameAssetResource localAssetResource = null;
        if (!this.m_dicAllAssetRes.TryGetValue(url, out localAssetResource))
        {
            localAssetResource = new GameAssetResource(url);
            localAssetResource.IsAssetInEditor = bAssetInEditor;
            if (this.m_allLoadFinishedEventHandler != null)
            {
                this.m_nSizeNeedToLoadFromStart++;
            }
            this.m_WWWqueue.Enqueue(localAssetResource);
            this.m_dicAllAssetRes.Add(url, localAssetResource);
        }
        return localAssetResource;
    }
    public void OnUpdate()
    {
        if (this.m_allUnLoadFinishedEventHandler != null)
        {
            Resources.UnloadUnusedAssets();
            Action<bool> allUnloadFinishedEventHandler = this.m_allUnLoadFinishedEventHandler;
            this.m_allUnLoadFinishedEventHandler = null;
            allUnloadFinishedEventHandler(true);
        }
        if (this.m_WWWqueue.Count > 0)
        {
            GameAssetResource localAssetResource = this.m_WWWqueue.Peek();
            if (localAssetResource.IsDone)
            {;
                if (!string.IsNullOrEmpty(localAssetResource.Error))
                {
                    this.m_bLoadError = true;
                    Debug.LogError(localAssetResource.Error);
                }
                if (this.m_allLoadFinishedEventHandler != null)
                {
                    this.m_nSizeHasLoadedFromStart++;
                }
                this.m_WWWqueue.Dequeue();
                localAssetResource.OnDownLoadFinished();
            }
            else
            {
                if (localAssetResource.Canceled)
                {
                    this.m_WWWqueue.Dequeue();
                }
                else
                {
                    if (!localAssetResource.Started)
                    {
                        localAssetResource.BeginLoad();
                    }
                }
            }
        }
        else
        {
            if (this.m_allLoadFinishedEventHandler != null)
            {

                Action<bool> allLoadFinishedEventHandler = this.m_allLoadFinishedEventHandler;
                this.m_allLoadFinishedEventHandler = null;
                try
                {
                    allLoadFinishedEventHandler(!this.m_bLoadError);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
                this.m_nSizeHasLoadedFromStart = 0;
                this.m_nSizeNeedToLoadFromStart = 0;
            }
        }
        this.UnLoadNotUsedResource();
    }
    /// <summary>
    /// 标准化路径（改为“/”，全部小写）
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string StandardlizePath(string path)
    {
        string text = path.Replace("\\", "/");
        text = path.Replace("//", "/");
        return text.ToLower();
    }
	#endregion
	#region 私有方法

    private void Awake()
    {
        GameResourceManager.s_instance = this;
    }
    private static string ConvertToAssetBundleName(string ResPath)
    {
        ResPath = GameResourceManager.StandardlizePath(ResPath);
        string text = ResPath.Replace('/', '_');
        text = text.Replace(" ", "");
        return Path.GetFileNameWithoutExtension(text);
    }
    private void UnLoadNotUsedResource()
    {
        List<string> list = new List<string>();
        foreach (KeyValuePair<string, GameAssetResource> current in this.m_dicAllAssetRes)
        {
            if (0 >= current.Value.RefCount)
            {
                list.Add(current.Key);
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            string key = list[i];
            GameAssetResource localAssetResource = null;
            if (this.m_dicAllAssetRes.TryGetValue(key, out localAssetResource))
            {
                if (localAssetResource != null)
                {
                    localAssetResource.Dispose();
                }
                this.m_dicAllAssetRes.Remove(key);
            }
        }
    }
	#endregion
}
