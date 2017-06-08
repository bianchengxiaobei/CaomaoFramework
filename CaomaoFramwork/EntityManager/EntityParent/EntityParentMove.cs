using System;
using System.Collections.Generic;
using CaomaoFramework.EntityFsm;
using UnityEngine;
namespace CaomaoFramework
{ 
    public partial class EntityParent
    {
        public virtual void Move()
        {
            if (m_battleManager == null)
            {
                ChangeMotionState(MotionState.WALKING);
            }
            else
            {
                this.m_battleManager.Move();
            }
        }
        public virtual void MoveTo(float x, float y, float z)
        {
            if (currentMotionState == MotionState.DEAD)
            {
                return;
            }
            if (motor == null || Mathf.Abs(x - Transform.position.x) < 0.1f && Mathf.Abs(z - Transform.position.z) < 0.1f)
            {
                return;
            }
            Vector3 v = new Vector3(x, y, z);
        }
        public virtual void TurnTo(float x, float y, float z)
        {
            if (currentMotionState == MotionState.DEAD)
            {
                return;
            }
            if (motor)
            {
                motor.RotateTo(y);
            }
        }
        public virtual void MoveTo(float x, float z, float dx, float dy, float dz)
        {
            if (!Transform)
                return;
            if (currentMotionState == MotionState.DEAD) return;
            if (Mathf.Abs(x - Transform.position.x) < 0.1f && Mathf.Abs(z - Transform.position.z) < 0.1f)
            {
                TurnTo(dx, dy, dz);
            }
            else
            {
                MoveTo(x, z);
            }
        }
        public virtual void MoveTo(float x, float z)
        {
            if (currentMotionState == MotionState.DEAD) return;
            MoveTo(x, 0, z);
        }
    }
}
