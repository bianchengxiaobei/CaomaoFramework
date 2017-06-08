using System;
using System.Collections.Generic;
using CaomaoFramework.Extenstion;
using CaomaoFramework.Effect;
using CaomaoFramework.Data;
namespace CaomaoFramework
{
    public class EffectManager
    {
        #region 字段
        private EntityParent theOwner;
        #endregion
        #region 属性
        public Dictionary<int, List<uint>> sfxTimerIDDic { get; protected set; }
        #endregion
        #region 构造方法
        public EffectManager(EntityParent owner)
        {
            this.theOwner = owner;
            sfxTimerIDDic = new Dictionary<int, List<uint>>();
        }
        #endregion
        #region 公有方法
        public void RemoveEffect(int actionID)
        {
            var sfxs = sfxTimerIDDic.GetValueOrDefault(actionID, new List<uint>());
            foreach (var item in sfxs)
            {
                FrameTimerManager.DelTimer(item);
            }
            Dictionary<int, float> sfx = SkillActionData.dataMap[actionID].effects;
            if (null == sfx)
            {
                return;
            }
            EffectHandler handler = theOwner.effectHandler;
            foreach (var item in sfx)
            {
                if (item.Key < 1000)
                {
                    StopUIFx(item.Key);
                }
                else
                {
                    handler.RemoveFx(item.Key);
                }
            }
        }
        /// <summary>
        /// 播放特效
        /// </summary>
        /// <param name="actionId"></param>
        public void PlayEffect(int actionId)
        {
            if (!SkillAction.dataMap.ContainsKey(actionId))
            {
                return;
            }
            Dictionary<int, float> sfx = SkillAction.dataMap[actionId].sfx;
            EffectHandler sfxHandler = theOwner.effectHandler;
            if (sfx != null && sfx.Count > 0)
            {
                if (!sfxTimerIDDic.ContainsKey(actionId))
                {
                    sfxTimerIDDic.Add(actionId, new List<uint>());
                }
                foreach (var pair in sfx)
                {
                    ///如果actionId小于1000的话是UI特效
                    if (pair.Key < 1000)
                    {
                        sfxTimerIDDic[actionId].Add(FrameTimerManager.AddTimer((uint)(1000 * pair.Value), 0, PlayUIFx, pair.Key));
                    }
                    else
                    {
                        sfxTimerIDDic[actionId].Add(FrameTimerManager.AddTimer((uint)(1000 * pair.Value), 0, TriggerCue, sfxHandler, pair.Key));
                    }
                }
            }
        }
        /// <summary>
        /// 清除所有特效
        /// </summary>
        public void ClearAllSfx()
        {
            foreach (var sfx in sfxTimerIDDic)
            {
                RemoveEffect(sfx.Key);
            }
            sfxTimerIDDic.Clear();
        }
        /// <summary>
        /// 播放UI特效
        /// </summary>
        /// <param name="actionId"></param>
        public void PlayUIFx(int actionId)
        {

        }
        /// <summary>
        /// 停止播放UI特效
        /// </summary>
        /// <param name="actionId"></param>
        public void StopUIFx(int actionId)
        {

        }
        /// <summary>
        /// 播放特效
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="actionId"></param>
        public void TriggerCue(EffectHandler handler, int actionId)
        {
            if (handler)
            {
                handler.HandleFx(actionId);
            }
        }
        #endregion
        #region 私有方法
        #endregion
    }
}

