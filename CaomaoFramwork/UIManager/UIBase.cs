using System;
using System.Collections.Generic;
using UnityEngine;
namespace CaomaoFramework
{
    public abstract class UIBase
    {
        protected Transform mRoot;//UI根目录
        protected string mResName;         //资源名
        protected bool mResident;          //是否常驻
        protected bool mVisible = false;   //是否可见


        //类对象初始化
        public abstract void Init();

        //类对象释放
        public abstract void Realse();

        //窗口控制初始化
        protected abstract void InitWidget();

        //窗口控件释放
        protected abstract void RealseWidget();

        //游戏事件注册
        protected abstract void OnAddListener();

        //游戏事件注消
        protected abstract void OnRemoveListener();

        //显示初始化
        public abstract void OnEnable();

        //隐藏处理
        public abstract void OnDisable();

        //每帧更新
        public virtual void Update(float deltaTime) { }

        //是否已打开
        public bool IsVisible() { return mVisible; }

        //是否常驻
        public bool IsResident() { return mResident; }

        //显示
        public void Show()
        {
            if (mRoot == null)
            {
                Create();
            }
            else if (mRoot && mRoot.gameObject.activeSelf == false)
            {
                mRoot.SetAsFirstSibling();
                mRoot.gameObject.SetActive(true);
                mVisible = true;

                OnEnable();

                OnAddListener();
            }
        }

        //隐藏
        public void Hide()
        {
            if (mRoot && mRoot.gameObject.activeSelf == true)
            {
                OnRemoveListener();
                OnDisable();

                if (mResident)
                {
                    mRoot.gameObject.SetActive(false);
                }
                else
                {
                    RealseWidget();
                    Destroy();
                }
            }

            mVisible = false;
        }

        //预加载
        public void PreLoad()
        {
            if (mRoot == null)
            {
                Create();
            }
        }

        //延时删除
        public void DelayDestory()
        {
            if (mRoot)
            {
                RealseWidget();
                Destroy();
            }
        }

        //创建窗体
        protected virtual void Create()
        {
            if (mRoot)
            {
                Debug.LogError("Window Create Error Exist!");
            }

            if (mResName == null || mResName == "")
            {
                Debug.LogError("Window Create Error ResName is empty!");
            }
            GameObject uiObj = null;
            ResourceManager.singleton.LoadUI(this.mResName,(request)=>
            {
                if (request != null)
                {
                    uiObj = GameObject.Instantiate(request.AssetResource.MainAsset as GameObject,Vector3.zero,Quaternion.identity,
                        GameObject.FindGameObjectWithTag("UIRoot").transform);
                    if (uiObj == null)
                    {
                        Debug.LogError("Window Create Error LoadRes WindowName = " + mResName);
                    }
                    mRoot = uiObj.transform;

                    mRoot.gameObject.SetActive(false);//设置为隐藏

                    InitWidget();
                    if (!IsResident())
                    {
                        mRoot.SetAsFirstSibling();
                        mRoot.gameObject.SetActive(true);
                        mVisible = true;
                        OnEnable();
                        OnAddListener();
                    }
                }
            });      
        }

        //销毁窗体
        protected void Destroy()
        {
            if (mRoot)
            {
                // LoadUiResource.DestroyLoad(mRoot.gameObject);
                GameObject.Destroy(mRoot.gameObject);
                mRoot = null;
            }
        }

        //取得根节点
        public Transform GetRoot()
        {
            return mRoot;
        }
    }
}
