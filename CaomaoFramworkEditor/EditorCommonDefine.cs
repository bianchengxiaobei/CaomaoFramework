using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
public class EditorCommonDefine
{
    public readonly static float unityTabHeight = 22;
    public readonly static int gridSize = 15;
    public readonly static float topMargin = 20;
    public readonly static Vector2 zoomPoint = new Vector2(0, 20);
    public readonly static Vector2 minSize = new Vector2(100, 20);
    public readonly static Vector2 virtualCenterOffset = new Vector2(-5000, -5000);
    public readonly static Color restingColor = new Color(0.7f, 0.7f, 1f, 0.8f);
}
public static class EditorTool
{
    public static T RTGetAttribute<T>(this Type type, bool inherited) where T : Attribute
    {
        return (T)type.GetCustomAttributes(typeof(T), inherited).FirstOrDefault();
    }
    public static ScriptableObject CreateAsset(System.Type type, bool displayFilePanel)
    {
        ScriptableObject asset = null;
        var path = EditorUtility.SaveFilePanelInProject(
                    "Create Asset of type " + type.ToString(),
                       type.Name + ".asset",
                    "asset", "");
        asset = CreateAsset(type, path);
        return asset;
    }
    public static ScriptableObject CreateAsset(System.Type type, string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;
        ScriptableObject data = null;
        data = ScriptableObject.CreateInstance(type);
        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
        return data;
    }
    public static GenericMenu GetTypeSelectionMenu(Type baseType, Action<Type> callback, GenericMenu menu = null, string subCategory = null)
    {
        if (menu == null)
        {
            menu = new GenericMenu();
        }
        GenericMenu.MenuFunction2 Selected = delegate (object selectedType) {
            callback((Type)selectedType);
        };
        menu.AddItem(new GUIContent("新增游戏状态"), false, Selected, typeof(StateNode));
        menu.AddItem(new GUIContent("新增游戏状态管理节点"), false, Selected, typeof(StateMrgNode));
        return menu;
    }
}
public enum ConnectionStatus
{
    Failure = 0,
    Success = 1,
    Running = 2,
    Resting = 3,
    Error = 4
}
