using UnityEngine;
using System.Collections;
namespace CaomaoFramework
{
    #region 模块信息
    /*----------------------------------------------------------------
    // 模块名：IStoryCommandFactory 
    // 创建者：chen
    // 修改者列表：
    // 创建日期：2016.10.12
    // 模块描述：剧情命令工厂接口
    //----------------------------------------------------------------*/
    #endregion
    /// <summary>
    /// 剧情命令工厂接口
    /// </summary>
    public interface IStoryCommandFactory
    {
        IStoryCommand Create(ScriptableData.ISyntaxComponent commandConfig);
    }
    /// <summary>
    /// 剧情命令工厂制造类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StoryCommandFactoryHelper<T> : IStoryCommandFactory where T : IStoryCommand, new()
    {
        public IStoryCommand Create(ScriptableData.ISyntaxComponent commandConfig)
        {
            T t = new T();
            t.Init(commandConfig);
            return t;
        }
    }
}