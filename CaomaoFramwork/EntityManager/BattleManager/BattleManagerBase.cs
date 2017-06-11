using System;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework.EntityFsm;
namespace CaomaoFramework
{
    public class BattleManagerBase
    {
        protected EntityParent theOnwer;
        protected SkillManagerBase m_skillManager;

        public BattleManagerBase(EntityParent _theOwner, SkillManagerBase _skillManager)
        {
            this.theOnwer = _theOwner;
            this.m_skillManager = _skillManager;
        }
        /// <summary>
        /// 死亡状态
        /// </summary>
        /// <param name="hitActionId"></param>
        public virtual void OnDead(int hitActionId)
        {
            theOnwer.ChangeMotionState(MotionState.DEAD, hitActionId);
        }
        /// <summary>
        /// 攻击状态，播放攻击动作
        /// </summary>
        /// <param name="nSkillID"></param>
        /// <param name="ltwm"></param>
        /// <param name="rotation"></param>
        /// <param name="forward"></param>
        /// <param name="position"></param>
        public virtual void OnAttacking(int nSkillID, Matrix4x4 ltwm, Quaternion rotation, Vector3 forward, Vector3 position)
        {
            m_skillManager.OnAttacking(nSkillID, ltwm, rotation, forward, position);
        }
        public virtual void OnAttacking(int nSkillID)
        {
            m_skillManager.OnAttacking(nSkillID);
        }
        /// <summary>
        /// 释放技能状态
        /// </summary>
        /// <param name="skillId"></param>
        public virtual void CastSkill(int skillId)
        {
            if (theOnwer.CurrentMotionState == MotionState.DEAD
               || theOnwer.CurrentMotionState == MotionState.HIT
               || theOnwer.CurrentMotionState == MotionState.PICKING)
            {
                return;
            }
            theOnwer.ChangeMotionState(MotionState.ATTACKING, skillId);
        }
        /// <summary>
        /// 行走状态
        /// </summary>
        public virtual void Move()
        {
            if (theOnwer.CurrentMotionState == MotionState.DEAD
                || theOnwer.CurrentMotionState == MotionState.ATTACKING
                || theOnwer.CurrentMotionState == MotionState.HIT
                || theOnwer.CurrentMotionState == MotionState.PICKING)
            {
                return;
            }
            theOnwer.ChangeMotionState(MotionState.WALKING);
        }
        /// <summary>
        /// Idle状态
        /// </summary>
        public virtual void Idle()
        {
            if (theOnwer.CurrentMotionState == MotionState.DEAD
              || theOnwer.CurrentMotionState == MotionState.ATTACKING
              || theOnwer.CurrentMotionState == MotionState.HIT
              || theOnwer.CurrentMotionState == MotionState.PICKING)
            {
                return;
            }
            theOnwer.ChangeMotionState(MotionState.IDLE);
        }
    }
}
