using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaomaoFramework
{
    public interface IEntityState
    {
        /// <summary>
        /// 进入状态
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="args"></param>
        void Enter(EntityParent entity, params object[] args);
        /// <summary>
        /// 离开状态
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="args"></param>
        void Exit(EntityParent entity, params object[] args);
        /// <summary>
        /// 处理状态
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="args"></param>
        void Process(EntityParent entity, params object[] args);
    }
}
