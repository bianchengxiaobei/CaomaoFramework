using System;
using System.Collections.Generic;
using UnityEngine;
namespace CaomaoFramework
{
    public class UnityMonoDriver : MonoBehaviour
    {
        public static UnityMonoDriver s_instance = null;

        public int targetFrameRate;

        public ClientGameStateManager clientGameStateManager;

        public ResourceManager resourceManager;

        public SDKPlatformManager sdkManager;

        public UIManager uiManager;


        private void Awake()
        {
            Application.targetFrameRate = targetFrameRate;
            UnityMonoDriver.s_instance = this;
            if (base.transform.parent != null)
            {
                DontDestroyOnLoad(base.transform.parent);
            }
            uiManager.Init();
        }
        private void Start()
        {
            sdkManager.Init();
            sdkManager.Install();
        }
        private void Update()
        {
            sdkManager.Update();
        }
    }
}
