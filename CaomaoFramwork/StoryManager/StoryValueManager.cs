using UnityEngine;
using System.Collections.Generic;
namespace CaomaoFramework
{
    #region 模块信息
    /*----------------------------------------------------------------
    // 模块名：StoryValueManager 
    // 创建者：chen
    // 修改者列表：
    // 创建日期：2016.10.12
    // 模块描述：剧情用到的值管理器
    //----------------------------------------------------------------*/
    #endregion
    /// <summary>
    /// 剧情用到的值管理器
    /// </summary>
    public class StoryValueManager : Singleton<StoryValueManager>
    {
        private Dictionary<string, IStoryValueFactory> m_dicValueHandlers = new Dictionary<string, IStoryValueFactory>();

        public void RegisterValueHandler(string name, IStoryValueFactory handler)
        {
            if (!m_dicValueHandlers.ContainsKey(name))
            {
                m_dicValueHandlers.Add(name, handler);
            }
            else
            {
                //error
            }
        }
        public IStoryValue<object> CalcValue(ScriptableData.ISyntaxComponent param)
        {
            if (param.IsValid() && param.GetId().Length == 0)
            {
                //处理括弧
                ScriptableData.CallData callData = param as ScriptableData.CallData;
                if (null != callData && callData.GetParamNum() > 0)
                {
                    int ct = callData.GetParamNum();
                    return CalcValue(callData.GetParam(ct - 1));
                }
                else
                {
                    //不支持的语法
                    return null;
                }
            }
            else
            {
                IStoryValue<object> ret = null;
                string id = param.GetId();
                if (m_dicValueHandlers.ContainsKey(id))
                {
                    ret = m_dicValueHandlers[id].Build(param);
                }
                return ret;
            }
        }
    }
}