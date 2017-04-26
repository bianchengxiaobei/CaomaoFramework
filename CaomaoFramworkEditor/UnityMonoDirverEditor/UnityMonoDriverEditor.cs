using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using CaomaoFramework;
using CaomaoFramework.GameState;
[CustomEditor(typeof(UnityMonoDriver),true)]
[CanEditMultipleObjects]
public class UnityMonoDriverEditor : Editor
{
    GUISkin skin;
    UnityMonoDriver driver;
    public override void OnInspectorGUI()
    {
        if (!skin)
        {
            skin = Resources.Load("skin") as GUISkin;
        }
        GUI.skin = skin;

        driver = (UnityMonoDriver)target;
        EditorGUILayout.Space();
        //标题
        GUILayout.BeginVertical("游戏Mono驱动器", "window");
        EditorGUILayout.Space();
        EditorGUILayout.Space();        
        //提示
        EditorGUILayout.HelpBox("游戏Mono驱动器管理着所有需要用到的管理器，比如资源加载管理器，UI界面管理器等", 
            MessageType.Info, true);
        //变量      
        base.OnInspectorGUI();
        //driver.targetFrameRate = EditorGUILayout.IntField("游戏帧率", driver.targetFrameRate);

        GUILayout.EndVertical();
    }
}
