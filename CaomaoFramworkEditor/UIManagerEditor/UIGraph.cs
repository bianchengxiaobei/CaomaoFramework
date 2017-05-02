using System;
using System.Collections.Generic;
using CaomaoFramework;
using UnityEngine;
public class UIGraph : Graph
{
    [SerializeField]
    public EUIManagerType uiPluginType;
    [SerializeField]
    public static Dictionary<string, UIBase> uiDics = new Dictionary<string, UIBase>();
    public override Type baseNodeType
    {
        get
        {
            return typeof(UINode);
        }
    }
}
