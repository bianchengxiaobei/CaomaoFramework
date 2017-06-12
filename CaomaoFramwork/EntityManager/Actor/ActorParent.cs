using System;
using System.Collections;
using UnityEngine;
using CaomaoFramework.EntityActor;
namespace CaomaoFramework
{
    public class ActorParent<T> : ActorParent where T : EntityParent
    {
        private T m_entity;
        public T Entity
        {
            get { return this.m_entity; }
            set { this.m_entity = value; }
        }
        public override EntityParent GetEntity()
        {
            return this.m_entity;
        }
    }
    public class ActorParent : MonoBehaviour
    {
        #region 字段
        private MecanimEvent m_mecanimEvent;//动作事件触发器
        public Animator m_animator;
        protected string preActName = "";//上个动画名称
        public Action<string, string> ActChangeHandle;
        private Action<string, bool> m_animationStateChanged;
        public Action<string, bool> AnimationStateChanged
        {
            get
            {
                return this.m_animationStateChanged;
            }
            set
            {
                this.m_animationStateChanged = value;
            }
        }
        #endregion
        #region 属性
        #endregion
        #region 构造方法
        #endregion
        #region 子类重写方法
        public virtual EntityParent GetEntity()
        {
            return null;
        }
        public virtual void Awake()
        {
            m_animator = GetComponent<Animator>();
            if (m_animator)
            {
                m_mecanimEvent = new MecanimEvent(m_animator);

            }
        }
        public virtual void ActChange()
        {
            if (null == this.m_animator)
            {
                return;
            }
            if (this.m_animator.IsInTransition(0))//如果正在过渡期间
            {
                return;
            }
            var state = this.m_animator.GetCurrentAnimatorClipInfo(0);
            if (0 == state.Length)
            {
                return;
            }
            string curActName = state[0].clip.name;
            if (curActName != preActName)
            {
                if (ActChangeHandle != null)
                {
                    ActChangeHandle(preActName, curActName);//动作变换
                }
                Debug.Log("cur:"+curActName);
                Debug.Log("pre:" + preActName);
                m_animator.SetInteger("Action", 0);
                preActName = curActName;
            }
            
        }
        public virtual void Attack(int spellID)
        {

        }
        public virtual void Idle()
        {

        }
        public virtual void OnHit(int spellID)
        {

        }
        public virtual void Walk()
        {

        }
        public virtual void AddUpdateAction(int id, Action<float> ac)
        {

        }
        public virtual void RemoveUpdateAction(int id, Action<float> ac)
        {

        }
        #region 包装基于帧的回调函数
        /// <summary>
        /// 添加基于帧的回调函数
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <param name="frameCount">默认为3帧</param>
        public void AddCallbackInFrames(Action callback, int frameCount = 3)
        {
            StartCoroutine(CallbackInFrames(callback, frameCount));
        }
        public void AddCallbackInFrames<T>(Action<T> callback, T arg1, int frameCount = 3)
        {
            StartCoroutine(CallbackInFrames(callback, arg1, frameCount));
        }
        public void AddCallbackInFrames<T, U>(Action<T, U> callback, T arg1, U arg2, int frameCount = 3)
        {
            StartCoroutine(CallbackInFrames(callback, arg1, arg2, frameCount));
        }
        public void AddCallbackInFrames<T, U, V>(Action<T, U, V> callback, T arg1, U arg2, V arg3, int frameCount = 3)
        {
            StartCoroutine(CallbackInFrames(callback, arg1, arg2, arg3, frameCount));
        }
        public void AddCallbackInFrames<T, U, V, W>(Action<T, U, V, W> callback, T arg1, U arg2, V arg3, W arg4, int frameCount = 3)
        {
            StartCoroutine(CallbackInFrames(callback, arg1, arg2, arg3, arg4, frameCount));
        }
        #endregion
        #endregion
        #region 私有方法
        private void StartCheckAnimationChange()
        {
            if (this && this.gameObject.activeSelf)
            {
                StartCoroutine("CheckAnimationChange");
            }
        }
        private IEnumerator CheckAnimationChange()
        {
            return m_mecanimEvent.CheckAnimationChange(OnStateChanged);
        }
        private void OnStateChanged(string name, bool isStart)
        {
            if (this.AnimationStateChanged != null)
            {
                this.AnimationStateChanged(name, isStart);
            }

        }
        /// <summary>
        /// 基于帧的回调函数
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <param name="frameCount">帧数</param>
        /// <returns></returns>
        private IEnumerator CallbackInFrames(Action callback, int frameCount)
        {
            int n = 0;
            while (n < frameCount)
            {
                yield return new WaitForFixedUpdate();
                n += 1;
            }
            callback();
        }
        private IEnumerator CallbackInFrames<T>(Action<T> callback, T arg1, int frameCount)
        {
            int n = 0;
            while (n < frameCount)
            {
                yield return new WaitForFixedUpdate();
                n += 1;
            }
            callback(arg1);
        }
        private IEnumerator CallbackInFrames<T, U>(Action<T, U> callback, T arg1, U arg2, int frameCount)
        {
            int n = 0;
            while (n < frameCount)
            {
                yield return new WaitForFixedUpdate();
                n += 1;
            }
            callback(arg1, arg2);
        }
        private IEnumerator CallbackInFrames<T, U, V>(Action<T, U, V> callback, T arg1, U arg2, V arg3, int frameCount)
        {
            int n = 0;
            while (n < frameCount)
            {
                yield return new WaitForFixedUpdate();
                n += 1;
            }
            callback(arg1, arg2, arg3);
        }
        private IEnumerator CallbackInFrames<T, U, V, W>(Action<T, U, V, W> callback, T arg1, U arg2, V arg3, W arg4, int frameCount)
        {
            int n = 0;
            while (n < frameCount)
            {
                yield return new WaitForFixedUpdate();
                n += 1;
            }
            callback(arg1, arg2, arg3, arg4);
        }
        #endregion
    }
}
