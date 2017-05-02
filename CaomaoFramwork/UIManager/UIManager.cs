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
    public class UIManager : Singleton<UIManager>,ISerializationCallbackReceiver
    {
        public EUIManagerType m_eUIType = EUIManagerType.e_FairyGUI;
        [SerializeField]
        public Dictionary<string, UIBase> m_dicUIs = new Dictionary<string, UIBase>();
        [SerializeField]
        private string m_serialzedUIManager;
        [SerializeField]
        private bool m_bHasDesrialized;
        [SerializeField]
        private List<UnityEngine.Object> m_objectReference;
        public UIManager()
        {
            
        }

        public void Init()
        {
            Debug.Log(m_dicUIs.Count);
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

        public void OnBeforeSerialize()
        {
            if (m_objectReference != null && m_objectReference.Any(o => o != null))
            {
                this.m_bHasDesrialized = false;
            }
#if UNITY_EDITOR
            if (JSONSerializer.applicationPlaying) return;
            m_serialzedUIManager = this.Serialize(false, m_objectReference);
#endif
        }

        public void OnAfterDeserialize()
        {
            if (m_bHasDesrialized && JSONSerializer.applicationPlaying) return;
            m_bHasDesrialized = true;
            this.Deserialize(m_serialzedUIManager, false, m_objectReference);
        }
        public string Serialize(bool pretyJson, List<UnityEngine.Object> objectReferences)
        {
            if (objectReferences == null)
            {
                objectReferences = this.m_objectReference = new List<UnityEngine.Object>();
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
                objectReferences = this.m_objectReference;
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
