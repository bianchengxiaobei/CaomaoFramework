using System;
using System.Collections.Generic;
namespace CaomaoFramework.Data
{
    public class SkillActionData : GameData<SkillActionData>
    {
        public static string fileName = "skillAction";
        public bool enableStick { get; set; }
        public string sound { get; set; }
        public string soundHit { get; set; }
        public int action { get; set; }
        public int duration { get; set; }
        public int actionTime { get; set; }
        public int nextTime { get; set; }
        public Dictionary<int, float> effects { get; set; }
    }
}
