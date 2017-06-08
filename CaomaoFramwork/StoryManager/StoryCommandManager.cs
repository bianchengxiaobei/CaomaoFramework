using UnityEngine;
using System.Collections.Generic;
namespace CaomaoFramework
{
    #region 模块信息
    /*----------------------------------------------------------------
    // 模块名：StoryCommandManager
    // 创建者：chen
    // 修改者列表：
    // 创建日期：2016.10.12
    // 模块描述：剧情命令管理器
    //----------------------------------------------------------------*/
    #endregion
    /// <summary>
    /// 剧情命令管理器
    /// </summary>
    public class StoryCommandManager : Singleton<StoryCommandManager>
    {
        private Dictionary<string, IStoryCommandFactory> m_dicStoryCommandFactories = new Dictionary<string, IStoryCommandFactory>();

        public StoryCommandManager()
        {
            //RegisterCommandFactory("sendmessage", new StoryCommandFactoryHelper<SendMessageCommand>());
            //RegisterCommandFactory("showui", new StoryCommandFactoryHelper<ShowUICommand>());
        }
        /// <summary>
        /// 注册命令工厂
        /// </summary>
        /// <param name="type"></param>
        /// <param name="factory"></param>
        public void RegisterCommandFactory(string type, IStoryCommandFactory factory)
        {
            if (!m_dicStoryCommandFactories.ContainsKey(type))
            {
                m_dicStoryCommandFactories.Add(type, factory);
            }
            else
            {
                Debug.LogWarning("重复添加命令工厂类");
            }
        }
        /// <summary>
        /// 创建一个命令
        /// </summary>
        /// <param name="commandConfig"></param>
        /// <returns></returns>
        public IStoryCommand CreateCommand(ScriptableData.ISyntaxComponent commandConfig)
        {
            IStoryCommand command = null;
            string type = commandConfig.GetId();
            IStoryCommandFactory factory = GetFactory(type);
            if (factory != null)
            {
                command = factory.Create(commandConfig);
            }
            else
            {
                Debug.LogError("创建命令失败,命令类型:" + type);
            }
            return command;
        }
        public IStoryCommandFactory GetFactory(string type)
        {
            IStoryCommandFactory factory = null;
            if (m_dicStoryCommandFactories.ContainsKey(type))
            {
                factory = m_dicStoryCommandFactories[type];
            }
            return factory;
        }
    }
}