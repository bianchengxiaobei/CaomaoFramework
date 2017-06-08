using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CaomaoFramework;
[CustomPropertyDrawer(typeof(ClientGameStateManager))]
public class ClientGameStateManagerEditor : PropertyDrawer
{
    private GUISkin skin;
    GameStateGraph stateManager;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!skin)
        {
            skin = Resources.Load("skin") as GUISkin;
        }
        if (!stateManager)
        {          
            stateManager = (GameStateGraph)EditorTool.GetAssetOfType(typeof(GameStateGraph), ".asset");
            if (!stateManager)
            {
                if (EditorUtility.DisplayDialog("配置游戏状态管理器", "第一次需要配置游戏状态管理器！", "确定", "不要取消"))
                {
                    stateManager = (GameStateGraph)this.NewAsAsset();
                }
                else
                {
                    stateManager = (GameStateGraph)this.NewAsAsset();
                }

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
        if (GraphWindow.IsClosed() == true)
        {
            if (GUILayout.Button("打开游戏状态管理器"))
            {
                GraphWindow.OpenWindow(stateManager);
            }
        }
        else
        {
            if (GUILayout.Button("关闭游戏状态管理器"))
            {
                GraphWindow.CloseWindow();
            }
        }
        (property.serializedObject.targetObject as UnityMonoDriver).clientGameStateManager.m_oClientStateMachine.m_dicClientStates = new Dictionary<string, ClientStateBase>(GameStateGraph.stateDics);
        SerializedProperty defalutStateName = property.FindPropertyRelative("m_sDefalutGameStateName");
        EditorGUILayout.PropertyField(defalutStateName, new GUIContent("默认游戏状态名称"));
        EditorUtility.SetDirty(property.serializedObject.targetObject);
        GUI.enabled = true;
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
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

