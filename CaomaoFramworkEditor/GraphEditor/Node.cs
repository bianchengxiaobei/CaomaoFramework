using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public abstract class Node
{
    /// <summary>
    /// 连接线的端口
    /// </summary>
    class GUIPort
    {
        public readonly int portIndex;
        public readonly Node parent;
        public readonly Vector2 pos;

        public GUIPort(int index, Node parent, Vector2 pos)
        {
            this.portIndex = index;
            this.parent = parent;
            this.pos = pos;
        }
    }
    [SerializeField]
    private Vector2 _position = Vector2.zero;
    private Vector2 size = new Vector2(100, 20);
    private Graph m_graph = null;

    private int m_id = 0;
    private Texture2D m_icon;
    private bool m_bIconLoaded = false;//是否Icon已经加载
    private List<Connection> _inConnections = new List<Connection>();
    private List<Connection> _outConnections = new List<Connection>();
    private static int dragDropMisses;
    #region 属性
    /// <summary>
    /// 节点ID
    /// </summary>
    public int ID
    {
        get { return this.m_id; }
        set { this.m_id = value; }
    }
    /// <summary>
    /// 节点图标
    /// </summary>
    public Texture2D Icon
    {
        get
        {
            if (m_bIconLoaded)
            {
                return this.m_icon;
            }
            if (m_icon == null)
            {
                var iconAtt = this.GetType().RTGetAttribute<IconAttribute>(true);
                if (iconAtt != null) m_icon = (Texture2D)Resources.Load(iconAtt.iconName);
            }
            this.m_bIconLoaded = true;
            return this.m_icon;
        }
    }
    /// <summary>
    /// 编辑器节点大小位置
    /// </summary>
    public Rect nodeRect
    {
        get { return new Rect(_position.x, _position.y, size.x, size.y); }
        set
        {
            _position = new Vector2(value.x, value.y);
            size = new Vector2(Mathf.Max(value.width, EditorCommonDefine.minSize.x), Mathf.Max(value.height, EditorCommonDefine.minSize.y));
        }
    }
    /// <summary>
    /// 节点位置
    /// </summary>
    public Vector2 nodePosition
    {
        get { return _position; }
        set { _position = value; }
    }
    /// <summary>
    /// 所属的管理器图
    /// </summary>
    public Graph graph
    {
        get
        {
            return this.m_graph;
        }
        set { this.m_graph = value; }
    }
    /// <summary>
    /// 进入该node的连接
    /// </summary>
    public List<Connection> inConnections
    {
        get { return _inConnections; }
        protected set { _inConnections = value; }
    }
    /// <summary>
    ///  从该node出去的连接
    /// </summary>
    public List<Connection> outConnections
    {
        get { return _outConnections; }
        protected set { _outConnections = value; }
    }
    /// <summary>
    /// 是否已经更新过ID
    /// </summary>
    private bool IsUpdate { get; set; }
    /// <summary>
    /// 是否是激活的状态
    /// </summary>
    public bool isActive
    {
        get
        {
            for (var i = 0; i < inConnections.Count; i++)
            {
                if (inConnections[i].isActive)
                {
                    return true;
                }
            }
            return inConnections.Count == 0;
        }
    }
    /// <summary>
    /// node节点被点击
    /// </summary>
    private bool nodeIsPressed { get; set; }
    /// <summary>
    /// 是否被选中
    /// </summary>
    public bool IsSelected
    {
        get
        {
            return Graph.currentSelection == this || Graph.multiSelection.Contains(this);
        }
    }
    /// <summary>
    /// 点击的端口
    /// </summary>
    private static GUIPort clickedPort { get; set; }

    #region 抽象属性
    abstract public Type outConnectionType { get; }
    #endregion
    #endregion
    #region 构造方法
    public Node()
    {

    }
    #endregion
    public void ShowNodeGUI(Vector2 canvasMousePos, float zoomFactor)
    {
        DrawNodeWindow(canvasMousePos, zoomFactor);
    }
    /// <summary>
    /// 设置节点ID
    /// </summary>
    /// <param name="lastID"></param>
    /// <returns></returns>
    public int AssignIDToGraph(int lastID)
    {
        if (IsUpdate)
        {
            return lastID;
        }
        IsUpdate = true;
        lastID++;
        ID = lastID;
        for (var i = 0; i < outConnections.Count; i++)
        {
            lastID = outConnections[i].targetNode.AssignIDToGraph(lastID);
        }
        return lastID;
    }
    /// <summary>
    /// 重新更新ID
    /// </summary>
    public void ResetRecursion()
    {
        if (!IsUpdate)
        {
            return;
        }

        IsUpdate = false;
        for (var i = 0; i < outConnections.Count; i++)
        {
            outConnections[i].targetNode.ResetRecursion();
        }
    }
    /// <summary>
    /// 创建一个节点
    /// </summary>
    /// <param name="targetGraph"></param>
    /// <param name="nodeType"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static Node Create(Graph targetGraph, Type nodeType, Vector2 pos)
    {
        if (targetGraph == null)
        {
            Debug.LogError("不存在Graph");
            return null;
        }
        var newNode = (Node)Activator.CreateInstance(nodeType);
        newNode.graph = targetGraph;
        newNode.nodePosition = pos;
        return newNode;
    }
    private void DrawNodeWindow(Vector2 canvasMousePos, float zoomFactor)
    {
        GUI.color = isActive ? Color.white : new Color(0.9f, 0.9f, 0.9f, 0.8f);
        GUI.color = Graph.currentSelection == this ? new Color(0.9f, 0.9f, 1) : GUI.color;
        nodeRect = GUILayout.Window(ID, nodeRect, NodeWindowGUI, string.Empty, (GUIStyle)"window");
        GUI.Box(nodeRect, "", (GUIStyle)"windowShadow");
        GUI.color = new Color(1, 1, 1, 0.5f);
        GUI.Box(new Rect(nodeRect.x + 6, nodeRect.y + 6, nodeRect.width, nodeRect.height), "", (GUIStyle)"windowShadow");
        if (IsSelected)
        {
            GUI.color = EditorCommonDefine.restingColor;
            GUI.Box(nodeRect, "", (GUIStyle)"windowHighlight");
        }
        GUI.color = Color.white;
        if (Graph.AllowClick)
        {
            EditorGUIUtility.AddCursorRect(new Rect(nodeRect.x * zoomFactor, nodeRect.y * zoomFactor, nodeRect.width * zoomFactor, nodeRect.height * zoomFactor), MouseCursor.Link);
        }
    }
    /// <summary>
    /// 画出连线
    /// </summary>
    /// <param name="canvasMousePos"></param>
    /// <param name="zoomFactor"></param>
    public void DrawNodeConnections(Vector2 canvasMousePos, float zoomFactor)
    {
        var e = Event.current;
        if (clickedPort != null && e.type == EventType.MouseUp && e.button == 0)
        {
            if (nodeRect.Contains(e.mousePosition))
            {
                graph.ConnectNodes(clickedPort.parent, this, clickedPort.portIndex);
                clickedPort = null;
                e.Use();
            }
            else
            {
                dragDropMisses++;
                if (dragDropMisses == graph.allNodes.Count && clickedPort != null)
                {

                }
            }
        }
    }
    /// <summary>
    /// 是否连接被允许
    /// </summary>
    /// <param name="sourceNode"></param>
    /// <returns></returns>
    public bool IsNewConnectionAllowed(Node sourceNode)
    {
        if (this == sourceNode)
        {
            return false;
        }
        //这个暂时还没有想好以后是否用mrg当作一个node来使用
        return true;
    }
    private void NodeWindowGUI(int ID)
    {
        var e = Event.current;
        ShowIcon();
        HandleEvents(e);
        ShowNodeContents();
        HandleNodePosition(e);
    }
    private void ShowIcon()
    {
        GUI.backgroundColor = Color.clear;
        GUILayout.Box(Icon, GUILayout.MaxHeight(50));
        GUI.backgroundColor = Color.white;
        GUI.color = Color.white;
    }
    public void ShowNodeContents()
    {
        GUI.color = Color.white;
        GUI.skin = null;
        GUI.skin.label.richText = true;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.BeginVertical();
        if (this is IClientGameState)
        {
            var state = (this as IClientGameState).State;
            if (state == null)
            {
                GUILayout.Label("当前没有状态");
            }
            else
            {
                GUILayout.Label(string.Format("<b>{0}</b>", state.Name));
            }
        }
        GUILayout.EndVertical();
        GUI.skin.label.alignment = TextAnchor.UpperLeft;
    }
    /// <summary>
    /// 处理节点事件
    /// </summary>
    /// <param name="e"></param>
    private void HandleEvents(Event e)
    {
        if (Graph.AllowClick && e.button != 2 && e.type == EventType.MouseDown)
        {
            if (!e.control)
            {
                Graph.currentSelection = this;
            }
            if (e.control)
            {
                if (IsSelected)
                {
                    Graph.multiSelection.Remove(this);
                }
                else
                {
                    Graph.multiSelection.Add(this);
                }
            }
            if (e.button == 0)
            {
                nodeIsPressed = true;
            }
        }
    }
    /// <summary>
    /// 拖动节点
    /// </summary>
    /// <param name="e"></param>
    private void HandleNodePosition(Event e)
    {
        if (Graph.AllowClick && e.button != 2)
        {
            if (e.type == EventType.MouseDrag && Graph.multiSelection.Count > 1)
            {
                for (var i = 0; i < Graph.multiSelection.Count; i++)
                {
                    ((Node)Graph.multiSelection[i]).nodePosition += e.delta;
                }
                return;
            }

            if (nodeIsPressed)
            {
                if (Graph.multiSelection.Count == 0)
                {
                    nodePosition = new Vector2(Mathf.Round(nodePosition.x / 15) * 15, Mathf.Round(nodePosition.y / 15) * 15);
                }
            }

            GUI.DragWindow();
        }
    }
}