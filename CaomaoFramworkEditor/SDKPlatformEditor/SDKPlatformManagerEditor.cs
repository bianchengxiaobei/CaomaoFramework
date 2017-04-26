using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CaomaoFramework;
[CustomPropertyDrawer(typeof(SDKPlatformManager),true)]
public class SDKPlatformManagerEditor : PropertyDrawer
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
        GUILayout.BeginVertical("游戏SDK管理器", "window");
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("游戏SDK管理器是管理游戏更新、账号登录、开场动画等", MessageType.Info);
        GUILayout.Space(10);

        open = GUILayout.Toggle(open, open ? "关闭SDK管理器" : "打开SDK管理器", EditorStyles.toolbarButton);
        if (open)
        {
            GUILayout.Space(10);
            var platformType = property.FindPropertyRelative("m_ePlatformType");
            if (platformType != null)
            {
                //EditorGUILayout.EnumPopup("平台类型", (EPlatformType)(platformType.enumValueIndex));
                EditorGUILayout.PropertyField(platformType, new GUIContent("平台类型"));
            }
            GUILayout.Space(10);
            UnityMonoDriver obj = property.serializedObject.targetObject as UnityMonoDriver;
            CaomaoEditorTools.DrawEvents("安装成功事件", obj.sdkManager.m_installSuccess, obj);
            property.serializedObject.ApplyModifiedProperties();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }
}
