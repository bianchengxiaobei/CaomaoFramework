using System;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework.Extenstion;
namespace CaomaoFramework
{
    public class UnityToolBase
    {
        /// <summary>
        /// 递归遍历t下面所有的子Transform信息
        /// </summary>
        /// <param name="t"></param>
        /// <param name="lst"></param>
        public static void FindAllTransform(Transform t, List<Transform> lst)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                lst.Add(child);
                FindAllTransform(child, lst);
            }
        }
        /// <summary>
        /// 将字符串转化成为对应类型的值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetValue(string value, Type type)
        {
            if (null == type)
            {
                return null;
            }
            else if (type == typeof(int))
            {
                return Convert.ToInt32(value);
            }
            else if (type == typeof(float))
            {
                return float.Parse(value);
            }
            else if (type == typeof(byte))
            {
                return Convert.ToByte(value);
            }
            else if (type == typeof(double))
            {
                return Convert.ToDouble(value);
            }
            else if (type == typeof(bool))
            {
                if (value == "false")
                {
                    return false;
                }
                else if (value == "true")
                {
                    return true;
                }
            }
            else if (type == typeof(string))
            {
                return value;
            }
            else if (type == typeof(Vector3))
            {
                Vector3 result = default(Vector3);
                try
                {
                    string[] array = value.Split(new char[]
                    {
                    ','
                    });
                    if (array.Length == 3)
                    {
                        result.x = (float)Convert.ToDouble(array[0]);
                        result.y = (float)Convert.ToDouble(array[1]);
                        result.z = (float)Convert.ToDouble(array[2]);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                return result;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type t = type.GetGenericArguments()[0];
                var list = value.StringToStringList();
                var result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                foreach (var item in list)
                {
                    var v = GetValue(item, t);
                    type.GetMethod("Add").Invoke(result, new object[] { v });
                }
                return result;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type[] types = type.GetGenericArguments();
                var map = value.StringToStringMap();
                var result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                foreach (var item in map)
                {
                    var key = GetValue(item.Key, types[0]);
                    var v = GetValue(item.Value, types[1]);
                    type.GetMethod("Add").Invoke(result, new object[] { key, v });
                }
                return result;
            }
            return null;
        }       
    }
}
