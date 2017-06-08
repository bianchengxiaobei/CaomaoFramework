using UnityEngine;
using System.Collections.Generic;
namespace CaomaoFramework
{
    #region 模块信息
    /*----------------------------------------------------------------
    // 模块名：StoryConfigManager 
    // 创建者：chen
    // 修改者列表：
    // 创建日期：2016.10.13
    // 模块描述：剧情配置管理器
    //----------------------------------------------------------------*/
    #endregion
    /// <summary>
    /// 剧情配置管理器
    /// </summary>
    public class StoryConfigManager : Singleton<StoryConfigManager>
    {
        #region 字段
        private Dictionary<int, StoryInstance> m_dicStoryInstance = new Dictionary<int, StoryInstance>();
        private object m_oLock = new object();
        #endregion
        #region 属性
        #endregion
        #region 构造方法
        #endregion
        #region 公共方法
        public StoryInstance NewStoryInstance(int storyId, int sceneId)
        {
            StoryInstance instance = null;
            int id = GetId(storyId, sceneId);
            StoryInstance temp = GetStoryInstanceResource(id);
            if (temp != null)
            {
                instance = temp.Clone();
            }
            return instance;
        }
        public void LoadStoryIfNotExist(int storyId, int sceneId, params string[] files)
        {
            if (!ExistStory(storyId, sceneId))
            {
                foreach (var file in files)
                {
                    LoadStory(file, sceneId);
                }
            }
        }
        /// <summary>
        /// 是否已经存在该剧情资源
        /// </summary>
        /// <param name="storyId"></param>
        /// <param name="sceneId"></param>
        /// <returns></returns>
        public bool ExistStory(int storyId, int sceneId)
        {
            int id = GetId(storyId, sceneId);
            return null != GetStoryInstanceResource(id);
        }
        /// <summary>
        /// 加载剧情，根据配置文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="sceneId"></param>
        public void LoadStory(string file, int sceneId)
        {
            if (!string.IsNullOrEmpty(file))
            {
                ScriptableData.ScriptableDataFile dataFile = new ScriptableData.ScriptableDataFile();
                if (dataFile.Load(file))
                {
                    Load(dataFile, sceneId);
                }
                else
                {
                    Debug.Log("不能读取DSL文件");
                }
            }
        }
        public void Clear()
        {
            lock (m_oLock)
            {
                m_dicStoryInstance.Clear();
            }
        }
        #endregion
        #region 私有方法
        private static int GetId(int storyId, int sceneId)
        {
            return sceneId * 100 + storyId;
        }
        private StoryInstance GetStoryInstanceResource(int id)
        {
            StoryInstance instance = null;
            lock (m_oLock)
            {
                if (m_dicStoryInstance.ContainsKey(id))
                {
                    instance = m_dicStoryInstance[id];
                }
            }
            return instance;
        }
        private void Load(ScriptableData.ScriptableDataFile dataFile, int sceneId)
        {
            lock (m_oLock)
            {
                foreach (var info in dataFile.ScriptableDatas)
                {
                    if (info.GetId() == "story" || info.GetId() == "script")
                    {
                        ScriptableData.FunctionData funcData = info.First;
                        if (funcData != null)
                        {
                            ScriptableData.CallData callData = funcData.Call;
                            if (callData != null && callData.HaveParam())
                            {
                                int storyId = int.Parse(callData.GetParamId(0));
                                int id = GetId(storyId, sceneId);
                                if (!m_dicStoryInstance.ContainsKey(id))
                                {
                                    StoryInstance instance = new StoryInstance();
                                    instance.Init(info);
                                    m_dicStoryInstance.Add(id, instance);
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region 析构方法
        #endregion
    }
}
