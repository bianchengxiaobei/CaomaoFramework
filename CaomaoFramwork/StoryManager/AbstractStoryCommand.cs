using UnityEngine;
using System.Collections;
namespace CaomaoFramework
{
    #region 模块信息
    /*----------------------------------------------------------------
    // 模块名：AbstractStoryCommand 
    // 创建者：chen
    // 修改者列表：
    // 创建日期：2016.10.12
    // 模块描述：抽象剧情命令基类
    //----------------------------------------------------------------*/
    #endregion
    /// <summary>
    /// 抽象剧情命令基类
    /// </summary>
    public abstract class AbstractStoryCommand : IStoryCommand
    {
        #region 字段
        //上个命令执行的结果
        private bool m_bLastExecResult = false;
        //是否是复合命令
        private bool m_bIsCompositeCommand = false;
        #endregion
        #region 属性
        /// <summary>
        /// 是否是复合命令
        /// </summary>
        public bool IsCompositeCommand
        {
            get { return this.m_bIsCompositeCommand; }
            set { this.m_bIsCompositeCommand = value; }
        }
        #endregion
        #region 子类重写方法 
        public abstract IStoryCommand Clone();
        protected virtual void ResetState() { }
        protected virtual void UpdateArguments(object iterator, object[] args) { }
        protected virtual void UpdateVariables(StoryInstance instance) { }
        protected virtual bool ExecCommand(StoryInstance instance, long delta) { return false; }
        protected virtual void SemanticAnalyze(StoryInstance instance) { }
        protected virtual void Load(ScriptableData.CallData callData) { }
        protected virtual void Load(ScriptableData.FunctionData funcData) { }
        protected virtual void Load(ScriptableData.StatementData stateData) { }
        #endregion
        #region 公共方法
        /// <summary>
        /// 初始化剧情命令
        /// </summary>
        /// <param name="config"></param>
        public void Init(ScriptableData.ISyntaxComponent config)
        {
            ScriptableData.CallData callData = config as ScriptableData.CallData;
            if (callData != null)
            {
                Load(callData);
            }
            else
            {
                ScriptableData.FunctionData funcData = config as ScriptableData.FunctionData;
                if (funcData != null)
                {
                    Load(funcData);
                }
                else
                {
                    ScriptableData.StatementData stateData = config as ScriptableData.StatementData;
                    if (stateData != null)
                    {
                        Load(stateData);
                    }
                    else
                    {
                        Debug.LogError("剧情命令不能解析");
                    }
                }
            }
        }

        public void Reset()
        {
            this.m_bLastExecResult = false;
            this.ResetState();
        }
        public void Prepare(StoryInstance instance, object iterator, object[] args)
        {
            UpdateArguments(iterator, args);
        }
        public bool Execute(StoryInstance instance, long delta)
        {
            if (!m_bLastExecResult || m_bIsCompositeCommand)
            {
                UpdateVariables(instance);
            }
            m_bLastExecResult = this.ExecCommand(instance, delta);
            return m_bLastExecResult;
        }
        public void Analyze(StoryInstance instance)
        {
            this.SemanticAnalyze(instance);
        }
        #endregion
        #region 私有方法
        #endregion
        #region 析构方法
        #endregion
    }
}