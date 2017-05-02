using System;
using System.Reflection;
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
   /* public static T RTGetAttribute<T>(this Type type, bool inherited) where T : Attribute
    {
        return (T)type.GetCustomAttributes(typeof(T), inherited).FirstOrDefault();
    }
    */
    public static ScriptableObject CreateAsset(System.Type type, bool displayFilePanel)
    {
        ScriptableObject asset = null;
        var path = EditorUtility.SaveFilePanelInProject(
                    "创建Asset资源类型： " + type.ToString(),
                       type.Name + ".asset",
                    "asset", "最好新建一个文件夹ScriptObjects来保存");
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
        if (baseType.Equals(typeof(StateNode)))
        {
            menu.AddItem(new GUIContent("新增游戏状态"), false, Selected, typeof(StateNode));
            menu.AddItem(new GUIContent("新增游戏状态管理节点"), false, Selected, typeof(StateMrgNode));
        }
        else if (baseType.Equals(typeof(UINode)))
        {
            menu.AddItem(new GUIContent("新增游戏UI节点"), false, Selected, typeof(UINode));
            menu.AddItem(new GUIContent("新增游戏UI管理节点"), false, Selected, typeof(UIMrgNode));
        }
        return menu;
    }
    static public string GetFuncName(object obj, string method)
    {
        if (obj == null) return "<null>";
        string type = obj.GetType().ToString();
        type = type.Remove(0, type.LastIndexOf(".")+1);
        int period = type.LastIndexOf('/');
        if (period > 0) type = type.Substring(period + 1);
        return string.IsNullOrEmpty(method) ? type : type + "/" + method;
    }
    public static List<string> GetFuncParamType(string func)
    {
        func = func.Remove(0, 1);
        func = func.Remove(func.Length - 1, 1);
        if (string.IsNullOrEmpty(func))
        {
            return null;
        }
        string[] paramTypes = func.Split(';');
        List<string> types = new List<string>();
        for (int i = 0; i < paramTypes.Length; i++)
        {
            //如果是泛型的话
            if (paramTypes[i].Equals("T") | paramTypes[i].Equals("U") | paramTypes[i].Equals("V") | paramTypes[i].Equals("X"))
            {
                types.Add(typeof(GenericType).Name);
            }
            else
            {
                types.Add(paramTypes[i]);
            }
        }
        return types;
    }
    public static bool OpenScriptOfType(Type type,string name)
    {
        foreach (var path in AssetDatabase.GetAllAssetPaths())
        {
            if (path.EndsWith(name + ".cs"))
            {
                MonoScript script = (MonoScript)AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
                if (script == null)
                {
                    Debug.Log("没有找到该脚本");
                    return false;
                }
                if (script.GetClass().IsSubclassOf(type))
                {
                    AssetDatabase.OpenAsset(script);
                    return true;
                }
            }
        }
        Debug.Log("找不到该脚本");
        return false;
    }
    public static Type GetScriptType(string scriptName)
    {
        foreach (var path in AssetDatabase.GetAllAssetPaths())
        {
            if (path.EndsWith(scriptName + ".cs"))
            {
                MonoScript script = (MonoScript)AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
                return script.GetClass();
            }
        }
        Debug.Log("找不到");
        return null;
    }
    public static UnityEngine.Object GetScriptOfType(Type type,string scriptName)
    {
        foreach (var path in AssetDatabase.GetAllAssetPaths())
        {
            if (path.EndsWith(scriptName + ".cs"))
            {
                MonoScript script = (MonoScript)AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
                if (script.GetClass().IsSubclassOf(type))
                {
                    return script;
                }
            }
        }
        return null;
    }
    public static UnityEngine.Object GetAssetOfType(Type type,string suffix)
    {
        foreach (var path in AssetDatabase.GetAllAssetPaths())
        {
            if (path.EndsWith(type.Name + suffix))
            {
                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(path, type);
                if (asset != null)
                {
                    return asset;
                }
                else
                {
                    Debug.LogError("找不到");
                    return null;
                }
            }
        }
        return null;
    }
    /// <summary>
    /// 删除脚本
    /// </summary>
    /// <param name="asset"></param>
    public static void DeleteScript(UnityEngine.Object asset)
    {
        if (asset == null)
        {
            return;
        }
        string path = AssetDatabase.GetAssetPath(asset);
        AssetDatabase.DeleteAsset(path);
    }
    public static void RecordUndo(UnityEngine.Object asset,string name)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.Undo.RecordObject(asset, name);
        }
#endif
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
public enum TipConnectionStyle
{
    None,
    Circle,
    Arrow
}
public enum ParamType
{
    Int,
    String
}