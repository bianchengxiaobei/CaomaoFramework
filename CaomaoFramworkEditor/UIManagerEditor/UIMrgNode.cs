using System;
using System.Collections.Generic;
[Name("UIManager")]
[Icon("ClientState/StateMrgIcon")]
[Description("游戏UI界面管理器节点，管理着所有UI界面节点的激活和不激活，需要通过连线的方式")]
public class UIMrgNode : UINode
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
            return typeof(UIConnection);
        }
    }
}
