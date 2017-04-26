using System;
using System.Collections.Generic;

namespace CaomaoFramework
{
    public class CEventParam
    {
        private string eventId;
        private Dictionary<string, object> paramList;

        public CEventParam()
        {
            paramList = new Dictionary<string, object>();
        }

        public CEventParam(string id)
        {
            eventId = id;
            paramList = new Dictionary<string, object>();
        }

        public string GetEventId()
        {
            return eventId;
        }

        public void AddParam(string name, object value)
        {
            paramList[name] = value;
        }

        public object GetParam(string name)
        {
            if (paramList.ContainsKey(name))
            {
                return paramList[name];
            }
            return null;
        }

        public bool HasParam(string name)
        {
            if (paramList.ContainsKey(name))
            {
                return true;
            }
            return false;
        }

        public int GetParamCount()
        {
            return paramList.Count;
        }

        public Dictionary<string, object> GetParamList()
        {
            return paramList;
        }
    }
    public delegate void Callback();
    public delegate void Callback<T>(T arg1);
    public delegate void Callback<T, U>(T arg1, U arg2);
    public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);
    public delegate void Callback<T, U, V, X>(T arg1, U arg2, V arg3, X arg4);
    public class EGameEvent
    {

    }
}
