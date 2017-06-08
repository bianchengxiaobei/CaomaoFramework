using System;
using System.Collections.Generic;
namespace CaomaoFramework.Data
{
    public class SkillActionData : GameData<SkillActionData>
    {
        public string Sound { get; set; }
        public string SoundHit { get; set; }
        public int Action { get; set; }
        public int Duration { get; set; }
        public int ActionTime { get; set; }
        public int NextTime { get; set; }
        public Dictionary<int, float> effects { get; set; }
    }
}
