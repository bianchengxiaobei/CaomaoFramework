using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace CaomaoFramework.GameState
{
    [Serializable]
    public class ClientGameStateMachine : ISerializationCallbackReceiver
    {
        [SerializeField]
        public Dictionary<string, ClientStateBase> m_dicClientStates = new Dictionary<string, ClientStateBase>();
        private ClientStateBase m_oCurrentClientState = null;
        private bool m_bScenePrepared = false;
        private bool m_bResourceLoaded = false;
        private ELoadingStyle m_eCurrentLoadingStyle = ELoadingStyle.DefaultRule;
        private Action m_aCallBackWhenChangeFinished = null;
        //[SerializeField]
        //private List<string> m_listStateIds = new List<string>();
        private string m_sLoadWaitUIEvent;
        private string m_sLoadNormalUIEvent;

        #region Editor
        [SerializeField]
        private string m_serialzedStateMachine;
        [SerializeField]
        private bool m_bHasDesrialized;
        [SerializeField]
        private List<UnityEngine.Object> m_stateObjectReference;
        #endregion
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
            //if (m_listStateIds.Count > 0)
            //{
            //    this.CurrentGameState = m_listStateIds[0];
            //}
            Debug.Log("dqwdqwdw:" + m_dicClientStates.Count);
            this.CurrentGameState = "Max";
            this.IsInChanging = false;           
        }

        public void ConvertToState(string nextGameState, ELoadingStyle loadingStyle, Action callBackOnChangeFinished,string specialStateLoad = "")
        {
            try
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
            catch (Exception e)
            {
                Debug.LogException(e);
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
                Debug.Log("Preload");
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
                    //EventDispatch.Broadcast(this.m_sLoadWaitUIEvent);
                    break;
                case ELoadingStyle.LoadingNormal:
                    //EventDispatch.Broadcast(this.m_sLoadNormalUIEvent);
                    break;
            }
        }
        public void OnBeforeSerialize()
        {
            if (m_stateObjectReference != null && m_stateObjectReference.Any(o => o != null))
            {
                this.m_bHasDesrialized = false;
            }
#if UNITY_EDITOR
            if (Application.isPlaying) return;
            m_serialzedStateMachine = this.Serialize(false, m_stateObjectReference);
#endif
        }

        public void OnAfterDeserialize()
        {
            m_bHasDesrialized = true;
            this.Deserialize(m_serialzedStateMachine, false, m_stateObjectReference);
        }
        public string Serialize(bool pretyJson, List<UnityEngine.Object> objectReferences)
        {
            if (objectReferences == null)
            {
                objectReferences = this.m_stateObjectReference = new List<UnityEngine.Object>();
            }
            else
            {
                objectReferences.Clear();
            }
            return JSONSerializer.Serialize(typeof(DataSerializerStateMachine), new DataSerializerStateMachine(this), pretyJson, objectReferences);
        }
        public DataSerializerStateMachine Deserialize(string serializedUIManager, bool validate, List<UnityEngine.Object> objectReferences)
        {
            if (string.IsNullOrEmpty(serializedUIManager))
            {
                return null;
            }
            if (objectReferences == null)
            {
                objectReferences = this.m_stateObjectReference;
            }
            try
            {
                var data = JSONSerializer.Deserialize<DataSerializerStateMachine>(serializedUIManager, objectReferences);
                if (LoadStateManagerData(data, validate) == true)
                {
                    this.m_serialzedStateMachine = serializedUIManager;
                    return data;
                }
                return null;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }
        private bool LoadStateManagerData(DataSerializerStateMachine data, bool validate)
        {
            if (data == null)
            {
                Debug.LogError("Can't Load graph, cause of null GraphSerializationData provided");
                return false;
            }
            this.m_dicClientStates = new Dictionary<string, ClientStateBase>(data.dicStates);
            return true;
        }
    }
}
