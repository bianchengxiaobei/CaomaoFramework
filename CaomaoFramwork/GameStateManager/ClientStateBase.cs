using System;
namespace CaomaoFramework
{
    [Serializable]
    public abstract class ClientStateBase
    {
        public virtual void OnEnter()
        {
        }

        public virtual void OnLeave()
        {
        }
    }
}
