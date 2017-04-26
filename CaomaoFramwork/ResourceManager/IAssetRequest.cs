using UnityEngine;
using System;

namespace CaomaoFramework.Resource
{
    /// <summary>
    /// 资源请求加载完成之后的委托
    /// </summary>
    /// <param name="assetRequest"></param>
    public delegate void AssetRequestFinishedEventHandler(IAssetRequest assetRequest);
    public interface IAssetRequest : IDisposable
    {
        IAssetResource AssetResource
        {
            get;
        }
        bool IsError
        {
            get;
        }
        /// <summary>
        /// 资源已经加载完成
        /// </summary>
        bool IsFinished
        {
            get;
        }
        object Data
        {
            get;
            set;
        }
        bool RemoveQuickly
        {
            get;
            set;
        }
    }
}