using System;
namespace CaomaoFramework
{
    #region 模块信息
    /*----------------------------------------------------------------
    // 模块名：StoryValueHelper 
    // 创建者：chen
    // 修改者列表：
    // 创建日期：2016.10.12
    // 模块描述：提供剧情使用值的类型转换
    //----------------------------------------------------------------*/
    #endregion
    /// <summary>
    /// 提供剧情使用值的类型转换
    /// </summary>
    public class StoryValueHelper
    {
        public static T CastTo<T>(object obj)
        {
            if (obj is T)
            {
                return (T)obj;
            }
            else
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
        }
        public static IStoryValue<object> AdaptFrom<T>(IStoryValue<T> original)
        {
            return new StoryValueAdapter<T>(original);
        }
    }
}
