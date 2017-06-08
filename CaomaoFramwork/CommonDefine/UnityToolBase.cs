using System;
using System.Collections.Generic;
using UnityEngine;
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
    }
}
