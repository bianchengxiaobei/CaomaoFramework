using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class ClientGameState
{
    [NonSerialized]
    private string m_stateTypeName;
    /// <summary>
    /// 游戏状态类型
    /// </summary>
    public string Name
    {
        get
        {
            if (string.IsNullOrEmpty(m_stateTypeName))
            {
                var nameAtt = this.GetType().RTGetAttribute<NameAttribute>(false);
                m_stateTypeName = nameAtt != null ? nameAtt.name : GetType().FullName;
            }
            return this.m_stateTypeName;
        }
    }
}
