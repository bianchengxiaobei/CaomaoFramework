using System;
using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework.Extenstion;
using CaomaoFramework.Resource;

namespace CaomaoFramework
{
    public partial class EntityParent
    {
        #region 字段
        #endregion
        #region 属性
        #endregion
        #region 公有方法
        /// <summary>
        /// 播放技能特效
        /// </summary>
        /// <param name="spellId"></param>
        public void PlaySfx(int spellId)
        {
            if (effectManager == null)
            {
                return;
            }
            effectManager.PlayEffect(spellId);
        }
        /// <summary>
        /// 移除技能特效
        /// </summary>
        /// <param name="spellId"></param>
        public void RemoveSfx(int spellId)
        {
            if (effectManager == null)
            {
                return;
            }
            effectManager.RemoveEffect(spellId);
        }
        /// <summary>
        /// 客户端增加增益buff
        /// </summary>
        /// <param name="id">buff的id</param>
        public void ClientAddBuff(int id)
        {

        }
        /// <summary>
        /// 客户端增加减益buff
        /// </summary>
        /// <param name="id"></param>
        public void ClientDelBuff(int id)
        {

        }
        /// <summary>
        /// 取得int类型的属性值
        /// </summary>
        /// <param name="arrtName"></param>
        /// <returns></returns>
        public int GetInAttr(string attrName)
        {
            return intAttrs.GetValueOrDefault(attrName, 0);
        }
        /// <summary>
        /// 收集挂载点信息
        /// </summary>
        /// <param name="rootRenderNode"></param>
        /// <returns></returns>
        public int GatherRenderNodeInfo(GameObject rootRenderNode)
        {
            if (null == rootRenderNode)
            {
                return 0;
            }
            List<Transform> lst = new List<Transform>();
            UnityToolBase.FindAllTransform(rootRenderNode.transform, lst);
            int count = 0;
            foreach (var t in lst)
            {
                this.renderNodeObjects[t.name] = t.gameObject;
                if (t.name == "left_weapon_at" || t.name == "right_weapon_at")
                {
                    bindNodeTable[t.name] = t;
                }
                ++count;
            }
            return count;
        }
        /// <summary>
        /// 角色换装
        /// </summary>
        /// <param name="path"></param>
        /// <param name="partPath"></param>
        /// <param name="anchorName"></param>
        public void ChangeAvatar(string path, string partPath, string anchorName)
        {
            if (null == GameObject)
            {
                return;
            }
            //脱下装备
            if (path.Length == 0)
            {
                ChangeAvatarInteral(null, path, partPath, anchorName);
                return;
            }
            ResourceManager.singleton.LoadModel(path, new AssetRequestFinishedEventHandler((requestAsset) =>
            {
                GameObject obj = requestAsset.AssetResource.MainAsset as GameObject;
                if (obj != null)
                {
                    bool bNeedChange = true;
                    if (partPath == "face")
                    {

                    }
                    if (bNeedChange)
                    {
                        GameObject avatar = ChangeAvatarInteral(obj as GameObject, path, partPath, anchorName);
                        if (avatar == null)
                        {
                            Debug.Log("换装为空");
                        }
                        else
                        {
                            if (!IsVisiable)
                            {
                                Renderer r = avatar.GetComponent<Renderer>();
                                if (r != null)
                                {
                                    r.enabled = false;
                                }
                            }
                        }
                    }
                }
            }));
        }
        protected GameObject ChangeAvatarInteral(GameObject avatarSuit, string path, string partPath, string anchorNodeName)
        {
            if (null == GameObject || renderNodeObjects.Count == 0)
            {
                return null;
            }
            GameObject result = null;
            if (string.IsNullOrEmpty(path))
            {
                if (renderNodeObjects.Contains(partPath))
                {
                    GameObject oldPart = renderNodeObjects[partPath] as GameObject;
                    oldPart.transform.SetParent(null);
                    GameObject.Destroy(oldPart);
                    renderNodeObjects.Remove(partPath);
                    RefreshAvatar();
                }
                return null;
            }
            if (null == avatarSuit)
            {
                Debug.LogError("需要替换的装备资源不存在,请检查加载是否有误");
                return null;
            }
            GameObject newAvatar = GameObject.Instantiate(avatarSuit) as GameObject;
            newAvatar.name = avatarSuit.name;
            bool bNeedDestory = true;
            if (newAvatar != null)
            {
                int childCount = newAvatar.transform.childCount;
                if (newAvatar.name == partPath && childCount == 0)
                {
                    result = newAvatar;
                    ChangeModel(newAvatar.transform, anchorNodeName);
                    bNeedDestory = false;
                }
                else
                {
                    for (int i = 0; i < childCount; i++)
                    {
                        Transform childTrans = newAvatar.transform.GetChild(i);
                        if (childTrans.name == partPath)
                        {
                            result = childTrans.gameObject;
                            ChangeModel(childTrans, anchorNodeName);
                            break;
                        }
                    }
                }
                if (null == result)
                {
                    Debug.LogWarning("找不到指定的骨骼节点替换模型");
                }
            }
            if (bNeedDestory)
            {
                GameObject.Destroy(newAvatar);
            }
            return result;
        }
        public void SetVisiableWhenLoad(bool bVisiable)
        {
            this.IsVisiable = bVisiable;
            if (GameObject)
            {
                Renderer[] rs = GameObject.GetComponentsInChildren<Renderer>();
                foreach (var r in rs)
                {
                    r.enabled = bVisiable;
                }
            }
        }
        #endregion          
        #region 子类重写方法
        public virtual void MainCameraCompleted()
        {

        }
        /// <summary>
        /// 创建游戏实例模型，加到GameWorld的所有实例模型中
        /// </summary>
        public virtual void CreateModel(Action callback = null)
        {
            if (GameObject)
            {
                //GameWorld.GameObjects.Add(GameObject.GetInstanceID(), this);
            }
        }
        public virtual void CreateActualModel(Action callback = null)
        {

        }
        public virtual void CreateDeafaultModel(Action callback = null)
        {

        }
        /// <summary>
        /// 通知角色装备改变了，需要刷新
        /// </summary>
        public virtual void RefreshAvatar()
        {

        }
        /// <summary>
        /// 通知角色骨骼皮肤改变了
        /// </summary>
        /// <param name="kSkinnedMeshRenderer"></param>
        protected virtual void NotifySkinnedMeshChanged(SkinnedMeshRenderer kSkinnedMeshRenderer)
        {

        }
        /// <summary>
        /// 通知角色渲染皮肤改变了
        /// </summary>
        /// <param name="kSkinnedMeshRenderer"></param>
        protected virtual void NotifyStaticMeshChanged(MeshRenderer kSkinnedMeshRenderer)
        {

        }
        /// <summary>
        /// 对象进入场景,初始化各种数据，资源，模型 
        /// </summary>
        public virtual void OnEnterWorld(Action callback = null)
        {

        }
        public virtual void UpdatePosition()
        {

        }
        /// <summary>
        /// 是否应用动画根运动，为ture则动画中可以移动，false不能移动
        /// </summary>
        /// <param name="value"></param>
        public virtual void ApplyRootMotion(bool value)
        {
            if (null == animator)
            {
                return;
            }
            animator.applyRootMotion = value;
        }
        public void SetAction(int act)
        {
            if (null == animator)
            {
                return;
            }
            animator.SetInteger("Action", act);//设置人物动画的状态
            if (weaponAnimator)
            {
                weaponAnimator.SetInteger("Action", act);//设置人物武器动画状态
            }
            if (act == (int)ActionConstants.knock_down)
            {
                m_bKnockDown = true;
            }
            else if (act == (int)ActionConstants.hited_ground)
            {
                m_bHitOnGround = true;
            }
        }
        /// <summary>
        /// 设置动画播放速度
        /// </summary>
        /// <param name="speed"></param>
        public virtual void SetSpeed(float speed)
        {
            if (null == animator)
            {
                return;
            }
            animator.SetFloat("Speed", speed);
        }
        /// <summary>
        /// Idle状态
        /// </summary>
        public virtual void Idle()
        {

        }
        /// <summary>
        /// 改变状态机的新状态
        /// </summary>
        /// <param name="newState">新状态</param>
        /// <param name="args"></param>
        public virtual void ChangeMotionState(string newState, params object[] args)
        {
            fsmMotion.ChangeStatus(this, newState, args);
        }
        #endregion
        #region 包装基于帧的回调函数
        /// <summary>
        /// 添加基于帧的回调函数，默认为3帧
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="frameCount"></param>
        public void AddCallbackInFrames(Action callback, int frameCount = 3)
        {
            if (Actor)
            {
                Actor.AddCallbackInFrames(callback, frameCount);
            }
        }
        public void AddCallbackInFrames<T>(Action<T> callback, T arg1, int frameCount = 3)
        {
            if (Actor)
            {
                Actor.AddCallbackInFrames<T>(callback, arg1, frameCount);
            }
        }
        public void AddCallbackInFrames<T, U>(Action<T, U> callback, T arg1, U arg2, int frameCount = 3)
        {
            if (Actor)
            {
                Actor.AddCallbackInFrames<T, U>(callback, arg1, arg2, frameCount);
            }
        }
        public void AddCallbackInFrames<T, U, W>(Action<T, U, W> callback, T arg1, U arg2, W arg3, int frameCount = 3)
        {
            if (Actor)
            {
                Actor.AddCallbackInFrames<T, U, W>(callback, arg1, arg2, arg3, frameCount);
            }
        }
        public void AddCallbackInFrames<T, U, W, V>(Action<T, U, W, V> callback, T arg1, U arg2, W arg3, V arg4, int frameCount = 3)
        {
            if (Actor)
            {
                Actor.AddCallbackInFrames<T, U, W, V>(callback, arg1, arg2, arg3, arg4, frameCount);
            }
        }
        #endregion
        #region 包装事件监听部分
        /// <summary>
        /// 生成唯一的eventType，用来进行不同实例的消息
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public string MakeUniqMessage(string eventType)
        {
            return string.Concat(eventType, this.ID);
        }
        public virtual void AddUniqEventListener(string eventType, Callback handler)
        {
            EventDispatch.AddListener(MakeUniqMessage(eventType), handler);
        }
        public virtual void AddUniqEventListener<T>(string eventType, Callback<T> handler)
        {
            EventDispatch.AddListener<T>(eventType, handler);
        }
        public virtual void AddUniqEventListener<T, U>(string eventType, Callback<T, U> handler)
        {
            EventDispatch.AddListener<T, U>(eventType, handler);
        }
        public virtual void AddUniqEventListener<T, U, V>(string eventType, Callback<T, U, V> handler)
        {
            EventDispatch.AddListener<T, U, V>(eventType, handler);
        }
        public virtual void AddUniqEventListener<T, U, V, W>(string eventType, Callback<T, U, V, W> handler)
        {
            EventDispatch.AddListener<T, U, V, W>(eventType, handler);
        }
        virtual public void RemoveUniqEventListener(string eventType, Callback handler)
        {
            EventDispatch.RemoveListener(MakeUniqMessage(eventType), handler);
        }
        public virtual void RemoveUniqEventListener<T>(string eventType, Callback<T> handler)
        {
            EventDispatch.RemoveListener<T>(eventType, handler);
        }
        public virtual void RemoveUniqEventListener<T, U>(string eventType, Callback<T, U> handler)
        {
            EventDispatch.RemoveListener<T, U>(eventType, handler);
        }
        public virtual void RemoveUniqEventListener<T, U, V>(string eventType, Callback<T, U, V> handler)
        {
            EventDispatch.RemoveListener<T, U, V>(eventType, handler);
        }
        public virtual void RemoveUniqEventListener<T, U, V, W>(string eventType, Callback<T, U, V, W> handler)
        {
            EventDispatch.RemoveListener<T, U, V, W>(eventType, handler);
        }
        virtual public void TriggerUniqEvent(string eventType)
        {
            EventDispatch.Broadcast(MakeUniqMessage(eventType));
        }
        public virtual void TriggerUniqEvent<T>(string eventType, T arg1)
        {
            EventDispatch.Broadcast<T>(MakeUniqMessage(eventType), arg1);
        }
        public virtual void TriggerUniqEvent<T, U>(string eventType, T arg1, U arg2)
        {
            EventDispatch.Broadcast<T, U>(MakeUniqMessage(eventType), arg1, arg2);
        }
        public virtual void TriggerUniqEvent<T, U, V>(string eventType, T arg1, U arg2, V arg3)
        {
            EventDispatch.Broadcast<T, U, V>(MakeUniqMessage(eventType), arg1, arg2, arg3);
        }
        public virtual void TriggerUniqEvent<T, U, V, W>(string eventType, T arg1, U arg2, V arg3, W arg4)
        {
            EventDispatch.Broadcast<T, U, V, W>(MakeUniqMessage(eventType), arg1, arg2, arg3, arg4);
        }
        #endregion
        #region 私有方法
        private void ChangeModel(Transform childTrans, string anchorName)
        {
            if (childTrans != null)
            {
                SkinnedMeshRenderer skinnedMesh = childTrans.gameObject.GetComponent<SkinnedMeshRenderer>();
                MeshRenderer staticMesh = childTrans.gameObject.GetComponent<MeshRenderer>();
                bool avatarChanged = false;
                if (skinnedMesh != null)
                {
                    skinnedMesh.receiveShadows = false;
                    Transform rootbone = skinnedMesh.rootBone;
                    //Transform oldRoot = UnityTools.FindChildTransform(GameObject.transform, rootbone.name);
                    Transform oldRoot = null;
                    if (oldRoot != null)
                    {
                        //UnityTools.ReplaceNewTran(rootbone, oldRoot);
                    }
                    else
                    {
                        Debug.LogError("找不到替换的骨骼位置");
                    }
                    List<Transform> newTargetBones = new List<Transform>();
                    foreach (Transform srcBone in skinnedMesh.bones)
                    {
                        if (renderNodeObjects.Contains(srcBone.name))
                            newTargetBones.Add((renderNodeObjects[srcBone.name] as GameObject).transform);
                    }

                    skinnedMesh.bones = newTargetBones.ToArray();
                    skinnedMesh.rootBone = (renderNodeObjects[skinnedMesh.rootBone.name] as GameObject).transform;
                    childTrans.parent = GameObject.transform;

                    avatarChanged = true;

                    NotifySkinnedMeshChanged(skinnedMesh);
                }
                else if (staticMesh != null)
                {
                    staticMesh.receiveShadows = false;
                    bool isExist = bindNodeTable.ContainsKey(anchorName);
                    if (isExist)
                    {
                        Transform bindParent = bindNodeTable[anchorName] as Transform;
                        childTrans.parent = bindParent;
                        childTrans.localPosition = new Vector3(0, 0, 0);
                        childTrans.localRotation = Quaternion.identity;
                        childTrans.localScale = Vector3.one;
                        avatarChanged = true;

                        NotifyStaticMeshChanged(staticMesh);    //< 回调函数aa
                    }
                }
                if (avatarChanged)
                {
                    if (renderNodeObjects.Contains(childTrans.name))
                    {
                        GameObject oldAvatarPart = renderNodeObjects[childTrans.name] as GameObject;
                        oldAvatarPart.transform.parent = null;
                        GameObject.Destroy(oldAvatarPart);
                    }
                    renderNodeObjects[childTrans.name] = childTrans.gameObject;
                    meshTable[childTrans.name] = childTrans.gameObject;
                    RefreshAvatar();
                }
            }
        }
        #endregion
    }
    public enum ActionConstants
    {
        town_idle,//城镇站立
        hited,//受击
        hited_ground,//受击倒地
        knock_down,//击飞
        push,//后退
        die,//死亡
        revive,//复活
        die_knock_down,//击飞死亡
        hit_air//浮空
    }
}
