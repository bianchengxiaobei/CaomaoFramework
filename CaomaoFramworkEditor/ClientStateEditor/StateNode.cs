using System;
using System.Collections.Generic;
using CaomaoFramework;
using UnityEditor;
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
    protected override void PostConnectionActiveEvent()
    {
        Type type = EditorTool.GetScriptType(this.scriptName);
        GameStateGraph.stateDics.Add(this.scriptName, Activator.CreateInstance(type) as ClientStateBase);
        EditorUtility.SetDirty(this.graph);
    }
    public override void OnDisConnection()
    {
        if (UIGraph.uiDics.Remove(this.scriptName))
            EditorUtility.SetDirty(this.graph);
    }
}
