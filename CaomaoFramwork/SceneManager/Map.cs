using System;
using System.Collections.Generic;
using CaomaoFramework;
using UnityEngine;
namespace CaomaoFramework.Scene
{
    public class Map
    {
        private int m_iMapId;
        private MapData m_oMapData;
        public MapData MapData
        {
            get
            {
                if (m_oMapData != null)
                {
                    return this.m_oMapData;
                }
                return null;
            }
        }
        public int MapId
        {
            get { return this.m_iMapId; }
        }
        public GameObject RealMapObj
        {
            get;set;
        }
        public bool Init(int _mapId)
        {
            this.m_iMapId = _mapId;
            this.m_oMapData = GameData<MapData>.dataMap[_mapId];
            return true;
        }
        public void DisActive()
        {
            if (RealMapObj != null)
            {
                RealMapObj.SetActive(false);
            }
        }
        public void Active()
        {
            if (RealMapObj != null)
            {
                RealMapObj.SetActive(true);
            }
        }
        public void Dispose()
        {
            this.m_oMapData = null;
            GameObject.Destroy(this.RealMapObj);
        }
    }
}
