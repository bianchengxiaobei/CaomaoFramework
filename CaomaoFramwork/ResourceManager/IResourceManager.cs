using System;
using System.Collections.Generic;
using CaomaoFramework.Resource;
namespace CaomaoFramework
{
    public interface IResourceManager
    {
        float ProgressValue
        {
            get;
        }
        void Init(string strBaseResDir, string strBaseResWWWDir);

        void Clear();

        void OnUpdate();

        IAssetRequest LoadUI(string strDlgName, AssetRequestFinishedEventHandler callBackFun);

        IAssetRequest LoadTexture(string relativeUrl, AssetRequestFinishedEventHandler callBackFun);

        IAssetRequest LoadAudio(string relativeUrl, AssetRequestFinishedEventHandler callBackFun);

        IAssetRequest LoadEffect(string relativeUrl, AssetRequestFinishedEventHandler callBackFun);

        IAssetRequest LoadModel(string relativeUrl, AssetRequestFinishedEventHandler callBackFun);

        IAssetRequest LoadShader(string relativeUrl, AssetRequestFinishedEventHandler callBackFun);

        IAssetRequest LoadScene(string relativeUrl, AssetRequestFinishedEventHandler callBackFun);

        IAssetRequest CreateAssetRequest(string url, AssetRequestFinishedEventHandler callBackFun);

        void SetAllLoadFinishedEventHandler(Action<bool> eventHandler);

        void SetAllUnLoadFinishedEventHandler(Action<bool> eventHandler);
    }
}
