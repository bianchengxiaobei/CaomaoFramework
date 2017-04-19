using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public abstract class Connection
{
    [SerializeField]
    private Node m_sourceNode;
    [SerializeField]
    private Node m_targetNode;
    [SerializeField]
    private bool m_isDisabled;

    [System.NonSerialized]
    private ConnectionStatus m_status = ConnectionStatus.Resting;

    public Node sourceNode
    {
        get { return m_sourceNode; }
        protected set { m_sourceNode = value; }
    }

    public Node targetNode
    {
        get { return m_targetNode; }
        protected set { m_targetNode = value; }
    }
    public ConnectionStatus connectionStatus
    {
        get { return m_status; }
        set { m_status = value; }
    }
    protected Graph graph
    {
        get { return sourceNode.graph; }
    }
    /// <summary>
    /// 是否激活
    /// </summary>
    public bool isActive
    {
        get { return !m_isDisabled; }
        set
        {
            if (!m_isDisabled && value == false)
            {
                //Reset();
            }
            m_isDisabled = !value;
        }
    }
    /// <summary>
    /// 创建线条
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="sourceIndex"></param>
    /// <returns></returns>
    public static Connection Create(Node source, Node target, int sourceIndex)
    {
        if (source == null || target == null)
        {
            Debug.LogError("起始节点或者目标节点不存在");
            return null;
        }
        var newConnection = (Connection)Activator.CreateInstance(source.outConnectionType);
        newConnection.sourceNode = source;
        newConnection.targetNode = target;
        source.outConnections.Insert(sourceIndex, newConnection);
        target.inConnections.Add(newConnection);
        return newConnection;
    }
}
