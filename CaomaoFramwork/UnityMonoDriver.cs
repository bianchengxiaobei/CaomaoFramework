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
            InvokeRepeating("Tick", 2f, 0.1f);
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
            TimerManager.Tick();
        }
        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                if (GameControllerBase.thePlayer != null)
                {
                    GameControllerBase.thePlayer.m_skillManager.Compensation(prePay);
                    TimerManager.AddTimer(1000, 0, () => { prePay = 0; });
                }
            }
        }
        private float prePause = 0;
        private float prePay = 0;
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                prePause = Time.realtimeSinceStartup;
            }
            else
            {
                float cur = Time.realtimeSinceStartup;
                GameControllerBase.StartTime = cur;
                float pay = cur - prePause;
                if (GameControllerBase.thePlayer != null)
                {
                    GameControllerBase.thePlayer.m_skillManager.Compensation(-prePay);
                    prePay = 0;
                    GameControllerBase.thePlayer.m_skillManager.Compensation(pay);
                }
            }
        }
    }
}
