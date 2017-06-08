using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CaomaoFramework.Resource;
using CaomaoFramework.Scene;
namespace CaomaoFramework
{
    public class SceneManager : Singleton<SceneManager>
    {
        private SceneBase m_oCurrentScene = null;
        private AsyncOperation m_oAsyncOperation;
        private IAssetRequest m_sceneRequest = null;
        private Action m_actionOnLoadSceneFinish = null;//加载场景完成的委托
        public Action OnMapChangeFinish = null;
        private bool m_bLoadSceneFinished = false;//是否加载完成
        private GameObject m_objMapParent;
        private Transform worldPoint;
        public void Init(Transform point)
        {
            this.worldPoint = point;
        }
        public SceneBase CreateScene(int _mapId,SceneBase _currentScene,Action loadSceneCallback = null)
        {
            if (this.m_oCurrentScene != null)
            {
                this.m_oCurrentScene.Dispose();
            }
            this.m_bLoadSceneFinished = false;
            this.RegisterLoadSceneFinishCallback(new Action(this.OnLoadSceneFinish));
            if (loadSceneCallback != null)
            {
                this.RegisterLoadSceneFinishCallback(loadSceneCallback);
            }
            this.m_oCurrentScene = _currentScene;
            if (!this.m_oCurrentScene.CreateMap(_mapId))
            {
                return null;
            }
            else
            {
                return this.m_oCurrentScene;
            }
        }
        public void LoadScene()
        {
            if (null == this.m_oCurrentScene)
            {
                Debug.LogError("null == this.currentScene");
            }
            else
            {
                MapData data = GameData<MapData>.dataMap[this.m_oCurrentScene.MapId];
                if (data != null)
                {
                    ConstructMap(data.InitPos, data.InitRotation, data.MapPath);
                    this.EnterScene(data.SceneName);
                }
            }
        }

        public void ChangeMap(int _mapId,Action changeMapCallback)
        {
            if (this.m_oCurrentScene != null)
            {
                if (!this.m_oCurrentScene.ChangeMap(_mapId))
                {
                    Debug.LogError("ChangeMap Error:"+_mapId);
                }
                MapData data = GameData<MapData>.dataMap[this.m_oCurrentScene.MapId];
                if (data != null)
                {
                    if (changeMapCallback != null)
                    {
                        this.RegisterChangeMapFinishCallback(changeMapCallback);
                    }
                    ConstructMap(data.InitPos, data.InitRotation, data.MapPath);
                }
            }
        }
        public void SaveData()
        {

        }
        private void EnterScene(string sceneName)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals(sceneName))
            {
                Debug.LogError("重复加载:" + sceneName);
            }
            else
            {
                UnityMonoDriver.s_instance.StartCoroutine(loadScene(sceneName));
            }
        }
        private IEnumerator loadScene(string strSceneName)
        {
            this.m_oAsyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(strSceneName);
            yield return this.m_oAsyncOperation;
            try
            {
                if (this.m_oAsyncOperation.isDone)
                {
                    if (null != this.m_actionOnLoadSceneFinish)
                    {
                        this.m_actionOnLoadSceneFinish();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            this.m_actionOnLoadSceneFinish = null;
            if (this.m_sceneRequest != null)
            {
                this.m_sceneRequest.Dispose();
            }
            yield return null;
        }
        private void ConstructMap(Vector3 _initPos,Vector3 _initRotate,string _mapPath)
        {
            if (this.m_objMapParent == null)
            {
                this.m_objMapParent = new GameObject("Maps");
                this.m_objMapParent.layer = UnityEngine.LayerMask.NameToLayer("Map");
                if (this.worldPoint != null)
                {
                    this.m_objMapParent.transform.SetParent(this.worldPoint);
                }
                GameObject.DontDestroyOnLoad(this.m_objMapParent);
            }
            if (this.m_objMapParent != null)
            {
                GameObject obj = ResourceManager.singleton.Load(_mapPath) as GameObject;
                GameObject map = GameObject.Instantiate(obj, this.m_objMapParent.transform);
                if (map != null)
                {
                    map.transform.localPosition = _initPos;
                    map.transform.localEulerAngles = _initRotate;
                    this.m_oCurrentScene.CurrentMap.RealMapObj = map;
                    if (this.OnMapChangeFinish != null)
                    {
                        this.OnMapChangeFinish();
                    }
                    this.OnMapChangeFinish = null;
                }
            }
            
        }
        /// <summary>
        /// 注册加载场景完成之后的委托
        /// </summary>
        /// <param name="actionCallback"></param>
        public void RegisterLoadSceneFinishCallback(Action actionCallback)
        {
            this.m_actionOnLoadSceneFinish = (Action)Delegate.Combine(this.m_actionOnLoadSceneFinish, actionCallback);
        }
        public void RegisterChangeMapFinishCallback(Action actionCallback)
        {
            this.OnMapChangeFinish = (Action)Delegate.Combine(this.OnMapChangeFinish, actionCallback);
        }
        private void OnLoadSceneFinish()
        {
            this.m_bLoadSceneFinished = true;                    
        }
    }
}
