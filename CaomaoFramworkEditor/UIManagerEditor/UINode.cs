using System;
using System.Collections.Generic;
public class UINode : Node
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
    public override int maxInConnections
    {
        get
        {
            return 10;
        }
    }
    public override int maxOutConnections
    {
        get
        {
            return 10;
        }
    }
    public override Type outConnectionType
    {
        get
        {
            return typeof(StateConnection);
        }
    }
}