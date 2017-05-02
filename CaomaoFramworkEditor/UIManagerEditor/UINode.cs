using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CaomaoFramework;
[Name("UI界面")]
[Icon("ClientState/StateIcon")]
[Description("游戏UI界面节点，显示UI需要用到的预制物")]
public class UINode : Node
{
    [SerializeField]
    protected UnityEngine.Object uiPrefab;
    [SerializeField]
    private List<UIFieldEditor> uiVariables = new List<UIFieldEditor>();
    [SerializeField]
    private bool bIsResident;
    public override int maxInConnections
    {
        get
        {
            return 10;
        }
    }
    public override int maxOutConnections
    {
        get
        {
            return 10;
        }
    }
    public override Type outConnectionType
    {
        get
        {
            return typeof(UIConnection);
        }
    }
    protected override void PostConnectionActiveEvent()
    {
        Type type = EditorTool.GetScriptType(this.scriptName);
        UIGraph.uiDics.Add(this.scriptName, Activator.CreateInstance(type) as UIBase);
        EditorUtility.SetDirty(this.graph);       
    }
    public override void OnDisConnection()
    {
        if (UIGraph.uiDics.Remove(this.scriptName))
            EditorUtility.SetDirty(this.graph);
    }
    protected override void OnNodeInspectorGUI()
    {
        base.OnNodeInspectorGUI();
        if (this.ScriptObject != null)
        {

        }
        else
        {
            if (this.graph.baseNodeType.Equals(typeof(UINode)) && this.GetType().Equals(typeof(UINode)))
            {
                EditorGUILayout.BeginVertical();
                this.scriptName = EditorGUILayout.TextField("脚本名字", this.scriptName);
                this.uiPrefab = EditorGUILayout.ObjectField("UI预制物", uiPrefab, typeof(UnityEngine.Object), true);
                this.bIsResident = EditorGUILayout.Toggle("是否常驻内存", this.bIsResident);
                EditorGUILayout.Separator();
                for (int i = 0; i < this.uiVariables.Count; i++)
                {
                    if (this.uiVariables[i].Deleted)
                    {
                        this.uiVariables.RemoveAt(i);
                        i--;
                        continue;
                    }
                    this.uiVariables[i].Show();
                }
                if (GUILayout.Button("添加一个组件变量"))
                {
                    this.uiVariables.Add(new UIFieldEditor(this));
                }
                EditorGUILayout.Space();
                EditorGUILayout.Separator();
                if (GUILayout.Button("创建脚本"))
                {
                    if (string.IsNullOrEmpty(this.scriptName))
                    {
                        return;
                    }
                    string path = EditorUtility.OpenFolderPanel("选择保存脚本路径", "", "");
                    TextAsset asset = Resources.Load<TextAsset>("ScriptsTemplate/FairyGUITemplate");
                    if (asset != null)
                    {
                        TemplateSystem template = new TemplateSystem(asset.text);
                        template.AddVariable("className", this.scriptName);
                        template.AddVariable("resident", this.bIsResident);
                        path = path + "/" + this.scriptName + ".cs";
                        string uiPath = AssetDatabase.GetAssetPath(this.uiPrefab);
                        if (uiPath.Contains("Resources"))
                        {
                            int index = uiPath.IndexOf("Resources") + 10;
                            string extension = uiPath.Remove(0, index);
                            string content = extension.Remove(extension.LastIndexOf("."));
                            template.AddVariable("resName", content);
                        }
                        List<object[]> variables = new List<object[]>();
                        for (int i = 0; i < this.uiVariables.Count; i++)
                        {
                            object[] fieldData = new object[]
                            {
                            (UIFairyPramaType)this.uiVariables[i].FieldType,
                            this.uiVariables[i].FieldName,
                            this.uiVariables[i].FieldPath
                            };
                            variables.Add(fieldData);
                        }
                        template.AddVariable("variables", variables.ToArray());
                        using (StreamWriter sw = File.CreateText(path))
                        {
                            sw.Write(template.Parse());
                        }

                    }
                    AssetDatabase.Refresh();
                }
                EditorGUILayout.EndVertical();
            }
        }
    }
}
public enum UIFairyPramaType
{
    GButton = 0,
    GList
}
public enum UIUGUIParamType
{
    Button = 10,
    Text,
    Image
}
public class UIFieldEditor
{
    public string FieldName;
    public string FieldPath;
    public int FieldType;
    public bool Deleted;
    public Node OwnerNode;
    public UIFieldEditor(Node node,string name="")
    {
        this.OwnerNode = node;
        this.FieldName = name;
    }
    public void Show()
    {
        if (this.Deleted)
        {
            return;
        }
        this.FieldName = EditorGUILayout.TextField("组件名称", this.FieldName);
        this.FieldPath = EditorGUILayout.TextField("组件所在UI路径", this.FieldPath);
        EUIManagerType type = EUIManagerType.e_NGUI;
        if (this.OwnerNode.graph is UIGraph)
        {
            UIGraph graph = this.OwnerNode.graph as UIGraph;
            type = graph.uiPluginType;
        }
        switch (type)
        {
            case EUIManagerType.e_FairyGUI:
                UIFairyPramaType fairygui = (UIFairyPramaType)EditorGUILayout.EnumPopup("组件类型", (UIFairyPramaType)this.FieldType);
                this.FieldType = (int)fairygui;
                break;
            case EUIManagerType.e_UGUI:

                break;
            case EUIManagerType.e_NGUI:

                break;
        }
        
        GUI.color = Color.red;
        if (GUILayout.Button("删除", EditorStyles.miniButton))
        {
            this.Deleted = true;
        }
        GUI.color = Color.white;
    }
}
