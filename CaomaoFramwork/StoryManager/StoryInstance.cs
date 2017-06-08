using UnityEngine;
using System.Collections.Generic;
namespace CaomaoFramework
{
    #region 模块信息
    /*----------------------------------------------------------------
    // 模块名：StoryInstance 
    // 创建者：chen
    // 修改者列表：
    // 创建日期：2016.10.12
    // 模块描述：具体的剧情类
    //----------------------------------------------------------------*/
    #endregion
    /// <summary>
    /// 具体的剧情类
    /// </summary>
    public sealed class StoryInstance
    {
        #region 字段
        //剧情id
        private int m_iStoryId = 0;
        //是否终止
        private bool m_bIsTerminated = false;
        //内容
        private object m_oContext = null;

        private long m_lCurtime = 0;
        private long m_lLastTickTime = 0;
        //剧情消息队列
        private Queue<StoryMessageInfo> m_queueMessageInfoQueue = new Queue<StoryMessageInfo>();
        //剧情消息事情处理器
        private List<StoryMessageHandler> m_listMessageHandlers = new List<StoryMessageHandler>();
        //本地剧情变量
        private Dictionary<string, object> m_dicLocalVariables = new Dictionary<string, object>();
        private Dictionary<string, object> m_dicGlobalVariables = new Dictionary<string, object>();
        #endregion
        #region 属性
        public int StoryId
        {
            get { return this.m_iStoryId; }
        }
        public bool IsTerminated
        {
            get { return this.m_bIsTerminated; }
            set { this.m_bIsTerminated = value; }
        }
        public object Context
        {
            get { return this.m_oContext; }
            set { this.m_oContext = value; }
        }
        public Dictionary<string, object> LocalVariables
        {
            get { return this.m_dicLocalVariables; }
        }
        public Dictionary<string, object> GlobalVariables
        {
            get { return this.m_dicGlobalVariables; }
            set { this.m_dicGlobalVariables = value; }
        }
        #endregion
        #region 构造方法
        #endregion
        #region 公共方法
        public StoryInstance Clone()
        {
            StoryInstance instance = new StoryInstance();
            foreach (var key in this.m_dicLocalVariables.Keys)
            {
                instance.m_dicLocalVariables.Add(key, this.m_dicLocalVariables[key]);
            }
            foreach (var handler in this.m_listMessageHandlers)
            {
                instance.m_listMessageHandlers.Add(handler);
            }
            instance.m_iStoryId = m_iStoryId;
            return instance;
        }
        public bool Init(ScriptableData.ScriptableDataInfo config)
        {
            bool ret = false;
            ScriptableData.FunctionData story = config.First;
            if (story != null && (story.GetId() == "story" || story.GetId() == "script"))
            {
                ret = true;
                //参数
                ScriptableData.CallData callData = story.Call;
                if (callData != null && callData.HaveParam())
                {
                    //第一个参数是剧情的id
                    m_iStoryId = int.Parse(callData.GetParamId(0));
                }
                foreach (var info in story.Statements)
                {
                    if (info.GetId() == "local")
                    {
                        ScriptableData.FunctionData sectionData = info as ScriptableData.FunctionData;
                        if (null != sectionData)
                        {
                            foreach (ScriptableData.ISyntaxComponent def in sectionData.Statements)
                            {
                                ScriptableData.CallData defData = def as ScriptableData.CallData;
                                if (null != defData && defData.HaveId() && defData.HaveParam())
                                {
                                    string id = defData.GetId();
                                    if (id.StartsWith("@") && !id.StartsWith("@@"))
                                    {
                                        StoryValue val = new StoryValue();
                                        val.InitFromDsl(defData.GetParam(0));
                                        if (!m_dicLocalVariables.ContainsKey(id))
                                        {
                                            m_dicLocalVariables.Add(id, val.Value);
                                        }
                                        else
                                        {
                                            m_dicLocalVariables[id] = val.Value;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError("剧情" + m_iStoryId + "DSL语法,必须是个函数");
                        }
                    }
                    else if (info.GetId() == "onmessage")
                    {
                        ScriptableData.FunctionData sectionData = info as ScriptableData.FunctionData;
                        if (null != sectionData)
                        {
                            StoryMessageHandler handler = new StoryMessageHandler();
                            handler.Load(sectionData);
                            m_listMessageHandlers.Add(handler);
                        }
                        else
                        {
                            Debug.LogError("剧情" + m_iStoryId + "DSL语法，必须是个函数");
                        }
                    }
                    else
                    {
                        Debug.LogError("StoryInstance::Init，不知道DSL语法部分:" + info.GetId());
                    }
                }
            }
            else
            {
                Debug.LogError("StoryInstance::Init，不是一个DSL语法");
            }
            return ret;
        }
        public void Reset()
        {
            m_bIsTerminated = false;
            m_queueMessageInfoQueue.Clear();
            foreach (var handler in m_listMessageHandlers)
            {
                handler.Reset();
            }
        }
        public void Start()
        {
            m_lLastTickTime = 0;
            m_lCurtime = 0;
            SendMessage("start");
        }
        public void SendMessage(string msgId, params object[] args)
        {
            StoryMessageInfo msgInfo = new StoryMessageInfo();
            msgInfo.m_sMsgId = msgId;
            msgInfo.m_oArgs = args;
            m_queueMessageInfoQueue.Enqueue(msgInfo);
        }
        public void Analyze()
        {
            foreach (var handler in m_listMessageHandlers)
            {
                handler.Analyze(this);
            }
        }
        public void Tick(long curTime)
        {
            long delta = 0;
            if (m_lLastTickTime == 0)
            {
                m_lLastTickTime = curTime;
            }
            else
            {
                delta = curTime - m_lLastTickTime;
                m_lLastTickTime = curTime;
                m_lCurtime += delta;
            }
            int ct = m_listMessageHandlers.Count;
            if (m_queueMessageInfoQueue.Count > 0)
            {
                int cantTriggerCount = 0;
                int triggerCount = 0;
                StoryMessageInfo msgInfo = m_queueMessageInfoQueue.Peek();
                for (int ix = ct - 1; ix >= 0; --ix)
                {
                    StoryMessageHandler handler = m_listMessageHandlers[ix];
                    if (handler.MessageId == msgInfo.m_sMsgId)
                    {
                        if (handler.IsTriggered)
                        {
                            ++cantTriggerCount;
                        }
                        else
                        {
                            handler.Trigger(this, msgInfo.m_oArgs);
                            ++triggerCount;
                        }
                    }
                }
                if (cantTriggerCount == 0 || triggerCount > 0)
                {
                    m_queueMessageInfoQueue.Dequeue();
                }
            }
            for (int ix = ct - 1; ix >= 0; --ix)
            {
                StoryMessageHandler handler = m_listMessageHandlers[ix];
                if (handler.IsTriggered)
                {
                    handler.Tick(this, delta);
                }
            }
        }
        #endregion
        #region 私有方法
        #endregion
        #region 析构方法
        #endregion
    }
}