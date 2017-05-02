using System;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
public class GameStateGraph : Graph
{
    [SerializeField]
    public static Dictionary<string, ClientStateBase> stateDics = new Dictionary<string, ClientStateBase>();
    public override Type baseNodeType
    {
        get
        {
            return typeof(StateNode);
        }
    }
}