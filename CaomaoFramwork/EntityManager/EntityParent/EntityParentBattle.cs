using System;
using System.Collections.Generic;
using CaomaoFramework.Data;
using CaomaoFramework.EntityFsm;
using UnityEngine;
namespace CaomaoFramework
{
    public partial class EntityParent
    {
        #region 字段
        #endregion
        #region 属性
        #endregion
        #region 子类重写方法
        public virtual void OnDeath(int hitActionId)
        {
            if (this.m_battleManager != null)
            {
                this.m_battleManager.OnDead(hitActionId);
            }
        }
        public virtual void OnAttacking(int actionID, Matrix4x4 ltwm, Quaternion rotation, Vector3 forward, Vector3 position)
        {
            if (this.m_battleManager != null)
            {
                this.m_battleManager.OnAttacking(actionID, ltwm, rotation, forward, position);
            }
        }
        public virtual void OnAttacking(int actionId)
        {

        }
        /// <summary>
        /// 带方向释放技能
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="rotation"></param>
        public virtual void CastSkill(int skillId, Vector3 rotation)
        {

        }
        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="skillId"></param>
        public virtual void CastSkill(int skillId)
        {

        }
        public virtual void CastSkill(string skillName)
        {

        }
        /// <summary>
        /// 清除技能,到Idle状态
        /// </summary>
        /// <param name="remove"></param>
        /// <param name="naturalEnd"></param>
        public virtual void ClearSkill(bool remove = false)
        {

        }
        public virtual void ActionChange(string preActName,string currActName)
        {

        }
        #endregion
        #region 公有方法
        /// <summary>
        /// 当前动画动作的名称
        /// </summary>
        /// <returns></returns>
        public string CurrActStateName()
        {
            if (null == this.animator)
            {
                return "";
            }
            var state = this.animator.GetCurrentAnimatorClipInfo(0);
            if (state.Length == 0)
            {
                return "";
            }
            return state[0].clip.name;
        }
        #endregion
        #region 私有方法
        #endregion
    }
}

