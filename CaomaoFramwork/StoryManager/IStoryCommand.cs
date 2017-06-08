using UnityEngine;
using System.Collections;
namespace CaomaoFramework
{
    #region 模块信息
    /*----------------------------------------------------------------
    // 模块名：IStoryCommand 
    // 创建者：chen
    // 修改者列表：
    // 创建日期：2016.10.12
    // 模块描述：剧情命令接口
    //----------------------------------------------------------------*/
    #endregion
    /// <summary>
    /// 剧情命令接口
    /// </summary>
    public interface IStoryCommand
    {
        void Init(ScriptableData.ISyntaxComponent config);
        IStoryCommand Clone();
        void Reset();
        void Prepare(StoryInstance instance, object iterator, object[] args);
        bool Execute(StoryInstance instance, long delta);
        void Analyze(StoryInstance instance);
    }
}