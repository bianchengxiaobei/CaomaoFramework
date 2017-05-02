using System;
using System.Collections.Generic;
using CaomaoFramework;
using UnityEngine;
using UnityEditor;
using System.Linq;
[CustomPropertyDrawer(typeof(UIManager), true)]
public class UIManagerEditor : PropertyDrawer
{
    private GUISkin skin;
    private UIGraph uiManager;
    private bool bIsOpen = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!skin)
        {
            skin = Resources.Load("skin") as GUISkin;
        }
        if (!uiManager)
        {
            uiManager = (UIGraph)EditorTool.GetAssetOfType(typeof(UIGraph), ".asset");
            if (!uiManager)
            {
                if (EditorUtility.DisplayDialog("配置游戏UI界面管理器", "第一次需要配置游戏UI界面管理器！", "确定", "不要取消"))
                {
                    uiManager = (UIGraph)this.NewAsAsset();
                }
                else
                {
                    uiManager = (UIGraph)this.NewAsAsset();
                }
            }
        }
        GUI.skin = skin;
        GUILayout.BeginVertical("游戏UI界面管理器", "window");
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("游戏UI界面管理器是管理不同场景游戏界面，比如登录界面，创建角色界面等", MessageType.Info);
        GUILayout.Space(10);
        if (GraphWindow.IsClosed() == true)
        {
            if (GUILayout.Button("打开游戏UI界面管理器"))
            {
                bIsOpen = true;
                GraphWindow.OpenWindow(uiManager);                
            }
        }
        else
        {
            if (GUILayout.Button("关闭游戏UI界面管理器"))
            {
                bIsOpen = false;
                GraphWindow.CloseWindow();
            }
        }
        if (bIsOpen)
        {
            GUILayout.Space(10);
            var uiType = property.FindPropertyRelative("m_eUIType");
            EditorGUILayout.PropertyField(uiType, new GUIContent("UI界面插件类型"));
            uiManager.uiPluginType = (EUIManagerType)uiType.enumValueIndex;
            GUILayout.Space(10);           
        }
        (property.serializedObject.targetObject as UnityMonoDriver).uiManager.m_dicUIs = new Dictionary<string, UIBase>(UIGraph.uiDics);
        //Debug.Log("fwefewf:"+ (property.serializedObject.targetObject as UnityMonoDriver).uiManager.m_dicUIs.Count);
        EditorUtility.SetDirty(property.serializedObject.targetObject);
        GUI.enabled = true;
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
        property.serializedObject.ApplyModifiedProperties();
    }
    private Graph NewAsAsset()
    {
        var newGraph = (Graph)EditorTool.CreateAsset(typeof(UIGraph), true);
        if (newGraph != null)
        {
            EditorUtility.SetDirty(newGraph);
            AssetDatabase.SaveAssets();
        }
        return newGraph;
    }
}
