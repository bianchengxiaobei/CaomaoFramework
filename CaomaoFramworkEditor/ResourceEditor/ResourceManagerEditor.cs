using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CaomaoFramework;
[CustomPropertyDrawer(typeof(ResourceManager))]
public class ResourceManagerEditor : PropertyDrawer
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
        GUILayout.BeginVertical("游戏资源管理器", "window");
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("游戏资源管理器是管理游戏加载资源、卸载资源，比如音频，UI界面，场景，模型等", MessageType.Info);
        GUILayout.Space(10);

        open = GUILayout.Toggle(open, open ? "关闭资源管理器" : "打开资源管理器", EditorStyles.toolbarButton);
        if (open)
        {
            GUILayout.Space(10);
            var path = property.FindPropertyRelative("strAssetPath");
            if (path != null)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(new GUIContent("资源加载路径:"), new GUIContent(path.stringValue));
                if (GUILayout.Button("选择资源路径"))
                {
                    path.stringValue = EditorUtility.OpenFolderPanel("选择资源加载路径","Resources", "Resources");
                }
                EditorGUILayout.EndVertical();
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }
}
