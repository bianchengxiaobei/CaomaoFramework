using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace CaomaoFramework
{
    public enum EUIManagerType
    {
        e_UGUI,
        e_FairyGUI,
        e_NGUI
    }
    [System.Serializable]
    public class UIManager : Singleton<UIManager>
    {
        public EUIManagerType m_eUIType = EUIManagerType.e_FairyGUI;
        [SerializeField]
        public Dictionary<string, UIBase> m_dicUIs = new Dictionary<string, UIBase>();

        public UIManager()
        {
            
        }

        public void Init()
        {
            m_dicUIs.Clear();
            foreach (var ui in this.m_dicUIs.Values)
            {
                ui.Init();
                if (ui.IsResident())
                {
                    ui.PreLoad();
                }
            }
        }
        public UIBase GetUI(string type)
        {
            if (m_dicUIs.ContainsKey(type))
            {
                return m_dicUIs[type];
            }
            else
            {
                return null;
            }
        }
    }
}
