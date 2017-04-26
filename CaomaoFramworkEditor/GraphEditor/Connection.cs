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
    private Rect areaRect = new Rect(0, 0, 50, 10);
    private ConnectionStatus m_status = ConnectionStatus.Resting;
    private Color connectionColor = EditorCommonDefine.restingColor;
    private Vector3 lineFromTangent = Vector3.zero;
    private Vector3 lineToTangent = Vector3.zero;
    private float lineSize = 3;
    private Rect outPortRect;
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
    #region 抽象属性
    virtual protected TipConnectionStyle tipConnectionStyle
    {
        get { return TipConnectionStyle.Circle; }
    }
    virtual protected float defaultSize
    {
        get { return 3f; }
    }
    #endregion
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
    /// <summary>
    /// 画线
    /// </summary>
    /// <param name="lineFrom"></param>
    /// <param name="lineTo"></param>
    public void DrawConnectionGUI(Vector3 lineFrom, Vector3 lineTo)
    {
        var mlt = .8f;
        var tangentX = Mathf.Abs(lineFrom.x - lineTo.x) * mlt;
        var tangentY = Mathf.Abs(lineFrom.y - lineTo.y) * mlt;

        GUI.color = EditorCommonDefine.restingColor;
        var arrowRect = new Rect(0, 0, 15, 15);
        arrowRect.center = lineTo;

        var hor = 0;

        if (lineFrom.x <= sourceNode.nodeRect.x)
        {
            lineFromTangent = new Vector3(-tangentX, 0, 0);
            hor--;
        }

        if (lineFrom.x >= sourceNode.nodeRect.xMax)
        {
            lineFromTangent = new Vector3(tangentX, 0, 0);
            hor++;
        }

        if (lineFrom.y <= sourceNode.nodeRect.y)
            lineFromTangent = new Vector3(0, -tangentY, 0);

        if (lineFrom.y >= sourceNode.nodeRect.yMax)
            lineFromTangent = new Vector3(0, tangentY, 0);


        if (lineTo.x <= targetNode.nodeRect.x)
        {
            lineToTangent = new Vector3(-tangentX, 0, 0);
            hor--;
            if (tipConnectionStyle == TipConnectionStyle.Circle)
                GUI.Box(arrowRect, "", "circle");
            else
            if (tipConnectionStyle == TipConnectionStyle.Arrow)
                GUI.Box(arrowRect, "", "arrowRight");
        }

        if (lineTo.x >= targetNode.nodeRect.xMax)
        {
            lineToTangent = new Vector3(tangentX, 0, 0);
            hor++;
            if (tipConnectionStyle == TipConnectionStyle.Circle)
                GUI.Box(arrowRect, "", "circle");
            else
            if (tipConnectionStyle == TipConnectionStyle.Arrow)
                GUI.Box(arrowRect, "", "arrowLeft");
        }

        if (lineTo.y <= targetNode.nodeRect.y)
        {
            lineToTangent = new Vector3(0, -tangentY, 0);
            if (tipConnectionStyle == TipConnectionStyle.Circle)
                GUI.Box(arrowRect, "", "circle");
            else
            if (tipConnectionStyle == TipConnectionStyle.Arrow)
                GUI.Box(arrowRect, "", "arrowBottom");
        }

        if (lineTo.y >= targetNode.nodeRect.yMax)
        {
            lineToTangent = new Vector3(0, tangentY, 0);
            if (tipConnectionStyle == TipConnectionStyle.Circle)
                GUI.Box(arrowRect, "", "circle");
            else
            if (tipConnectionStyle == TipConnectionStyle.Arrow)
                GUI.Box(arrowRect, "", "arrowTop");
        }

        GUI.color = Color.white;

        ///

        outPortRect = new Rect(0, 0, 12, 12);
        outPortRect.center = lineFrom;

        if (!Application.isPlaying)
        {
            var highlight = Graph.currentSelection == this || Graph.currentSelection == sourceNode || Graph.currentSelection == targetNode;
            connectionColor = EditorCommonDefine.restingColor;
            lineSize = highlight ? defaultSize + 2 : defaultSize;
        }

        connectionColor = isActive ? connectionColor : new Color(0.3f, 0.3f, 0.3f);

        Handles.color = connectionColor;
        var shadow = new Vector3(3.5f, 3.5f, 0);
        Handles.DrawBezier(lineFrom, lineTo + shadow, lineFrom + shadow + lineFromTangent + shadow, lineTo + shadow + lineToTangent, new Color(0, 0, 0, 0.1f), null, lineSize + 10f);
        Handles.DrawBezier(lineFrom, lineTo, lineFrom + lineFromTangent, lineTo + lineToTangent, connectionColor, null, lineSize);

        Handles.color = Color.white;

        var t = 0.5f;
        float u = 1.0f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        Vector3 result = uuu * lineFrom;
        result += 3 * uu * t * (lineFrom + lineFromTangent);
        result += 3 * u * tt * (lineTo + lineToTangent);
        result += ttt * lineTo;
        var midPosition = (Vector2)result;
        areaRect.center = midPosition;
        areaRect.width = 0;
        areaRect.height = 0;
    }
    public void SetTarget(Node newTarget, bool isRelink = true)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.Undo.RecordObject(graph, "Set Target");
        }
#endif

        if (isRelink)
        {
            var i = targetNode.inConnections.IndexOf(this);
            targetNode.inConnections.Remove(this);
        }
        newTarget.inConnections.Add(this);

        targetNode = newTarget;
    }
    #region 抽象方法
    virtual public void OnDestroy() { }
    #endregion
}
