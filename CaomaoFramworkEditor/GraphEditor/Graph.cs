using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
[Serializable]
public abstract class Graph : ScriptableObject,ISerializationCallbackReceiver
{
    [SerializeField]
    private string _serializedGraph;
    [SerializeField]
    private List<Object> _objectReferences;
    [SerializeField]
    private bool _deserializationFailed = false;
    [System.NonSerialized]
    private bool hasEnabled = false;
    [System.NonSerialized]
    private bool hasDeserialized = false;
    #region GraphDataSerialized
    private string _name;
    private float _zoomFactor = 1f;
    private Vector2 _translation = new Vector2(-5000, -5000);
    private List<Node> _nodes = new List<Node>();
    private Node _primeNode = null;
    #endregion
    private Rect inspectorRect = new Rect(15, 55, 0, 0);
    private Vector2 nodeInspectorScrollPos;//节点Inspector窗口位置
    private static object m_currentSelection;
    private static List<object> m_multiSeletion = new List<object>();
    #region 属性

    public bool deserializationFailed
    {
        get { return _deserializationFailed; }
    }
    new public string name
    {
        get { return string.IsNullOrEmpty(_name) ? GetType().Name : _name; }
        set { _name = value; }
    }
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
    public static Action PostGUI { get; set; }
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
            m_currentSelection = value;
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
    /// <summary>
    /// 选择的node
    /// </summary>
    public static Node selectedNode
    {
        get { return currentSelection as Node; }
    }
    /// <summary>
    /// 选择的connect
    /// </summary>
    public static Connection selectedConnection
    {
        get { return currentSelection as Connection; }
    }
    #endregion
    #region 抽象属性
    public abstract Type baseNodeType { get;  }
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
        this.UpdateNodeIDs(false);
        for (var i = 0; i < allNodes.Count; i++)
        {
            if (fullDrawPass || drawCanvas.Overlaps(allNodes[i].nodeRect))
            {
                allNodes[i].ShowNodeGUI(canvasMousePos, zoomFactor);
            }
        }
        for (var i = 0; i < allNodes.Count; i++)
        {
            allNodes[i].DrawNodeConnections(canvasMousePos, zoomFactor);
        }
        if (primeNode != null)
        {
            GUI.Box(new Rect(primeNode.nodeRect.x, primeNode.nodeRect.y - 20, primeNode.nodeRect.width, 20), "<b>Manager</b>");
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
        if (!nodeType.IsSubclassOf(baseNodeType) && !nodeType.Equals(baseNodeType))
        {
            Debug.LogError("节点不是子类o"+nodeType.Name +baseNodeType.Name);
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
    /// 移除连接线
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="recordUndo"></param>
    public void RemoveConnection(Connection connection, bool recordUndo = true)
    {
        connection.OnDestroy();
        connection.sourceNode.outConnections.Remove(connection);
        connection.targetNode.inConnections.Remove(connection);
        currentSelection = null;
        UpdateNodeIDs(false);
    }
    /// <summary>
    /// 删除节点
    /// </summary>
    /// <param name="node"></param>
    /// <param name="recordUndo"></param>
    public void RemoveNode(Node node,bool recordUndo = true)
    {
        if (!allNodes.Contains(node))
        {
            return;
        }
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            //auto reconnect parent & child of deleted node. Just a workflow convenience
            if (node.inConnections.Count == 1 && node.outConnections.Count == 1)
            {
                var relinkNode = node.outConnections[0].targetNode;
                RemoveConnection(node.outConnections[0]);
                node.inConnections[0].SetTarget(relinkNode);
            }
        }
        //TODO: Fix this in the property accessors?
        currentSelection = null;

#endif

        //callback
        node.OnDestroy();

        //disconnect parents
        foreach (var inConnection in node.inConnections.ToArray())
        {
            RemoveConnection(inConnection);
        }

        //disconnect children
        foreach (var outConnection in node.outConnections.ToArray())
        {
            RemoveConnection(outConnection);
        }

        if (recordUndo)
        {
            if (!Application.isPlaying)
            {
                Undo.RecordObject(this, "Delete Node");
            }
        }

        allNodes.Remove(node);

        if (node == primeNode)
        {
            primeNode = GetNodeWithID(2);
        }

        UpdateNodeIDs(false);
    }
    /// <summary>
    /// 处理事件
    /// </summary>
    /// <param name="e"></param>
    /// <param name="canvasMousePos"></param>
    public void ShowGraphControls(Event e, Vector2 canvasMousePos)
    {
        this.ShowInspectorGUIPanel(e, canvasMousePos);
        HandleEvents(e, canvasMousePos);
        if (PostGUI != null)
        {
            PostGUI();
            PostGUI = null;
        }
    }
    /// <summary>
    /// 取得节点根据ID
    /// </summary>
    /// <param name="searchID"></param>
    /// <returns></returns>
    public Node GetNodeWithID(int searchID)
    {
        if (searchID <= allNodes.Count && searchID >= 0)
            return allNodes.Find(n => n.ID == searchID);
        return null;
    }
    public void OnBeforeSerialize()
    {
        if (_objectReferences != null && _objectReferences.Any(o => o != null))
        { //As it seems Unity requires double deserialize for objects
            hasDeserialized = false;
        }
#if UNITY_EDITOR
			if (JSONSerializer.applicationPlaying) return;
			_serializedGraph = this.Serialize(false, _objectReferences);
#endif
    }
    public void OnAfterDeserialize()
    {
        if (hasDeserialized && JSONSerializer.applicationPlaying) return; //avoid double call that Unity does (bug?)
        hasDeserialized = true;
        this.Deserialize(_serializedGraph, false, _objectReferences);
    }
    ///Serialize (save) the graph and returns the serialized json string
    public string Serialize(bool pretyJson, List<UnityEngine.Object> objectReferences)
    {

        //if something went wrong on deserialization, dont serialize back, but rather keep what we had
        if (_deserializationFailed)
        {
            _deserializationFailed = false;
            return _serializedGraph;
        }
        //the list to save the references in. If not provided externaly we save into the local list
        if (objectReferences == null)
        {
            objectReferences = this._objectReferences = new List<Object>();
        }
        else
        {
            objectReferences.Clear();
        }

        UpdateNodeIDs(true);
        return JSONSerializer.Serialize(typeof(GraphSerializationData), new GraphSerializationData(this), pretyJson, objectReferences);
    }
    ///Deserialize (load) the json serialized graph provided. Returns false if Deserialization failed
    public GraphSerializationData Deserialize(string serializedGraph, bool validate, List<UnityEngine.Object> objectReferences)
    {

        if (string.IsNullOrEmpty(serializedGraph))
        {
            return null;
        }

        //the list to load the references from. If not provided externaly we load from the local list (which is the case most times)
        if (objectReferences == null)
        {
            objectReferences = this._objectReferences;
        }

        try
        {
            //deserialize provided serialized graph into a new GraphSerializationData object and load it
            var data = JSONSerializer.Deserialize<GraphSerializationData>(serializedGraph, objectReferences);
            if (LoadGraphData(data, validate) == true)
            {
                this._deserializationFailed = false;
                this._serializedGraph = serializedGraph;
                return data;
            }

            _deserializationFailed = true;
            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError(string.Format("Deserialization Error: '{0}'\n'{1}'\n\n<b>Please report bug</b>", e.Message, e.StackTrace), this);
            _deserializationFailed = true;
            return null;
        }
    }

    //TODO: Move this in GraphSerializationData object Reconstruction?
    bool LoadGraphData(GraphSerializationData data, bool validate)
    {
        if (data == null)
        {
            Debug.LogError("Can't Load graph, cause of null GraphSerializationData provided");
            return false;
        }

        if (data.type != this.GetType())
        {
            Debug.LogError("Can't Load graph, cause of different Graph type serialized and required");
            return false;
        }

        data.Reconstruct(this);

        //grab the final data and set fields directly
        this._name = data.name;
        this._translation = data.translation;
        this._zoomFactor = data.zoomFactor;
        this._nodes = data.nodes;
        this._primeNode = data.primeNode;

        //IMPORTANT: Validate should be called in all deserialize cases outside of Unity's 'OnAfterDeserialize',
        //like for example when loading from json, or manualy calling this outside of OnAfterDeserialize.
        return true;
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
    /// <summary>
    /// 显示创建脚本界面
    /// </summary>
    /// <param name="e"></param>
    /// <param name="canvasMousePos"></param>
    private void ShowInspectorGUIPanel(Event e, Vector2 canvasMousePos)
    {
        if (selectedNode == null)
        {
            inspectorRect.height = 0;
            return;
        }
        inspectorRect.width = 330;
        inspectorRect.x = 10;
        inspectorRect.y = 30;
        EditorGUIUtility.AddCursorRect(new Rect(inspectorRect.x, inspectorRect.y, 330, 30), MouseCursor.Link);
        
        GUI.Box(inspectorRect, "", "windowShadow");
        string title = selectedNode != null ? selectedNode.Name : "Node";
        var lastSkin = GUI.skin;
        var viewRect = new Rect(inspectorRect.x, inspectorRect.y, inspectorRect.width + 18, Screen.height - inspectorRect.y - 30);
        nodeInspectorScrollPos = GUI.BeginScrollView(viewRect, nodeInspectorScrollPos, inspectorRect);
        GUILayout.BeginArea(inspectorRect, title, (GUIStyle)"editorPanel");
        GUILayout.Space(5);
        GUI.skin = null;

        if (selectedNode != null)
        {
            selectedNode.ShowNodeInspectorGUI();
        }
        else if (selectedConnection != null)
        {
            //selectedConnection.ShowConnectionInspectorGUI();
        }

        GUILayout.Box("", GUILayout.Height(5), GUILayout.Width(inspectorRect.width - 10));
        GUI.skin = lastSkin;
        if (e.type == EventType.Repaint)
        {
            inspectorRect.height = GUILayoutUtility.GetLastRect().yMax + 5;
        }

        GUILayout.EndArea();
        GUI.EndScrollView();

        if (GUI.changed)
        {
            Debug.Log("3333");
            EditorUtility.SetDirty(this);
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

