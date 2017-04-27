using System;
using System.Collections.Generic;
[Name("ClientState")]
[Icon("ClientState/StateIcon")]
[Description("每个游戏状态管理着该状态的进入和退出，进入该干什么事情，退出该干什么事情")]
public class StateNode : Node
{
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
