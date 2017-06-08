using System;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework.Scene;
namespace CaomaoFramework
{
    public class SceneBase
    {
        private Map m_oCurrentMap;
        private Dictionary<int, Map> m_dicMaps = new Dictionary<int, Map>();
        public int MapId
        {
            get
            {
                if (null == m_oCurrentMap)
                {
                    return -1;
                }
                return m_oCurrentMap.MapId;
            }
        }
        public Vector3 InitPos
        {
            get
            {
                if (null == m_oCurrentMap)
                {
                    return Vector3.zero;
                }
                return m_oCurrentMap.MapData.InitPos;
            }         
        }
        public Map CurrentMap
        {
            get { return this.m_oCurrentMap; }
        }
        public Vector2 InitRotation
        {
            get
            {
                if (null == m_oCurrentMap)
                {
                    return Vector3.zero;
                }
                return m_oCurrentMap.MapData.InitRotation;
            }
        }
        public bool CreateMap(int _mapId)
        {
            this.m_oCurrentMap = new Map();
            if (this.m_oCurrentMap.Init(_mapId))
            {
                this.m_dicMaps.Add(_mapId, this.m_oCurrentMap);
                return true;
            }
            return false;
        }
        public bool ChangeMap(int _mapId)
        {
            if (this.m_oCurrentMap != null)
            {
                this.m_oCurrentMap.DisActive();
                if (this.m_dicMaps.ContainsKey(_mapId))
                {
                    this.m_dicMaps[_mapId].Active();
                }
                else
                {
                    this.CreateMap(_mapId);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Dispose()
        {
            this.m_oCurrentMap = null;
            foreach (var map in m_dicMaps)
            {
                map.Value.Dispose();
            }
            this.m_dicMaps.Clear();
            
        }
    }
}
