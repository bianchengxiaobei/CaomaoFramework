using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：XMLParser
// 创建者：chen
// 修改者列表：
// 创建日期：2016.3.7
// 模块描述：
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework.Data
{
    public class XMLParser
    {
        public static bool LoadIntoMap(string filename, out Dictionary<int, Dictionary<string, string>> map)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(filename);
                if (null == xml)
                {
                    Debug.LogError("xml文件不存在" + filename);
                    map = null;
                    return false;
                }
                else
                {
                    map = LoadIntoMap(xml, filename);
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("XML加载出错:" + e.ToString());
                map = null;
                return false;
            }
        }
        public static Dictionary<int, Dictionary<string, string>> LoadIntoMap(XmlDocument doc, string filePath)
        {
            Dictionary<int, Dictionary<string, string>> result = new Dictionary<int, Dictionary<string, string>>();
            int index = 0;
            XmlNode root = doc.SelectSingleNode("root");
            foreach (XmlNode node in root.ChildNodes)//root的子节点，就拿map_setting来讲就是map子节点
            {
                index++;
                if (null == node.ChildNodes || 0 == node.ChildNodes.Count)
                {
                    Debug.LogWarning("The XML is empty nodes");
                    continue;
                }
                int key = int.Parse(node.ChildNodes[0].InnerText);//map[0] ==> id
                if (result.ContainsKey(key))
                {
                    Debug.LogWarning(string.Format("key:{0} is already loaded", key));
                    continue;
                }
                var children = new Dictionary<string, string>();
                result.Add(key, children);
                for (int i = 1; i < node.ChildNodes.Count; i++)//去除id，所以i从1开始(这样id节点得放在第一个位置)
                {
                    var childNode = node.ChildNodes[i];
                    string tag;
                    if (childNode.Name.Length < 3)
                    {
                        tag = childNode.Name;
                    }
                    else
                    {
                        var tagTial = childNode.Name.Substring(childNode.Name.Length - 2, 2);//截取最后两个字符
                        if (tagTial == "_i" || tagTial == "_s" || tagTial == "_f" || tagTial == "_l" || tagTial == "k" || tagTial == "_m" || tagTial == "_b")
                        {
                            tag = childNode.Name.Substring(0, childNode.Name.Length - 2);
                        }
                        else
                        {
                            tag = childNode.Name;
                        }
                    }
                    if (childNode != null && !children.ContainsKey(tag))
                    {
                        if (string.IsNullOrEmpty(childNode.InnerText))
                        {
                            children.Add(tag, "");
                        }
                        else
                        {
                            children.Add(tag, childNode.InnerText.Trim());
                        }
                    }
                    else
                    {
                        Debug.LogWarning(string.Format("XML文件子节点的Key:{0} 已经存在", tag));
                    }
                }
            }
            return result;
        }
    }
}