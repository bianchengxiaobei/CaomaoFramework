using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
[Serializable]
public abstract class Graph : ScriptableObject,ISerializationCallbackReceiver
{
    private float _zoomFactor = 1f;
    private Vector2 _translation = new Vector2(-5000, -5000);
    private Rect inspectorRect = new Rect(15, 55, 0, 0);
    private List<Node> _nodes = new List<Node>();
    private Node _primeNode = null;

    private static object m_currentSelection;
    private static List<object> m_multiSeletion = new List<object>();
    #region 属性
    /// <summary>
    /// 在编辑窗口画布缩放因子
    /// </summary>
    public float zoomFactor
    {
        get { return _zoomFactor; }
        set { _zoomFactor = value; }
    }
    /// <summary>
    /// 编辑窗口的边界
    /// </summary>
    public Vector2 translation
    {
        get { return _translation; }
        set { _translation = value; }
    }
    /// <summary>
    /// 状态节点
    /// </summary>
    public List<Node> allNodes
    {
        get { return _nodes; }
        private set { _nodes = value; }
    }
    /// <summary>
    /// 起始节点
    /// </summary>
    public Node primeNode
    {
        get { return this._primeNode; }
        set { this._primeNode = value; }
    }
    /// <summary>
    /// 选择的节点
    /// </summary>
    public static object currentSelection
    {
        get
        {
            if (multiSelection.Count > 1)
            {
                return null;
            }
            if (multiSelection.Count == 1)
            {
                return multiSelection[0];
            }
            return m_currentSelection;
        }
        set
        {
            if (!multiSelection.Contains(value))
            {
                multiSelection.Clear();
            }
            GUIUtility.keyboardControl = 0;
            SceneView.RepaintAll();
        }
    }
    /// <summary>
    /// 选择多个节点
    /// </summary>
    public static List<object> multiSelection
    {
        get
        {
            return m_multiSeletion;
        }
        set
        {
            if (value.Count == 1)
            {
                currentSelection = value[0];
                value.Clear();
            }
            m_multiSeletion = value;
        }
    }
    /// <summary>
    /// 是否可以点击
    /// </summary>
    public static bool AllowClick
    {
        get;set;
    }
    #endregion
    #region 抽象属性
    public Type baseNodeType { get { return typeof(Node); } }
    #endregion
    /// <summary>
    /// 显示NodeGUI
    /// </summary>
    /// <param name="e"></param>
    /// <param name="drawCanvas"></param>
    /// <param name="fullDrawPass"></param>
    /// <param name="canvasMousePos"></param>
    /// <param name="zoomFactor"></param>
    public void ShowNodesGUI(Event e, Rect drawCanvas, bool fullDrawPass, Vector2 canvasMousePos, float zoomFactor)
    {
        GUI.color = Color.white;
        GUI.backgroundColor = Color.white;
        Debug.Log("r3r");
        this.UpdateNodeIDs(false);

        for (var i = 0; i < allNodes.Count; i++)
        {
            Debug.Log("r3rdwdw");
            if (fullDrawPass || drawCanvas.Overlaps(allNodes[i].nodeRect))
            {
                Debug.Log("rdwqdqw3r");
                allNodes[i].ShowNodeGUI(canvasMousePos, zoomFactor);
            }
        }
    }
    /// <summary>
    /// 更新节点ID
    /// </summary>
    /// <param name="alsoReorderList"></param>
    public void UpdateNodeIDs(bool alsoReorderList)
    {
        var lastID = 0;

        if (primeNode != null)
        {
            lastID = primeNode.AssignIDToGraph(lastID);
        }
        for (var i = 0; i < allNodes.Count; i++)
        {
            lastID = allNodes[i].AssignIDToGraph(lastID);
        }

        for (var i = 0; i < allNodes.Count; i++)
        {
            allNodes[i].ResetRecursion();
        }

        if (alsoReorderList)
        {
            allNodes = allNodes.OrderBy(node => node.ID).ToList();
        }
    }
    /// <summary>
    /// 添加节点
    /// </summary>
    /// <param name="nodeType"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Node AddNode(System.Type nodeType, Vector2 pos)
    {
        if (!nodeType.IsSubclassOf(baseNodeType))
        {
            Debug.LogWarning("节点不是子类"+nodeType.Name +baseNodeType.Name);
            return null;
        }
        var newNode = Node.Create(this, nodeType, pos);
        allNodes.Add(newNode);
        if (primeNode == null)
        {
            primeNode = newNode;
        }
        this.UpdateNodeIDs(false);
        return newNode;
    }
    /// <summary>
    /// 连接节点
    /// </summary>
    /// <param name="sourceNode"></param>
    /// <param name="targetNode"></param>
    /// <param name="indexToInsert"></param>
    /// <returns></returns>
    public Connection ConnectNodes(Node sourceNode, Node targetNode, int indexToInsert)
    {
        if (targetNode.IsNewConnectionAllowed(sourceNode) == false)
            return null;
        var newConnection = Connection.Create(sourceNode, targetNode, indexToInsert);           
        UpdateNodeIDs(false);
        return newConnection;
    }
    /// <summary>
    /// 处理事件
    /// </summary>
    /// <param name="e"></param>
    /// <param name="canvasMousePos"></param>
    public void ShowGraphControls(Event e, Vector2 canvasMousePos)
    {
        HandleEvents(e, canvasMousePos);
    }
    public void OnBeforeSerialize()
    {

    }
    public void OnAfterDeserialize()
    {

    }
    #region 私有方法
    private void HandleEvents(Event e, Vector2 canvasMousePos)
    {
        var inspectorWithScrollbar = new Rect(inspectorRect.x, inspectorRect.y, inspectorRect.width + 14, inspectorRect.height);
        AllowClick = !inspectorWithScrollbar.Contains(e.mousePosition);
        if (!AllowClick)
        {
            return;
        }
        if (e.type == EventType.ContextClick)
        {
            var menu = GetAddNodeMenu(canvasMousePos);
            menu.ShowAsContext();
            e.Use();
        }
    }
    private GenericMenu GetAddNodeMenu(Vector2 canvasMousePos)
    {
        Action<Type> Selected = (type) => { currentSelection = AddNode(type, canvasMousePos); };
        var menu = EditorTool.GetTypeSelectionMenu(baseNodeType, Selected);
        return menu;
    }
    #endregion
}

