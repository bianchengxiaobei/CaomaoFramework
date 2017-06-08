using UnityEngine;
using System.Collections.Generic;
using System;
using CaomaoFramework.Data;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：GameData
// 创建者：chen
// 修改者列表：
// 创建日期：2017.5.3
// 模块描述：
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework
{
    public abstract class GameData
    {
        #region 字段

        #endregion
        #region 属性
        public int id
        {
            get;
            protected set;
        }
        protected static Dictionary<int, T> GetDataMap<T>()
        {
            Dictionary<int, T> dataMap;
            var type = typeof(T);
            var fileNameField = type.GetField("fileName");
            if (fileNameField != null)
            {
                string filename = fileNameField.GetValue(null) as string;
                dataMap = GameDataController.Instance.FormatXMLData(filename, typeof(Dictionary<int, T>), type) as Dictionary<int, T>;
            }
            else
            {
                dataMap = new Dictionary<int, T>();
            }
            return dataMap;
        }
        #endregion
        #region 构造方法
        #endregion
        #region 公有方法
        #endregion
        #region 私有方法
        #endregion
    }
    public abstract class GameData<T> : GameData where T : GameData<T>
    {
        private static Dictionary<int, T> m_dataMap;
        public static Dictionary<int, T> dataMap
        {
            get
            {
                if (m_dataMap == null)
                {
                    m_dataMap = GetDataMap<T>();
                }
                return m_dataMap;
            }
            set { m_dataMap = value; }
        }
    }
    public abstract class DataLoader
    {
        protected static readonly bool bm_isPreloadData = true;
        protected readonly string m_resourcePath;
        protected readonly string m_fileExtention;
        protected Action<int, int> m_progress;
        protected Action m_finished;
        protected DataLoader()
        {
            // m_resourcePath = Application.dataPath + "/../config/";
            m_resourcePath = ResourceManager.singleton.strAssetPath+"/config/";
            m_fileExtention = ".xml";
        }
    }
    public class GameDataController : DataLoader
    {
        private List<Type> m_defaultData = new List<Type>()
        {

        };
        private static GameDataController m_instance;

        public static GameDataController Instance
        {
            get { return GameDataController.m_instance; }
        }

        static GameDataController()
        {
            m_instance = new GameDataController();
        }
        /// <summary>
        /// 讲XML文件转换成Dictionary<int,T>格式
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="dicType"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object FormatXMLData(string filename, Type dicType, Type type)
        {
            filename = string.Concat(this.m_resourcePath, filename, this.m_fileExtention);
            object result = null;
            try
            {
                result = dicType.GetConstructor(Type.EmptyTypes).Invoke(null);
                Dictionary<int, Dictionary<string, string>> map;
                if (XMLParser.LoadIntoMap(filename, out map))
                {
                    var props = type.GetProperties();//取得类的所有属性
                    foreach (var item in map)
                    {
                        var t = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                        foreach (var prop in props)
                        {
                            if (prop.Name.Equals("ID"))
                            {
                                prop.SetValue(t, item.Key, null);//如果是id的话，就设置属性值
                            }
                            else
                            {
                                if (item.Value.ContainsKey(prop.Name))
                                {
                                    var value = GetValue(item.Value[prop.Name], prop.PropertyType);
                                    prop.SetValue(t, value, null);
                                }
                            }
                        }
                        dicType.GetMethod("Add").Invoke(result, new object[] { item.Key, t });//result是dicType的实例，也就是dic<int,T>
                        //item.key=>id,t是泛型T的实例
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
            return result;
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
            return null;
        }
    }
}