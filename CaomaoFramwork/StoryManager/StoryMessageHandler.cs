using UnityEngine;
using System.Collections.Generic;
namespace CaomaoFramework
{
    #region 模块信息
    /*----------------------------------------------------------------
    // 模块名：StoryMessag 
    // 创建者：chen
    // 修改者列表：
    // 创建日期：2016.10.12
    // 模块描述：剧情消息处理器，根据消息的id来获取对应的处理器
    //----------------------------------------------------------------*/
    #endregion
    /// <summary>
    /// 剧情消息处理器
    /// </summary>
    public sealed class StoryMessageHandler
    {
        #region 字段
        private string m_sMessageId = null;
        private bool m_bIsTriggered = false;
        private Queue<IStoryCommand> m_queueCommandQueue = new Queue<IStoryCommand>();
        private object[] m_oArguments = null;
        private List<IStoryCommand> m_listLoadedCommands = new List<IStoryCommand>();
        #endregion
        #region 属性
        /// <summary>
        /// 消息Id
        /// </summary>
        public string MessageId
        {
            get { return this.m_sMessageId; }
            set { this.m_sMessageId = value; }
        }
        /// <summary>
        /// 是否触发
        /// </summary>
        public bool IsTriggered
        {
            get { return this.m_bIsTriggered; }
            set { this.m_bIsTriggered = value; }
        }
        #endregion
        #region 构造方法
        #endregion
        #region 公共方法
        /// <summary>
        /// 克隆一个handler
        /// </summary>
        /// <returns></returns>
        public StoryMessageHandler Clone()
        {
            StoryMessageHandler handler = new StoryMessageHandler();
            foreach (var cmd in m_listLoadedCommands)
            {
                handler.m_listLoadedCommands.Add(cmd.Clone());
            }
            handler.MessageId = m_sMessageId;
            return handler;
        }
        /// <summary>
        /// 加载命令，存到loaded的链表中
        /// </summary>
        /// <param name="messageHandlerData"></param>
        public void Load(ScriptableData.FunctionData messageHandlerData)
        {
            ScriptableData.CallData callData = messageHandlerData.Call;
            if (callData != null && callData.HaveParam())
            {
                int argsNum = callData.GetParamNum();
                string[] args = new string[argsNum];
                for (int i = 0; i < argsNum; i++)
                {
                    args[i] = callData.GetParamId(i);
                }
                m_sMessageId = string.Join(":", args);
            }
            RefreshCommand(messageHandlerData);
        }
        public void Reset()
        {
            m_bIsTriggered = false;
            foreach (var cmd in m_queueCommandQueue)
            {
                cmd.Reset();
            }
            m_queueCommandQueue.Clear();
        }
        public void Prepare()
        {
            Reset();
            foreach (var cmd in m_listLoadedCommands)
            {
                m_queueCommandQueue.Enqueue(cmd);
            }
        }
        public void Analyze(StoryInstance instance)
        {
            foreach (var cmd in m_listLoadedCommands)
            {
                cmd.Analyze(instance);
            }
        }
        /// <summary>
        /// 触发事件，在触发之前先将加载完成的命令加入到队列中，然后执行命令的UpdateArguments方法，具体子类实现
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="args"></param>
        public void Trigger(StoryInstance instance, object[] args)
        {
            Prepare();
            m_bIsTriggered = true;
            m_oArguments = args;
            foreach (var cmd in m_queueCommandQueue)
            {
                cmd.Prepare(instance, args.Length, args);
            }
        }
        public void Tick(StoryInstance instance, long delta)
        {
            while (m_queueCommandQueue.Count > 0)
            {
                IStoryCommand cmd = m_queueCommandQueue.Peek();
                if (cmd.Execute(instance, delta))
                {
                    break;
                }
                else
                {
                    cmd.Reset();
                    m_queueCommandQueue.Dequeue();
                }
            }
            if (m_queueCommandQueue.Count == 0)
            {
                m_bIsTriggered = false;
            }
        }
        #endregion
        #region 私有方法
        private void RefreshCommand(ScriptableData.FunctionData handlerData)
        {
            m_listLoadedCommands.Clear();
            foreach (var data in handlerData.Statements)
            {
                IStoryCommand cmd = StoryCommandManager.singleton.CreateCommand(data);
                if (cmd != null)
                {
                    m_listLoadedCommands.Add(cmd);
                }
            }
        }
        #endregion
        #region 析构方法
        #endregion
    }
}