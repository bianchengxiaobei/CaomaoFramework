using System;
using System.Collections.Generic;
using UnityEngine;

namespace CaomaoFramework
{
    public class GameMotor : MonoBehaviour
    {
        /// <summary>
        /// 是否使用输入设备
        /// </summary>
        public bool enableStick = true;
        /// <summary>
        /// 碰撞标记
        /// </summary>
        protected CollisionFlags collisionFlags = CollisionFlags.CollidedBelow;
        /// <summary>
        /// 是否正在跳跃
        /// </summary>
        protected bool isFlying = false;
        /// <summary>
        /// 是否正在翻滚
        /// </summary>
        public bool isRolling = false;
        /// <summary>
        /// 重力值
        /// </summary>
        public float gravity = 0f;
        /// <summary>
        /// 是否能够移动
        /// </summary>
        public bool canMove = true;
        /// <summary>
        /// 是否向目标移动
        /// </summary>
        public bool isMovingToTarget = false;
        /// <summary>
        /// 移动目的地
        /// </summary>
        public Vector3 targetToMoveTo;
        /// <summary>
        /// 是否是在移动
        /// </summary>
        public bool isMovable;
        /// <summary>
        /// 速度
        /// </summary>
        public float speed = 0f;
        /// <summary>
        /// 附加速度
        /// </summary>
        public float extraSpeed = 0f;
        /// <summary>
        /// 目标速度
        /// </summary>
        public float targetSpeed;
        /// <summary>
        /// 加速度
        /// </summary>
        public float acceleration = 2;
        /// <summary>
        /// 转向y的值
        /// </summary>
        public float RotationY = 0f;
        /// <summary>
        /// 旋转速度
        /// </summary>
        private float acturalAngularSpeed = 1440f;

        private float angularSpeed = 1440f;
        /// <summary>
        /// 选择速度比例，默认为1
        /// </summary>
        private float angularSpeedRate = 1.0f;
        /// <summary>
        /// 移动方向3D
        /// </summary>
        public Vector3 moveDir3D = Vector3.zero;
        /// <summary>
        /// 移动方向2D
        /// </summary>
        public Vector2 moveDir2D = Vector2.zero;
        public Vector2 jumpPos = Vector2.zero;
        public float maxJumpHeight = 0.8f;
        public float minJumpHeight = 0.2f;
        public float timeToJumpApex = 0.4f;
        [HideInInspector]
        public float maxJumpVelocity;
        [HideInInspector]
        public float minJumpVelocity;
        /// <summary>
        /// 是否启用输入设备控制人物面朝
        /// </summary>
        public bool enableRotation = true;
        /// <summary>
        /// 是否能转向
        /// </summary>
        public bool canTurn = false;
        /// <summary>
        /// 是否能设置转向
        /// </summary>
        public bool IsRotationTo = false;
        /// <summary>
        /// 是否正在旋转
        /// </summary>
        public bool IsTurning = false;
        /// <summary>
        /// 是否看着某个方向
        /// </summary>
        protected bool isLookingAtTarget = false;
        /// <summary>
        /// 看着某个方向值
        /// </summary>
        protected Vector3 targetToLookAt;

        /// <summary>
        /// 设置附加速度
        /// </summary>
        /// <param name="extra"></param>
        public void SetExtraSpeed(float extra)
        {
            extraSpeed = extra;
        }
        /// <summary>
        /// 设置速度
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetSpeed(float value)
        {
            this.speed = value;
        }
        /// <summary>
        /// 根据加速度acceleration，缓慢增加速度
        /// </summary>
        /// <param name="originalSpeed"></param>
        /// <param name="_targetSpeed"></param>
        /// <returns></returns>
        protected float AccelerateSpeed(float originalSpeed, float _targetSpeed)
        {
            if (Mathf.Abs(originalSpeed - _targetSpeed) < acceleration * Time.deltaTime)
            {
                originalSpeed = _targetSpeed;
            }
            else if (originalSpeed - _targetSpeed > 0)
            {
                originalSpeed -= acceleration * Time.deltaTime;
            }
            else
            {
                originalSpeed += acceleration * Time.deltaTime;
            }
            return originalSpeed;
        }
        #region 子类重写
        /// <summary>
        /// 通过Navmesh来寻路
        /// </summary>
        /// <param name="v"></param>
        /// <param name="stopDistance"></param>
        /// <param name="needToAdjustPosY"></param>
        /// <returns></returns>
        public virtual bool MoveToByNav(Vector2 v, float stopDistance = 0f, bool needToAdjustPosY = true)
        {
            return false;
        }
        /// <summary>
        /// 移动到指定目的点
        /// </summary>
        /// <param name="v"></param>
        /// <param name="needToAdjustPosY"></param>
        public virtual void MoveTo(Vector2 target, bool needToAdjustPosY = true)
        {

        }
        public virtual void RotateTo(float targetAngleY)
        {
            if (!canTurn)
            {
                return;
            }
            IsRotationTo = true;
            RotationY = targetAngleY;
        }
        /// <summary>
        /// 停止寻路
        /// </summary>
        public virtual void StopNav()
        {

        }
        /// <summary>
        /// 跳跃
        /// </summary>
        public virtual void Jump()
        {

        }
        /// <summary>
        /// 按下力度较下就JumpMin的高度
        /// </summary>
        public virtual void JumpMin()
        { 

        }
        public virtual void Roll()
        {

        }
        /// <summary>
        /// 设置是否在跳跃状态，子类选择关闭一些输入操作
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetFlying(bool value)
        {
            this.isFlying = value;
        }
        /// <summary>
        /// 传送角色到目的地
        /// </summary>
        /// <param name="destination"></param>
        public virtual void TeleportTo(Vector3 destination)
        {
            transform.position = destination;
        }
        /// <summary>
        /// 是否在地面上
        /// </summary>
        /// <returns></returns>
        public virtual bool IsOnGround()
        {
            return true;
        }
        #endregion
    }
}
