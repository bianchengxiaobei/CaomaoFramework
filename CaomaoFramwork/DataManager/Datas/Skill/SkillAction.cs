using System;
using System.Collections.Generic;

namespace CaomaoFramework.Data
{
    public class SkillAction : GameData<SkillAction>
    {
        // 客户端表现
        public int cameraTweenId { get; protected set; }
        public float cameraTweenST { get; protected set; }
        public float cameraTweenSL { get; protected set; }
        public string sound { get; protected set; }
        public float soundST { get; protected set; }
        public string soundHit { get; protected set; }
        public int freeze { get; protected set; }
        public int removeCollider { get; protected set; }
        public int replication { get; protected set; }
        public int action { get; protected set; }
        public int duration { get; protected set; }
        public int actionTime { get; protected set; }
        public int nextHitTime { get; protected set; }
        public int enableStick { get; protected set; }
        public float extraSpeed { get; protected set; }
        public float extraSt { get; protected set; }
        public float extraSl { get; protected set; }
        public float teleportDistance { get; protected set; }
        public int damageTriggerFrame { get; protected set; }
        public List<int> hitAction { get; protected set; }
        public List<int> deadAction { get; protected set; }
        public List<float> hitSfx { get; protected set; }
        public float hitExtraSpeed { get; protected set; }
        public float hitExtraSt { get; protected set; }
        public float hitExtraSl { get; protected set; }
        public List<int> hitHover { get; protected set; }
        public List<int> hitFly { get; protected set; }
        public float stiff { get; protected set; }
        public Dictionary<int, float> sfx { get; protected set; }
        public List<int> entitys { get; protected set; }
        public List<int> spawnPos { get; protected set; }

        // 技能效果
        public float hitXoffset { get; protected set; }
        public float hitYoffset { get; protected set; }
        public float angleOffset { get; protected set; }
        /// <summary>
        /// 技能攻击目标类型
        /// </summary>
        public int targetType { get; protected set; }
        /// <summary>
        /// 技能范围类型
        /// </summary>
        public int targetRangeType { get; protected set; }
        /// <summary>
        /// 某个技能范围类型的参数，比如说半径啥的
        /// </summary>
        public List<float> targetRangeParam { get; protected set; }
        public int maxTargetCount { get; protected set; }
        public int activeBuffMode { get; protected set; }
        public int activeBuff { get; protected set; }
        /// <summary>
        /// 伤害标志，1伤害型   2治愈型    3.既有治愈也有伤害
        /// </summary>
        public int damageFlag { get; protected set; }
        /// <summary>
        /// 技能伤害
        /// </summary>
        public int damage { get; protected set; }
        /// <summary>
        /// 技能加成类型，比如有ap，ad，基于生命值
        /// 1.ad    2.ap    3.hp    4.护甲    5.魔抗
        /// </summary>
        public byte damageAddType { get; protected set; }
        /// <summary>
        /// 技能伤害加成系数
        /// </summary>
        public float damageAdd { get; protected set; }
        /// <summary>
        /// 治愈值
        /// </summary>
        public List<int> heal { get; protected set; }
        public List<int> casterAddBuff { get; protected set; }
        public List<int> casterDelBuff { get; protected set; }
        public List<int> targetAddBuff { get; protected set; }
        public List<int> targetDelBuff { get; protected set; }
        public List<int> casterHeal { get; protected set; }
        public List<int> targetHeal { get; protected set; }
        public int actionDuration { get; protected set; }
        public int castPosType { get; protected set; }
        public int actionBeginDuration { get; protected set; }
        public int actionEndDuration { get; protected set; }

        public int triggerEvent { get; protected set; }
        public int spawnPoint { get; protected set; }

        static public readonly string fileName = "SkillAction";
    }
}


