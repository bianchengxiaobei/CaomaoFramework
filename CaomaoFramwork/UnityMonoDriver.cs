using System;
using System.Collections.Generic;
using UnityEngine;
namespace CaomaoFramework
{
    public class UnityMonoDriver : MonoBehaviour
    {
        public static UnityMonoDriver s_instance = null;

        public int targetFrameRate;

        public ClientGameStateManager clientGameStateManager = ClientGameStateManager.singleton;

        public ResourceManager resourceManager = ResourceManager.singleton;

        public SDKPlatformManager sdkManager = SDKPlatformManager.singleton;

        public UIManager uiManager = UIManager.singleton;


        private void Awake()
        {
            Application.targetFrameRate = targetFrameRate;
            UnityMonoDriver.s_instance = this;
            if (base.transform.parent != null)
            {
                DontDestroyOnLoad(base.transform.parent);
            }
            //InvokeRepeating("Tick", 2f, 0.1f);
            resourceManager.Init(GameObject.Find("ResourceManager").GetComponent<GameResourceManager>());
            uiManager.Init();
        }
        private void Start()
        {
            sdkManager.Init();
            sdkManager.Install();
            clientGameStateManager.m_oClientStateMachine.Init();
            clientGameStateManager.EnterDefaultState();
        }
        private void Update()
        {
            sdkManager.Update();
            resourceManager.Update();
            uiManager.Update(Time.deltaTime);
        }
        private void Tick()
        {
            //StoryManager.singleton.Tick();
        }
    }
}
