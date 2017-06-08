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
    public class UIManager : Singleton<UIManager>, ISerializationCallbackReceiver
    {
        public EUIManagerType m_eUIType = EUIManagerType.e_FairyGUI;
        [SerializeField]
        public Dictionary<string, UIBase> m_dicUIs = new Dictionary<string, UIBase>();
        [SerializeField]
        private string m_serialzedUIManager;
        [SerializeField]
        private bool m_bHasDesrialized;
        [SerializeField]
        private List<UnityEngine.Object> m_uiObjectReference;
        public UIManager()
        {

        }

        public void Init()
        {
            foreach (var ui in this.m_dicUIs.Values)
            {
                ui.Init();
                if (ui.IsResident())
                {
                    ui.PreLoad();
                }
            }
        }
        public void Update(float deltaTime)
        {
            foreach (var ui in this.m_dicUIs.Values)
            {
                if (ui.IsVisible())
                {
                    ui.Update(deltaTime);
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

        public void OnBeforeSerialize()
        {
            if (m_uiObjectReference != null && m_uiObjectReference.Any(o => o != null))
            {
                this.m_bHasDesrialized = false;
            }
#if UNITY_EDITOR
            if (Application.isPlaying) return;
            m_serialzedUIManager = this.Serialize(false, m_uiObjectReference);
#endif
        }

        public void OnAfterDeserialize()
        {
            m_bHasDesrialized = true;
            this.Deserialize(m_serialzedUIManager, false, m_uiObjectReference);
        }
        public string Serialize(bool pretyJson, List<UnityEngine.Object> objectReferences)
        {
            if (objectReferences == null)
            {
                objectReferences = this.m_uiObjectReference = new List<UnityEngine.Object>();
            }
            else
            {
                objectReferences.Clear();
            }
            return JSONSerializer.Serialize(typeof(DataSerializerUIManagare), new DataSerializerUIManagare(this), pretyJson, objectReferences);
        }
        public DataSerializerUIManagare Deserialize(string serializedUIManager, bool validate, List<UnityEngine.Object> objectReferences)
        {
            if (string.IsNullOrEmpty(serializedUIManager))
            {
                return null;
            }
            if (objectReferences == null)
            {
                objectReferences = this.m_uiObjectReference;
            }
            try
            {
                var data = JSONSerializer.Deserialize<DataSerializerUIManagare>(serializedUIManager, objectReferences);
                if (LoadUIManagerData(data, validate) == true)
                {
                    this.m_serialzedUIManager = serializedUIManager;
                    return data;
                }
                return null;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }
        private bool LoadUIManagerData(DataSerializerUIManagare data, bool validate)
        {
            if (data == null)
            {
                Debug.LogError("Can't Load graph, cause of null GraphSerializationData provided");
                return false;
            }
            this.m_dicUIs = new Dictionary<string, UIBase>(data.dicUis);
            return true;
        }
    }
}
