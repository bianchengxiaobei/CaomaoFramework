using System;
using System.Collections.Generic;

namespace CaomaoFramework.GameState
{ 
    [Serializable]
    public class DataSerializerStateMachine
    {
        public Dictionary<string, ClientStateBase> dicStates = new Dictionary<string, ClientStateBase>();
        public DataSerializerStateMachine(ClientGameStateMachine m)
        {
            dicStates = m.m_dicClientStates;
        }
    }
}
