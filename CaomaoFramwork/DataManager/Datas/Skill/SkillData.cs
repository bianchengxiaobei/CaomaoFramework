using System;
using System.Collections.Generic;
namespace CaomaoFramework.Data
{
    public class SkillData : GameData<SkillData>
    {
        //技能数据
        public string name { get; protected set; }
        public int level { get; protected set; }
        public int desc { get; protected set; }
        public int icon { get; protected set; }
        public int posi { get; protected set; }

        //学习限制
        //施放条件
        public List<float> cd { get; protected set; }
        public int energy { get; protected set; }
        public int castTime { get; protected set; }
        public int castRange { get; protected set; }

        public List<int> skillAction { get; protected set; }

        public int totalActionDuration = 0;
        public static readonly string fileName = "SkillData";
    }
}
