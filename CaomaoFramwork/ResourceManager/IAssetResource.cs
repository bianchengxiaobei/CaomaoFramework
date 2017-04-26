using UnityEngine;
using System;
namespace CaomaoFramework.Resource
{
    /// <summary>
    /// 资源加载完成之后的委托
    /// </summary>
    /// <param name="assetResource"></param>
    public delegate void AssetLoadFinishedEventHandler(IAssetResource assetResource);
    public interface IAssetResource : IDisposable
    {
        string URL
        {
            get;
        }
        bool IsDone
        {
            get;
        }
        UnityEngine.Object MainAsset
        {
            get;
        }
        string Text
        {
            get;
        }
        byte[] Bytes
        {
            get;
        }
        Texture Texture
        {
            get;
        }
        AudioClip Audio
        {
            get;
        }
        float progress
        {
            get;
        }
    }
}