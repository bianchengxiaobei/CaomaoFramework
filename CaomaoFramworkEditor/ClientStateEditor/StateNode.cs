using System;
using System.Collections.Generic;
[Name("ClientNameState")]
[Icon("ClientState/StateIcon")]
public class StateNode : Node, IClientGameState
{
    private ClientGameState m_state;
    public ClientGameState State
    {
        get
        {
            return m_state;
        }

        set
        {
            m_state = value;
        }
    }
}
