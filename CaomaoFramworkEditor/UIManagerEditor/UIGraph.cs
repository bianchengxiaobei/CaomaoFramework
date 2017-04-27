using System;
using System.Collections.Generic;
using CaomaoFramework;
using UnityEngine;
public class UIGraph : Graph
{
    [SerializeField]
    public EUIManagerType uiPluginType;

    public override Type baseNodeType
    {
        get
        {
            return typeof(UINode);
        }
    }
}
