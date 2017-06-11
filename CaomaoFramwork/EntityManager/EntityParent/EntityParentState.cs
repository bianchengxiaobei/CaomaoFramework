using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaomaoFramework
{
    public partial class EntityParent
    {
        #region 字段
        #endregion
        #region 属性
        #endregion
        #region 构造方法
        #endregion
        #region 公有方法
        /// <summary>
        /// Entity能否移动
        /// </summary>
        /// <returns></returns>
        public bool CanMove()
        {
            return this.canMove;
        }
        /// <summary>
        /// 通过StateFlag设置Action为idle动作
        /// </summary>
        public void SetActionByStateFlagInIdleState()
        {
            //if ((this.stateFlag & dizzy_state) != 0)//也就是stateFlag的第二位必须为1
            //{
            //    SetAction(16);
            //    AddCallbackInFrames(() => { SetAction(999); });
            //}
        }
        #endregion
        #region 子类重写方法
        protected virtual void StateChange(ulong value)
        {

        }
        #endregion
    }
}
