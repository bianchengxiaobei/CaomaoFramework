using System;
using System.Collections.Generic;

namespace CaomaoFramework.GameState
{
    public enum ELoadingStyle
    {
        DefaultRule,
        None,
        LoaidngWait,
        LoadingNormal
    }
    public class ClientStateChangeArgs
    {
        public string sClientState;
        public ELoadingStyle eLoadingStyle;
        public Action aCallBack;
    }
}
