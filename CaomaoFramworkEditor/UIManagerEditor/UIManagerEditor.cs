using System;
using System.Collections.Generic;
using CaomaoFramework;
using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(UIManager), true)]
public class UIManagerEditor : PropertyDrawer
{
    private GUISkin skin;
    private bool open;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!skin)
        {
            skin = Resources.Load("skin") as GUISkin;
        }
        GUI.skin = skin;
        GUILayout.BeginVertical("游戏UI界面管理器", "window");
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("游戏UI界面管理器是管理不同场景游戏界面，比如登录界面，创建角色界面等", MessageType.Info);
        GUILayout.Space(10);

        open = GUILayout.Toggle(open, open ? "关闭UI界面管理器" : "打开UI界面管理器", EditorStyles.toolbarButton);
        if (open)
        {
            GUILayout.Space(10);
            var uiType = property.FindPropertyRelative("m_eUIType");
            EditorGUILayout.PropertyField(uiType, new GUIContent("UI界面插件类型"));
            GUILayout.Space(10);

            Dictionary<string, UIBase> uiDics = (property.serializedObject.targetObject as UnityMonoDriver).uiManager.m_dicUIs;
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }
}
