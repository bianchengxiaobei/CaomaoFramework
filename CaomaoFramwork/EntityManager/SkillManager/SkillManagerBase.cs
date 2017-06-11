using System;
using System.Collections.Generic;
using UnityEngine;

namespace CaomaoFramework
{
    public class SkillManagerBase
    {
        protected EntityParent theOwner;
        public SkillManagerBase(EntityParent entity)
        {
            this.theOwner = entity;
        }
        public virtual void AttackEffect(int hitActionID, Matrix4x4 ltwm, Quaternion rotation, Vector3 forward, Vector3 position)
        {

        }
        public virtual void OnAttacking(int hitActionId, Matrix4x4 ltwm, Quaternion rotation, Vector3 forward, Vector3 position)
        {

        }
        public virtual void OnAttacking(int hitActionId)
        {

        }
        public virtual void Compensation(float t)
        {

        }
    }
}
