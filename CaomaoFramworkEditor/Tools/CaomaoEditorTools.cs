using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CaomaoFramework;
public static class CaomaoEditorTools
{
    private static bool m_bEndHorizontal = false;
    public static void DrawEvents(string name,List<EventDelegate> evts,Object undoObject)
    {
        if (!CaomaoEditorTools.DrawHead(name, name))
        {
            return;
        }
        CaomaoEditorTools.BeginContents();
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        EventDelegateEditor.Field(evts,undoObject);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        CaomaoEditorTools.EndContents();
    }
    /// <summary>
    /// 画出头部分
    /// </summary>
    /// <param name="headName"></param>
    /// <param name="key"></param>
    public static bool DrawHead(string headName, string key)
    {
        bool state = EditorPrefs.GetBool(key, true);

        GUILayout.Space(3f);
        GUILayout.BeginHorizontal();
        GUI.changed = false;
        headName = "<b><size=11>" + headName + "</size></b>";
        if (state)
        {
            headName = "\u25BC " + headName;
        }
        else
        {
            headName = "\u25BA " + headName;
        }
        if (!GUILayout.Toggle(true, headName, "dragtab", GUILayout.MinWidth(20f))) state = !state;
        if (GUI.changed)
        {
            EditorPrefs.SetBool(key, state);
        }
        GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!state)
        {
            GUILayout.Space(3f);
        }
        return state;
    }
    public static void BeginContents()
    {
        m_bEndHorizontal = true;
        GUILayout.BeginHorizontal();
        EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }
    public static void EndContents()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        if (m_bEndHorizontal)
        {
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(3f);
    }
}
