using UnityEngine;
using System;
using System.Collections.Generic;
namespace CaomaoFramework
{
    #region 模块信息
    /*----------------------------------------------------------------
    // 模块名：StoryManager 
    // 创建者：chen
    // 修改者列表：
    // 创建日期：2016.10.12
    // 模块描述：游戏剧情管理器
    //----------------------------------------------------------------*/
    #endregion
    /// <summary>
    /// 游戏剧情管理器
    /// </summary>
    public class StoryManager : Singleton<StoryManager>
    {
        #region 字段
        private Dictionary<string, object> m_dicGlobalVariables = new Dictionary<string, object>();
        private List<StoryInstanceInfo> m_listStoryLogicInfos = new List<StoryInstanceInfo>();
        private Dictionary<int, List<StoryInstanceInfo>> m_dicStoryInstancePool = new Dictionary<int, List<StoryInstanceInfo>>();
        private long m_fStartTime = System.DateTime.Now.Ticks / 10;
        #region 剧情内部类
        private class StoryInstanceInfo
        {
            public int m_iStoryId;
            public StoryInstance m_oInstance;
            public bool m_bIsUsed;
        }
        #endregion
        #endregion
        #region 属性
        /// <summary>
        /// 激活的剧情的数目
        /// </summary>
        public int ActiveStoryCount
        {
            get { return this.m_listStoryLogicInfos.Count; }
        }
        #endregion
        #region 构造方法
        public StoryManager()
        {

        }
        #endregion
        #region 公共方法
        public void Init()
        {

        }
        public void Enter()
        {

        }
        /// <summary>
        /// 开始剧情
        /// </summary>
        /// <param name="storyId"></param>
        public void StartStory(int storyId)
        {
            StoryInstanceInfo instance = NewStoryInstance(storyId);
            if (instance != null)
            {
                m_listStoryLogicInfos.Add(instance);
                instance.m_oInstance.GlobalVariables = m_dicGlobalVariables;
                instance.m_oInstance.Start();
            }
        }
        /// <summary>
        /// 停止剧情
        /// </summary>
        /// <param name="storyId"></param>
        public void StopStory(int storyId)
        {
            int count = m_listStoryLogicInfos.Count;
            for (int index = count - 1; index >= 0; index--)
            {
                StoryInstanceInfo info = m_listStoryLogicInfos[index];
                if (info.m_iStoryId == storyId)
                {
                    RecycleStoryInstanceInfo(info);
                    m_listStoryLogicInfos.RemoveAt(index);
                }
            }
        }
        public void Tick()
        {
            long time = DateTime.Now.Ticks / 10 - this.m_fStartTime;
            int num = m_listStoryLogicInfos.Count;
            for (int index = num - 1; index >= 0; index--)
            {
                StoryInstanceInfo info = m_listStoryLogicInfos[index];
                info.m_oInstance.Tick(time);
                if (info.m_oInstance.IsTerminated)
                {
                    RecycleStoryInstanceInfo(info);
                    m_listStoryLogicInfos.RemoveAt(index);
                }
            }
        }
        public void SendMessage(string MsgId, params object[] args)
        {
            int num = m_listStoryLogicInfos.Count;
            for (int index = num - 1; index >= 0; index++)
            {
                StoryInstanceInfo info = m_listStoryLogicInfos[index];
                info.m_oInstance.SendMessage(MsgId, args);
            }
        }
        private StoryInstanceInfo NewStoryInstance(int storyId)
        {
            //去剧情缓冲池里面找实例
            StoryInstanceInfo instInfo = GetUnusedStoryInstanceInfoFromPool(storyId);
            if (instInfo == null)
            {
                //这里应该是现在角色所在的场景id
                DataSceneConfig cfg = GameData<DataSceneConfig>.dataMap[storyId];
                if (cfg != null)
                {
                    string[] filePath = new string[1] { cfg.StoryDSLFile };
                    for (int i = 0; i < 1; i++)
                    {
                        filePath[i] = Application.dataPath + "/Resources/" + cfg.StoryDSLFile;
                    }
                    StoryConfigManager.singleton.LoadStoryIfNotExist(storyId, 0, filePath);
                    StoryInstance inst = StoryConfigManager.singleton.NewStoryInstance(storyId, 0);
                    if (inst == null)
                    {
                        Debug.LogError("不能加载剧情:" + storyId);
                        return null;
                    }
                    StoryInstanceInfo info = new StoryInstanceInfo();
                    info.m_iStoryId = storyId;
                    info.m_oInstance = inst;
                    info.m_bIsUsed = true;

                    AddStoryInstanceInfoToPool(storyId, info);
                    return info;
                }
                else
                {
                    Debug.LogError("不能找到剧情" + storyId + "配置文件");
                    return null;
                }
            }
            else
            {
                instInfo.m_bIsUsed = true;
                return instInfo;
            }
        }
        public void SendMessage(string objName, string msg, object arg, bool needReceiver = false)
        {
            GameObject obj = GameObject.Find(objName);
            if (obj != null)
            {
                try
                {
                    obj.SendMessage(msg, arg, needReceiver ? SendMessageOptions.RequireReceiver : SendMessageOptions.DontRequireReceiver);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        #endregion
        #region 私有方法
        private void AddStoryInstanceInfoToPool(int storyId, StoryInstanceInfo info)
        {
            if (m_dicStoryInstancePool.ContainsKey(storyId))
            {
                List<StoryInstanceInfo> list = m_dicStoryInstancePool[storyId];
                list.Add(info);
            }
            else
            {
                List<StoryInstanceInfo> list1 = new List<StoryInstanceInfo>();
                list1.Add(info);
                m_dicStoryInstancePool.Add(storyId, list1);
            }
        }
        private StoryInstanceInfo GetUnusedStoryInstanceInfoFromPool(int storyId)
        {
            StoryInstanceInfo info = null;
            if (m_dicStoryInstancePool.ContainsKey(storyId))
            {
                var infos = m_dicStoryInstancePool[storyId];
                foreach (var inst in infos)
                {
                    if (!inst.m_bIsUsed)
                    {
                        info = inst;
                        break;
                    }
                }
            }
            return info;
        }
        private void RecycleStoryInstanceInfo(StoryInstanceInfo info)
        {
            info.m_oInstance.Reset();
            info.m_bIsUsed = false;
        }
        #endregion
        #region 析构方法
        #endregion
    }
}