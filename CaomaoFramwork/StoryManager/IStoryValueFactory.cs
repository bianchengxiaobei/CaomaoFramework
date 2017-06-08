namespace CaomaoFramework
{
    #region 模块信息
    /*----------------------------------------------------------------
    // 模块名：IStoryValueFactory 
    // 创建者：chen
    // 修改者列表：
    // 创建日期：2016.10.12
    // 模块描述：剧情使用值工厂接口
    //----------------------------------------------------------------*/
    #endregion
    /// <summary>
    /// 剧情使用值工厂接口
    /// </summary>
    public interface IStoryValueFactory
    {
        IStoryValue<object> Build(ScriptableData.ISyntaxComponent param);
    }
    public sealed class StoryValueFactoryHelper<C> : IStoryValueFactory where C : IStoryValue<object>, new()
    {
        public IStoryValue<object> Build(ScriptableData.ISyntaxComponent param)
        {
            C c = new C();
            c.InitFromDsl(param);
            return c;
        }
    }
}
