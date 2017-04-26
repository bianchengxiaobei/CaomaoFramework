using System;
using System.Collections.Generic;

public class UIGraph : Graph
{
    public override Type baseNodeType
    {
        get
        {
            return typeof(UINode);
        }
    }
}
