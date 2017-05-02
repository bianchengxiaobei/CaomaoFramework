using System;
using System.Collections.Generic;

namespace CaomaoFramework
{
    [Serializable]
    public class DataSerializerUIManagare
    {
        private readonly float SerializationVersion = 2.3f;
        public float version;
        public Dictionary<string, UIBase> dicUis = new Dictionary<string, UIBase>();
        public DataSerializerUIManagare(UIManager uiManager)
        {
            this.version = SerializationVersion;
            this.dicUis = uiManager.m_dicUIs;
        }
    }
}
