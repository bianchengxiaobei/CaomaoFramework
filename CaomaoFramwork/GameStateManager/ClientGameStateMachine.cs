using System;
using System.Collections.Generic;
using UnityEngine;
namespace CaomaoFramework.GameState
{
    [Serializable]
    public class ClientGameStateMachine
    {
        [SerializeField]
        public Dictionary<string, ClientStateBase> m_dicClientStates = new Dictionary<string, ClientStateBase>();
        private ClientStateBase m_oCurrentClientState = null;
        private bool m_bScenePrepared = false;
        private bool m_bResourceLoaded = false;
        private ELoadingStyle m_eCurrentLoadingStyle = ELoadingStyle.DefaultRule;
        private Action m_aCallBackWhenChangeFinished = null;
        private List<string> m_listStateIds = new List<string>();
        private string m_sLoadWaitUIEvent;
        private string m_sLoadNormalUIEvent;
        #region 属性
        public string CurrentGameState
        {
            get;
            private set;
        }
        public string NextGameState
        {
            get;
            private set;
        }
        public bool IsInChanging
        {
            get;
            private set;
        }
        #endregion
        public void Init()
        {
            if (m_listStateIds.Count > 0)
            {
                this.CurrentGameState = m_listStateIds[0];
            }
            this.CurrentGameState = "Max";
            this.IsInChanging = false;
            
        }

        public void ConvertToState(string nextGameState, ELoadingStyle loadingStyle, Action callBackOnChangeFinished,string specialStateLoad = "")
        {
            if (!nextGameState.Equals(this.CurrentGameState))
            {
                this.NextGameState = nextGameState;
                this.m_eCurrentLoadingStyle = loadingStyle;
                this.m_aCallBackWhenChangeFinished = (Action)Delegate.Combine(this.m_aCallBackWhenChangeFinished, callBackOnChangeFinished);
                if ("Max" != this.CurrentGameState)
                {
                    if (ELoadingStyle.DefaultRule == this.m_eCurrentLoadingStyle)
                    {
                        if (specialStateLoad == this.CurrentGameState)
                        {
                            this.m_eCurrentLoadingStyle = ELoadingStyle.LoadingNormal;
                        }
                        else
                        {
                            this.m_eCurrentLoadingStyle = ELoadingStyle.LoaidngWait;
                        }
                    }
                    this.SetLoadingVisible(this.m_eCurrentLoadingStyle, true);
                }
                this.IsInChanging = true;
                if ("Max" != this.CurrentGameState)
                {
                    this.m_oCurrentClientState.OnLeave();
                    ResourceManager.singleton.SetAllUnLoadFinishedEventHandler(delegate (bool o)
                    {
                        this.DoChangeToNewState();
                    });
                }
                else
                {
                    this.DoChangeToNewState();
                }
            }
        }
        public void RegisterCallBackOnChangedFinished(Action callBackOnChangeFinished)
        {
            this.m_aCallBackWhenChangeFinished = (Action)Delegate.Combine(this.m_aCallBackWhenChangeFinished, callBackOnChangeFinished);
        }

        private void DoChangeToNewState()
        {
            this.CurrentGameState = this.NextGameState;
            this.NextGameState = "Max";
            this.m_bResourceLoaded = false;
            this.m_bScenePrepared = true;
            this.m_oCurrentClientState = this.m_dicClientStates[this.CurrentGameState];
            this.m_oCurrentClientState.OnEnter();
            ResourceManager.singleton.SetAllLoadFinishedEventHandler(delegate (bool o)
            {
                this.m_bResourceLoaded = true;
                this.OnPartLoaded();
            });
        }
        private void OnPartLoaded()
        {
            if (this.m_bResourceLoaded && this.m_bScenePrepared)
            {
                this.IsInChanging = false;
                this.SetLoadingVisible(this.m_eCurrentLoadingStyle, false);
                if (null != this.m_aCallBackWhenChangeFinished)
                {
                    Action callBackWhenChangeFinished = this.m_aCallBackWhenChangeFinished;
                    this.m_aCallBackWhenChangeFinished = null;
                    callBackWhenChangeFinished();
                }
            }
        }
        private void SetLoadingVisible(ELoadingStyle dlgType, bool bVisible)
        {
            switch (dlgType)
            {
                case ELoadingStyle.LoaidngWait:
                    EventDispatch.Broadcast(this.m_sLoadWaitUIEvent);
                    break;
                case ELoadingStyle.LoadingNormal:
                    EventDispatch.Broadcast(this.m_sLoadNormalUIEvent);
                    break;
            }
        }
    }
}
