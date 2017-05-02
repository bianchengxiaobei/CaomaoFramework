using System;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework.Resource;
namespace CaomaoFramework
{
    [System.Serializable]
    public class ResourceManager
    {
        [SerializeField]
        public string strAssetPath;
        private IResourceManager m_resourceManager;
        private static ResourceManager s_instance;
        public static ResourceManager singleton
        {
            get
            {
                if (ResourceManager.s_instance == null)
                {
                    ResourceManager.s_instance = new ResourceManager();
                }
                return ResourceManager.s_instance;
            }
        }
        /// <summary>
        /// 加载进度
        /// </summary>
        public float ProgressValue
        {
            get
            {
                if (this.m_resourceManager == null)
                {
                    Debug.LogError("null == m_resourceManager");
                    return 0f;
                }
                return this.m_resourceManager.ProgressValue;
            }
        }
        /// <summary>
        /// 私有构造函数
        /// </summary>
        private ResourceManager()
        {
            
        }
        /// <summary>
        /// 初始化资源管理器
        /// </summary>
        /// <param name="editorResourceManager"></param>
        public void Init(IResourceManager editorResourceManager)
        {
            if ((RuntimePlatform.WindowsEditor == Application.platform || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsPlayer) && editorResourceManager != null)
            {
                this.m_resourceManager = editorResourceManager;
            }
            if (this.m_resourceManager != null)
            {
                this.m_resourceManager.Init(this.strAssetPath,"");
                return;
            }
            Debug.LogError("null == m_resourceManager");
        }
        public void Clear()
        {
            if (this.m_resourceManager == null)
            {
                Debug.LogError("null == m_resourceManager");
                return;
            }
            this.m_resourceManager.Clear();
        }
        /// <summary>
        /// 通过Resources加载资源
        /// </summary>
        /// <param name="strAssetFile"></param>
        /// <returns></returns>
        public UnityEngine.Object Load(string strAssetFile)
        {
            UnityEngine.Object @object = null;
            if (!string.IsNullOrEmpty(strAssetFile))
            {
                @object = Resources.Load(strAssetFile);
            }
            if (null == @object)
            {
                Debug.LogError("null == obj: " + strAssetFile);
            }
            Resources.UnloadUnusedAssets();
            return @object;
        }
        public IAssetRequest CreateAssetRequest(string strCompleteUrl, AssetRequestFinishedEventHandler callBackFun)
        {
            if (this.m_resourceManager == null)
            {
                Debug.LogError("null == m_resourceManager");
                return null;
            }
            IAssetRequest result = null;
            try
            {
                result = this.m_resourceManager.CreateAssetRequest(strCompleteUrl, callBackFun);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            return result;
        }
        public IAssetRequest LoadTexture(string relativeUrl, AssetRequestFinishedEventHandler callBackFun)
        {
            if (this.m_resourceManager == null)
            {
                Debug.LogError("null == m_resourceManager");
                return null;
            }
            IAssetRequest result = null;
            try
            {
                result = this.m_resourceManager.LoadTexture(relativeUrl, callBackFun);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            return result;
        }
        public IAssetRequest LoadAudio(string relativeUrl, AssetRequestFinishedEventHandler callBackFun)
        {
            if (this.m_resourceManager == null)
            {
                Debug.LogError("null == m_resourceManager");
                return null;
            }
            IAssetRequest result = null;
            try
            {
                result = this.m_resourceManager.LoadAudio(relativeUrl, callBackFun);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            return result;
        }
        public IAssetRequest LoadUI(string relativeUrl, AssetRequestFinishedEventHandler callBackFun)
        {
            if (this.m_resourceManager == null)
            {
                Debug.LogError("null == m_resourceManager");
                return null;
            }
            IAssetRequest result = null;
            try
            {
                result = this.m_resourceManager.LoadUI(relativeUrl, callBackFun);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            return result;
        }
        public IAssetRequest LoadEffect(string relativeUrl, AssetRequestFinishedEventHandler callBackFun)
        {
            if (this.m_resourceManager == null)
            {
                Debug.LogError("null == m_resourceManager");
                return null;
            }
            IAssetRequest result = null;
            try
            {
                result = this.m_resourceManager.LoadEffect(relativeUrl, callBackFun);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            return result;
        }
        public IAssetRequest LoadModel(string relativeUrl, AssetRequestFinishedEventHandler callBackFun)
        {
            IAssetRequest result;
            if (null == this.m_resourceManager)
            {
                Debug.LogError("null == m_resourceManager");
                result = null;
            }
            else
            {
                IAssetRequest assetRequest = null;
                try
                {
                    assetRequest = this.m_resourceManager.LoadModel(relativeUrl, callBackFun);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                result = assetRequest;
            }
            return result;
        }
        public IAssetRequest LoadScene(string relativeUrl, AssetRequestFinishedEventHandler callBackFun)
        {
            IAssetRequest result;
            if (null == this.m_resourceManager)
            {
                Debug.LogError("null == m_resourceManager");
                result = null;
            }
            else
            {
                IAssetRequest assetRequest = null;
                try
                {
                    assetRequest = this.m_resourceManager.LoadScene(relativeUrl, callBackFun);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                result = assetRequest;
            }
            return result;
        }
        public void SetAllLoadFinishedEventHandler(Action<bool> eventHandler)
        {
            if (this.m_resourceManager == null)
            {
                return;
            }
            this.m_resourceManager.SetAllLoadFinishedEventHandler(eventHandler);
        }
        public void SetAllUnLoadFinishedEventHandler(Action<bool> eventHandler)
        {
            if (null != this.m_resourceManager)
            {
                this.m_resourceManager.SetAllUnLoadFinishedEventHandler(eventHandler);
            }
        }
        public void Update()
        {
            try
            {
                if (this.m_resourceManager != null)
                {
                    this.m_resourceManager.OnUpdate();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
