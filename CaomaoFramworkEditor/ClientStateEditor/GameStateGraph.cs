using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class GameStateGraph : Graph
{
    public override Type baseNodeType
    {
        get
        {
            return typeof(StateNode);
        }
    }
}