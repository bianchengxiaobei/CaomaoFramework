using System;
using System.Collections;
using UnityEngine;

namespace CaomaoFramework.EntityActor
{
    public class MecanimEvent
    {
        #region 字段
        private Animator m_animator;
        private AnimationClip m_currentMark;
        private float passTime = 0;//记录动画累积播放多长时间
        private bool m_isNotFading = true;
        #endregion
        #region 属性
        #endregion
        #region 构造方法
        public MecanimEvent(Animator animator)
        {
            this.m_animator = animator;
        }
        #endregion
        #region 公有方法
        /// <summary>
        /// 监测动作状态变化
        /// </summary>
        /// <param name="stateChanged">动画状态变化事件委托</param>
        /// <returns></returns>
        public IEnumerator CheckAnimationChange(Action<int, bool> stateChanged)
        {
            while (true)
            {
                var state = this.m_animator.GetCurrentAnimatorStateInfo(0);//public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex);
                passTime += Time.deltaTime;
                if (passTime >= state.length)//动画播放完成之后，执行委托
                {
                    stateChanged(state.fullPathHash, state.loop);
                    passTime = 0;
                    yield return new WaitForFixedUpdate();
                }
                yield return new WaitForFixedUpdate();
            }
        }
        public IEnumerator CheckAnimationChange(Action<string, bool> stateChange)
        {
            while (true)
            {
                //var state = m_animator.GetCurrentAnimationClipState(0);
                var state = m_animator.GetCurrentAnimatorClipInfo(0);
                if (state.Length != 0)
                {
                    if (state[0].weight != 1)//动画正在过渡
                    {
                        if (m_isNotFading)//判断标记是否为过渡
                        {
                            var nextState = m_animator.GetNextAnimatorClipInfo(0);//过渡的下个动画
                            if (stateChange != null && nextState.Length != 0)
                            {
                                stateChange(nextState[0].clip.name, true);
                            }
                            m_currentMark = state[0].clip;//更新当前动画
                            m_isNotFading = false;//标记为过渡完成状态
                            yield return new WaitForFixedUpdate();
                        }
                    }
                    else //表示动画没在过渡
                    {
                        if (m_currentMark != state[0].clip)
                        {
                            if (m_currentMark != null && stateChange != null)
                            {
                                stateChange(m_currentMark.name, false);
                            }
                            m_currentMark = state[0].clip;//更新当前动画
                            m_isNotFading = true;//标记为非过渡状态
                            yield return new WaitForFixedUpdate();
                        }
                    }
                }
                yield return new WaitForFixedUpdate();
            }
        }
        #endregion
        #region 私有方法
        #endregion
    }

}
