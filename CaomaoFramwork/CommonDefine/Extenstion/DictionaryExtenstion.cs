using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaomaoFramework.Extenstion
{
    public static class DictionaryExtenstion
    {
        /// <summary>
        /// 取得字典中的值，如果没有就是默认类型的值
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetValueOrDefault<Tkey, TValue>(this IDictionary<Tkey, TValue> dictionary, Tkey key)
        {
            TValue value = default(TValue);
            dictionary.TryGetValue(key, out value);
            return value;
        }
        /// <summary>
        /// 取得字典中的值，如果没有就是默认自定义值
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetValueOrDefault<Tkey, TValue>(this IDictionary<Tkey, TValue> dictionary, Tkey key, Func<TValue> provider)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : provider();
        }
        /// <summary>
        /// 取得字典中的值，如果没有就是默认值
        /// </summary>
        /// <typeparam name="Tkey">key</typeparam>
        /// <typeparam name="TValue">默认值</typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetValueOrDefault<Tkey, TValue>(this IDictionary<Tkey, TValue> dictionary, Tkey key, TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }
    }
}
