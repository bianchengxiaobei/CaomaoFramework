using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GraphWindow : EditorWindow
{
    public static Graph currentGraph;
    private Graph root;

    private Event evt;
    private GUISkin skin;
    private Rect viewRect;
    private Rect canvasRect;
    private Rect totalCanvas;
    private Matrix4x4 oldMatrix;
    private Vector2? smoothPan = null;
    private float? smoothZoomFactor = null;
    private Rect nodeBounds;//节点大小
    private bool mouseButton2Down = false;//是否鼠标中键按下
    private bool fullDrawPass = true;
    private bool willRepaint;
    private Rect zoomRecoveryRect;
    #region 属性
    private float zoomFactor
    {
        get { return currentGraph != null ? Mathf.Clamp(currentGraph.zoomFactor, 0.25f, 1f) : 1f; }
        set { if (currentGraph != null) currentGraph.zoomFactor = Mathf.Clamp(value, 0.25f, 1f); }
    }
    private Vector2 virtualCenter
    {
        get { return -EditorCommonDefine.virtualCenterOffset + viewRect.size / 2; }
    }
    private Vector2 pan
    {
        get { return currentGraph != null ? Vector2.Min(currentGraph.translation, Vector2.zero) : virtualCenter; }
        set
        {
            if (currentGraph != null)
            {
                var t = currentGraph.translation;
                t = Vector2.Min(value, Vector2.zero);
                if (smoothPan == null)
                {
                    t.x = Mathf.Round(t.x);
                    t.y = Mathf.Round(t.y);
                }
                currentGraph.translation = t;
            }
        }
    }
    /// <summary>
    /// 鼠标在画布的坐标
    /// </summary>
    private Vector2 mousePosInCanvas
    {
        get { return ViewToCanvas(Event.current.mousePosition); }
    }
    #endregion
    private void OnGUI()
    {
        if (EditorApplication.isCompiling)
        {
            ShowNotification(new GUIContent("程序正在编译，请等待......"));
            return;
        }
        GUI.color = Color.white;
        GUI.backgroundColor = Color.white;
        evt = Event.current;
        GUI.skin = skin;

        currentGraph = root;
        this.HandleEvents(evt);
        canvasRect = new Rect(5, EditorCommonDefine.topMargin, position.width - 10, position.height - EditorCommonDefine.topMargin - 5);
        GUI.Box(canvasRect, string.Format("{0}\n{1}", "游戏状态管理器", "@Caomao Framework v0.0.1"), (GUIStyle)"canvasBG");
        if (zoomFactor != 1)
        {
            canvasRect = StartZoomArea(canvasRect);
        }
        GUI.BeginGroup(canvasRect);
            totalCanvas = canvasRect;
            totalCanvas.x = 0;
            totalCanvas.y = 0;
            totalCanvas.x += pan.x / zoomFactor;
            totalCanvas.y += pan.y / zoomFactor;
            totalCanvas.width -= pan.x / zoomFactor;
            totalCanvas.height -= pan.y / zoomFactor;


            GUI.BeginGroup(totalCanvas);               
                viewRect = totalCanvas;
                viewRect.x = 0;
                viewRect.y = 0;
                viewRect.x -= pan.x / zoomFactor;
                viewRect.y -= pan.y / zoomFactor;
                viewRect.width += pan.x / zoomFactor;
                viewRect.height += pan.y / zoomFactor;
                nodeBounds = GetNodeBounds(currentGraph, viewRect, true);
                DrawGrid(viewRect, pan, zoomFactor);
                BeginWindows();
                currentGraph.ShowNodesGUI(evt, viewRect, fullDrawPass, mousePosInCanvas, zoomFactor);
                EndWindows();
            GUI.EndGroup();
        GUI.EndGroup();
        if (zoomFactor != 1)
        {
            this.EndZoomArea();
        }

        currentGraph.ShowGraphControls(evt, mousePosInCanvas);

        if (willRepaint)
        {
            Repaint();
        }
        if (GUI.changed)
        {
            foreach (var node in currentGraph.allNodes)
            {
                node.nodeRect = new Rect(node.nodePosition.x, node.nodePosition.y, EditorCommonDefine.minSize.x, EditorCommonDefine.minSize.y);
            }
            Repaint();
        }
        if (evt.type == EventType.Repaint)
        {
            fullDrawPass = false;
            willRepaint = false;
        }
        //关闭
        GUI.Box(canvasRect, "", "canvasBorders");
        GUI.skin = null;
        GUI.color = Color.white;
        GUI.backgroundColor = Color.white;
    }
    private void OnEnable()
    {
#if UNITY_5
        var canvasIcon = (Texture)Resources.Load("CanvasIcon");
        titleContent = new GUIContent("游戏状态编辑器", canvasIcon);
#else
        title = "游戏状态编辑器";
#endif
        willRepaint = true;
        fullDrawPass = true;
        if (!skin)
        {
            skin = EditorGUIUtility.isProSkin ? Resources.Load("NodeCanvasSkin") as GUISkin : Resources.Load("NodeCanvasSkinLight") as GUISkin;
        }
        minSize = new Vector2(700, 300);
        Selection.selectionChanged += OnSelectionChange;
        Repaint();
    }
    private void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChange;
    }
    #region 事件回调
    private void OnSelectionChange()
    {
        
    }
    #endregion
    #region 打开窗体
    public static GraphWindow OpenWindow(Graph graph)
    {
        var window = GetWindow<GraphWindow>();
        window.root = graph;
        return window;
    }
    public static void CloseWindow()
    {
        var window = GetWindow<GraphWindow>();
        window.Close();
    }
    #endregion
    #region 私有方法
    private Rect StartZoomArea(Rect container)
    {
        GUI.EndGroup();
        container.height += EditorCommonDefine.unityTabHeight;
        container.width *= 1 / zoomFactor;
        container.height *= 1 / zoomFactor;
        oldMatrix = GUI.matrix;
        var matrix1 = Matrix4x4.TRS(EditorCommonDefine.zoomPoint, Quaternion.identity, Vector3.one);
        var matrix2 = Matrix4x4.Scale(new Vector3(zoomFactor, zoomFactor, 1f));
        GUI.matrix = matrix1 * matrix2 * matrix1.inverse * GUI.matrix;
        return container;
    }
    /// <summary>
    /// 画画布
    /// </summary>
    /// <param name="container"></param>
    /// <param name="offset"></param>
    /// <param name="zoomFactor"></param>
    private void DrawGrid(Rect container, Vector2 offset, float zoomFactor)
    {

        var scaledX = (container.width - offset.x) / zoomFactor;
        var scaledY = (container.height - offset.y) / zoomFactor;

        for (var i = 0 - (int)offset.x; i < scaledX; i++)
        {
            if (i % EditorCommonDefine.gridSize == 0)
            {
                Handles.color = new Color(0, 0, 0, i % (EditorCommonDefine.gridSize * 5) == 0 ? 0.2f : 0.1f);
                Handles.DrawLine(new Vector3(i, 0, 0), new Vector3(i, scaledY, 0));
            }
        }

        for (var i = 0 - (int)offset.y; i < scaledY; i++)
        {
            if (i % EditorCommonDefine.gridSize == 0)
            {
                Handles.color = new Color(0, 0, 0, i % (EditorCommonDefine.gridSize * 5) == 0 ? 0.2f : 0.1f);
                Handles.DrawLine(new Vector3(0, i, 0), new Vector3(scaledX, i, 0));
            }
        }
        Handles.color = Color.white;
    }
    /// <summary>
    /// 取得节点位置大小
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="container"></param>
    /// <param name="bound"></param>
    /// <returns></returns>
    private Rect GetNodeBounds(Graph graph, Rect container, bool bound = false)
    {
        var minX = float.PositiveInfinity;
        var minY = float.PositiveInfinity;
        var maxX = float.NegativeInfinity;
        var maxY = float.NegativeInfinity;

        for (var i = 0; i < graph.allNodes.Count; i++)
        {
            minX = Mathf.Min(minX, graph.allNodes[i].nodeRect.xMin);
            minY = Mathf.Min(minY, graph.allNodes[i].nodeRect.yMin);
            maxX = Mathf.Max(maxX, graph.allNodes[i].nodeRect.xMax);
            maxY = Mathf.Max(maxY, graph.allNodes[i].nodeRect.yMax);
        }

        minX -= 50;
        minY -= 50;
        maxX += 50;
        maxY += 50;

        if (bound)
        {
            minX = Mathf.Min(minX, container.xMin + 50);
            minY = Mathf.Min(minY, container.yMin + 50);
            maxX = Mathf.Max(maxX, container.xMax - 50);
            maxY = Mathf.Max(maxY, container.yMax - 50);
        }

        return Rect.MinMaxRect(minX, minY, maxX, maxY);
    }
    /// <summary>
    /// 视口坐标转为画布坐标
    /// </summary>
    /// <param name="viewPos"></param>
    /// <returns></returns>
    private Vector2 ViewToCanvas(Vector2 viewPos)
    {
        return (viewPos - pan) / zoomFactor;
    }
    /// <summary>
    /// 结束缩放
    /// </summary>
    private void EndZoomArea()
    {
        GUI.matrix = oldMatrix;
        zoomRecoveryRect.y = EditorCommonDefine.unityTabHeight;
        zoomRecoveryRect.width = Screen.width;
        zoomRecoveryRect.height = Screen.height;
        GUI.BeginGroup(zoomRecoveryRect); 
    }
    /// <summary>
    /// 处理事件
    /// </summary>
    /// <param name="e"></param>
    private void HandleEvents(Event e)
    {
        //如果鼠标在窗口上，或者鼠标点击窗口，还有键盘按下都重新绘制窗口
        if (mouseOverWindow == this && (e.isMouse || e.isKey))
        {
            willRepaint = true;
        }
        if (e.type == EventType.MouseUp || e.type == EventType.KeyUp)
        {
            this.SnapNodes();
        }
        //鼠标中键按下移动画布
        if ((e.button == 2 && e.type == EventType.MouseDrag && canvasRect.Contains(e.mousePosition))
                || ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.alt && e.isMouse))
        {
            pan += e.delta;
            smoothPan = null;
            smoothZoomFactor = null;
            e.Use();
        }
        if (e.type == EventType.MouseDown && e.button == 2 && canvasRect.Contains(e.mousePosition))
        {
            mouseButton2Down = true;
        }
        if (e.type == EventType.MouseUp && e.button == 2)
        {
            mouseButton2Down = false;
        }
        //鼠标中键按下设置鼠标icon
        if (mouseButton2Down == true)
        {
            EditorGUIUtility.AddCursorRect(new Rect(0, 0, Screen.width, Screen.height), MouseCursor.Pan);
        }
    }
    private void SnapNodes()
    {
        foreach (var node in currentGraph.allNodes)
        {
            var snapedPos = new Vector2(node.nodeRect.xMin, node.nodeRect.yMin);
            snapedPos.y = Mathf.Round(snapedPos.y / 15) * 15;
            snapedPos.x = Mathf.Round(snapedPos.x / 15) * 15;
            node.nodeRect = new Rect(snapedPos.x, snapedPos.y, node.nodeRect.width, node.nodeRect.height);
        }
    }
    #endregion
}

