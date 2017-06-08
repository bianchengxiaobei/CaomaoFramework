using System;
using System.Collections.Generic;

namespace CaomaoFramework
{
    [Serializable]
    public class DataSerializerUIManagare
    {
        public Dictionary<string, UIBase> dicUis = new Dictionary<string, UIBase>();
        public DataSerializerUIManagare(UIManager uiManager)
        {
            this.dicUis = uiManager.m_dicUIs;
        }
    }
}
