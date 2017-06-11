using System;
using System.Collections.Generic;
using UnityEngine;

namespace CaomaoFramework
{
    public class GameControllerBase
    {
        /// <summary>
        /// int=>场景中模型GUID
        /// </summary>
        public static Dictionary<int, EntityParent> gameObjects = new Dictionary<int, EntityParent>();
        /// <summary>
        /// 主角
        /// </summary>
        public static EntityParent thePlayer;
        /// <summary>
        /// 世界节点
        /// </summary>
        public static Transform WorldPoint;
        /// <summary>
        /// 游戏从暂停恢复的时间点
        /// </summary>
        public static float StartTime = 0;
        public static EntityParent m_currentEntity;
        public static Dictionary<uint, EntityParent> entities = new Dictionary<uint, EntityParent>();
    }
}
