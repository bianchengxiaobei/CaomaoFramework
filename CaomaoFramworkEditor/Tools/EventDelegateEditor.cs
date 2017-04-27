using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CaomaoFramework;
using System.Reflection;
using Entry = PropertyReferenceDrawer.Entry;
using System;
using Object = UnityEngine.Object;
public class EventDelegateEditor
{
    public static void Field(List<EventDelegate> evts,Object undoObject)
    {
        if (null == evts)
        {
            return;
        }
        bool targetPresent = false;
        bool isValid = false;
        for (int i = 0; i < evts.Count; ++i)
        {
            var del = evts[i];
            if (null == del || (null == del.target && !del.isValid))
            {
                evts.RemoveAt(i);
                continue;
            }
            Field(del, undoObject);
            EditorGUILayout.Space();
            if (del.target == null && !del.isValid)
            {
                evts.RemoveAt(i);
                continue;
            }
            else
            {
                if (del.target != null) targetPresent = true;
                isValid = true;
            }
        }
        EventDelegate newDel = new EventDelegate();
        Field(newDel, undoObject);
        if (newDel.target != null)
        {
            targetPresent = true;
            evts.Add(newDel);
        }
    }
    public static bool Field(EventDelegate del,Object undoObject)
    {
        if (null == del)
        {
            return false;
        }
        bool prev = GUI.changed;
        GUI.changed = false;
        bool retVal = false;
        MonoBehaviour target = del.target;
        bool remove = false;

        if (del.target != null || del.isValid)
        {
            EditorGUIUtility.labelWidth = 82f;

            if (null == del.target && del.isValid)
            {
                EditorGUILayout.LabelField("事件", del.ToString());
            }
            else
            {
                target = EditorGUILayout.ObjectField("事件", del.target, typeof(MonoBehaviour), true) as MonoBehaviour;
            }
            GUILayout.Space(-18f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(70f);
            if (GUILayout.Button("", "ToggleMixed", GUILayout.Width(20f), GUILayout.Height(16f)))
            {
                target = null;
                remove = true;
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            target = EditorGUILayout.ObjectField("事件", del.target, typeof(MonoBehaviour), true) as MonoBehaviour;
        }
        if (remove)
        {
            del.Clear();
            EditorUtility.SetDirty(undoObject);
        }
        else if(del.target != target)
        {
            del.target = target;
            EditorUtility.SetDirty(undoObject);
        }
        if (del.target != null && del.target.gameObject != null)
        {
            GameObject go = del.target.gameObject;
            List<Entry> list = GetMethods(go);
            int index = 0;
            string[] names = PropertyReferenceDrawer.GetNames(list, del.ToString(), out index);
            int choice = 0;

            GUILayout.BeginHorizontal();
            choice = EditorGUILayout.Popup("方法", index, names);
            GUILayout.Space(18f);

            GUILayout.EndHorizontal();
            if (choice > 0 && choice != index)
            {
                Debug.Log(choice + index);
                Entry entry = list[choice - 1];
                del.target = entry.target as MonoBehaviour;
                del.methodName = entry.name;
                del.extentName = entry.extendName;
                del.ParamTypes = entry.paramTypes;
                EditorUtility.SetDirty(undoObject);
                retVal = true;
            }
            GUI.changed = false;
            EventDelegate.Parameter[] ps = del.parameters;
            if (ps != null)
            {
                for (int i = 0; i < ps.Length; ++i)
                {
                    EventDelegate.Parameter param = ps[i];
                    Type t = Type.GetType(del.ParamTypes[i]);
                    Object obj = null;
                    if (t != typeof(MonoBehaviour))
                    {
                        if (t == typeof(string))
                        {
                            param.stringValue = EditorGUILayout.TextField(new GUIContent("参数" + i), param.stringValue);
                            if (GUI.changed)
                            {
                                GUI.changed = false;
                                EditorUtility.SetDirty(undoObject);
                            }
                        }
                        else if (t == typeof(int))
                        {
                            param.intValue = EditorGUILayout.IntField("参数" + i, param.intValue);
                            if (GUI.changed)
                            {
                                GUI.changed = false;
                                EditorUtility.SetDirty(undoObject);
                            }
                        }
                        else if (t == typeof(GenericType))
                        {
                            //如果是泛型的话

                        }
                        PropertyReferenceDrawer.filter = typeof(void);
                        PropertyReferenceDrawer.canConvert = true;
                    }
                    else
                    {
                        obj = EditorGUILayout.ObjectField("参数" + i, param.obj, typeof(Object), true);

                        if (GUI.changed)
                        {
                            GUI.changed = false;
                            param.obj = obj;
                            EditorUtility.SetDirty(undoObject);
                        }
                        if (obj == null) continue;

                        GameObject selGO = null;
                        System.Type type = obj.GetType();
                        if (type == typeof(GameObject)) selGO = obj as GameObject;
                        else if (type.IsSubclassOf(typeof(Component))) selGO = (obj as Component).gameObject;

                        if (selGO != null)
                        {
                            // Parameters must be exact -- they can't be converted like property bindings
                            PropertyReferenceDrawer.filter = param.expectedType;
                            PropertyReferenceDrawer.canConvert = false;
                            List<PropertyReferenceDrawer.Entry> ents = PropertyReferenceDrawer.GetProperties(selGO, true, false);

                            int selection;
                            string[] props = GetNames(ents, EditorTool.GetFuncName(param.obj, param.field), out selection);

                            GUILayout.BeginHorizontal();
                            int newSel = EditorGUILayout.Popup(" ", selection, props);
                            GUILayout.Space(18f);
                            GUILayout.EndHorizontal();

                            if (GUI.changed)
                            {
                                GUI.changed = false;

                                if (newSel == 0)
                                {
                                    param.obj = selGO;
                                    param.field = null;
                                }
                                else
                                {
                                    param.obj = ents[newSel - 1].target;
                                    param.field = ents[newSel - 1].name;
                                }
                                EditorUtility.SetDirty(undoObject);
                            }
                        }
                        else if (!string.IsNullOrEmpty(param.field))
                        {
                            param.field = null;
                            EditorUtility.SetDirty(undoObject);
                        }

                        PropertyReferenceDrawer.filter = typeof(void);
                        PropertyReferenceDrawer.canConvert = true;
                    }                           
  
                }
            }
        }
        else retVal = GUI.changed;
        GUI.changed = prev;
        return retVal;
    }
    static public List<Entry> GetMethods(GameObject target)
    {
        MonoBehaviour[] comps = target.GetComponents<MonoBehaviour>();

        List<Entry> list = new List<Entry>();

        for (int i = 0, imax = comps.Length; i < imax; ++i)
        {
            MonoBehaviour mb = comps[i];
            if (mb == null) continue;

            MethodInfo[] methods = mb.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

            for (int b = 0; b < methods.Length; ++b)
            {
                MethodInfo mi = methods[b];
                if (mi.ReturnType == typeof(void))
                {
                    ParameterInfo[] infos = mi.GetParameters();
                    string param = "";
                    for (int j = 0; j < infos.Length; j++)
                    {
                        param += infos[j].ParameterType.ToString();
                        if (j == (infos.Length - 1))
                        {
                            break;
                        }
                        param += ";";
                    }
                    param = string.Format("({0})", param);
                    string name = mi.Name;
                    if (name == "Invoke") continue;
                    if (name == "InvokeRepeating") continue;
                    if (name == "CancelInvoke") continue;
                    if (name == "StopCoroutine") continue;
                    if (name == "StopAllCoroutines") continue;
                    if (name == "BroadcastMessage") continue;
                    if (name.StartsWith("SendMessage")) continue;
                    if (name.StartsWith("set_")) continue;
                    Entry ent = new Entry();
                    ent.target = mb;
                    ent.name = mi.Name;
                    ent.extendName = mi.Name + param;
                    ent.paramTypes = EditorTool.GetFuncParamType(param);
                    list.Add(ent);
                }
            }
        }
        return list;
    }
    static public string[] GetNames(List<Entry> list, string choice, out int index)
    {
        index = 0;
        string[] names = new string[list.Count + 1];
        names[0] = "<GameObject>";

        for (int i = 0; i < list.Count;)
        {
            Entry ent = list[i];
            string del = EditorTool.GetFuncName(ent.target, ent.extendName);
            names[++i] = del;
            if (index == 0 && string.Equals(del, choice))
                index = i;
        }
        return names;
    }
}