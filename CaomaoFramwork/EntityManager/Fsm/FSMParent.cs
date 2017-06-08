using System;
using System.Collections.Generic;

namespace CaomaoFramework
{
    public abstract class FSMParent
    {
        #region 字段
        protected Dictionary<string, IEntityState> m_theFSM = new Dictionary<string, IEntityState>();
        #endregion
        #region 属性
        #endregion
        #region 构造方法
        public FSMParent()
        {

        }
        #endregion
        #region 公有方法
        public virtual void ChangeStatus(EntityParent owner, string newState, params object[] args)
        {

        }
        #endregion
        #region 私有方法
        #endregion
    }
}
