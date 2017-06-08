using System;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
namespace CaomaoFramework.Scene
{
    public class MapData : GameData<MapData>
    {
        public const string fileName = "mapData";
        public int ID
        {
            get;
            set;
        }
        public Vector3 InitPos
        {
            get;
            set;
        }
        public Vector3 InitRotation
        {
            get;
            set;
        }
        public string SceneName
        {
            get;
            set;
        }
        public string MapPath
        {
            get;set;
        }
    }
}
