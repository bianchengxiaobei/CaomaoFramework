using System;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework.GameState;
namespace CaomaoFramework
{
    public class UnityMonoDriver : MonoBehaviour
    {
        private static UnityMonoDriver s_instance = null;

        public int targetFrameRate;

        public ClientGameStateManager clientGameStateManager;

        private void Awake()
        {
            Application.targetFrameRate = targetFrameRate;
            UnityMonoDriver.s_instance = this;
            if (base.transform.parent != null)
            {
                DontDestroyOnLoad(base.transform.parent);
            }

        }
    }
}
