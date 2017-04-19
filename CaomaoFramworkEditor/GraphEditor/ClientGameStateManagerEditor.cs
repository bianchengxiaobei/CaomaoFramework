using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CaomaoFramework.GameState;
[CustomPropertyDrawer(typeof(ClientGameStateManager))]
public class ClientGameStateManagerEditor : PropertyDrawer
{
    private GUISkin skin;
    private bool isOpen = false;
    GameStateGraph stateManager;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!skin)
        {
            skin = Resources.Load("skin") as GUISkin;
        }
        if (!stateManager)
        {
            stateManager = AssetDatabase.LoadAssetAtPath<GameStateGraph>("Assets/ScriptObject/ClientState/GameStateGraph.asset");
            if (!stateManager)
            {
                stateManager = (GameStateGraph)this.NewAsAsset();
            }           
        }
        GUI.skin = skin;

        GUILayout.BeginVertical("游戏状态管理器", "window");
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("游戏状态管理器是管理每个游戏状态的进入和离开，比如登录状态，创建角色状态等等", MessageType.Info);
        GUILayout.Space(10);
        if (isOpen == false)
        {
            if (GUILayout.Button("打开游戏状态管理器"))
            {
                isOpen = true;
                GraphWindow.OpenWindow(stateManager);
            }
        }
        else
        {
            if (GUILayout.Button("关闭游戏状态管理器"))
            {
                isOpen = false;
                GraphWindow.CloseWindow();
            }
        }
        GUI.enabled = true;
    }
    private Graph NewAsAsset()
    {
        var newGraph = (Graph)EditorTool.CreateAsset(typeof(GameStateGraph), true);
        if (newGraph != null)
        {
            EditorUtility.SetDirty(newGraph);
            AssetDatabase.SaveAssets();
        }
        return newGraph;
    }
}

