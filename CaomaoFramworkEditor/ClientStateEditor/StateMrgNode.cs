using System;
using System.Collections.Generic;
[Name("ClientNameStateManager")]
[Icon("ClientState/StateMrgIcon")]
[Description("游戏状态管理器节点，管理着所有状态节点的激活和不激活，需要通过连线的方式")]
public class StateMrgNode : StateNode
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
