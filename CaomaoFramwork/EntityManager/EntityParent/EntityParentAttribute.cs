using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework.Effect;
using CaomaoFramework.EntityFsm;
namespace CaomaoFramework
{
    public partial class EntityParent
    {
        #region 字段
        public Animator animator;
        public Animation animation;
        public Animator weaponAnimator = null;
        public GameMotor motor;
        public EffectHandler effectHandler;
        protected EffectManager effectManager;
        protected BattleManagerBase m_battleManager;
        public FSMParent fsmMotion;
        public AudioSource audioSource;
        protected string currentMotionState = MotionState.IDLE;
        public uint ID = 0;
        public bool stiff = false;
        public bool IsVisiable = false;//角色是否可见
        /// <summary>
        /// 击飞
        /// </summary>
        public bool knock_down = false;
        /// <summary>
        /// 受击倒地
        /// </summary>
        public bool hit_ground = false;
        public bool charging = false;
        public bool canMove = true;//能否移动

        public int currSpellID = -1;//技能id，小于等于0为当前空闲
        public int currHitAction = -1;//当前hitAction的ID
        public int hitActionIdx = 0;//当前技能hitAction索引

        public string skillActName = "";//当前技能动作名
        public float aiRate = 1;//AI速率，就是播放动作的快慢
        public List<uint> hitTimer = new List<uint>();//技能动作定时器的id
        public uint delayAttackTimerID = 0;//延迟攻击定时器的id
        public uint hitTimerID = 0;//当前技能定时器id
        public bool walkingCastSkill = false;//是否在跑动中释放技能
        public bool isBackDirection = false; //移动是否反方向
        public float moveSpeedRate = 1;//移动速率
        public float gearMoveSpeedRate = 1;//关卡移动速率
        private float m_speed;//移动速度 
        private string m_name;//昵称
        private uint m_exp;//经验
        private uint m_nextLevelExp = 1;
        private byte m_level;//等级
        protected uint m_curHp;//生命值

        protected ulong stateFlag = 0;//角色状态标记位
        public const int dizzy_state = 1 << 1;//眩晕状态,2
        public const int immobilize_state = 1 << 3;//固定状态,8
        public const int silent_state = 1 << 4;//沉默状态,16
        public const int immunity_state = 1 << 10;//免疫状态,1024
        public const int no_hit_state = 1 << 11;//没有被攻击到,2048
        //装备物品信息
        public Hashtable itemInfo;
        //渲染节点组
        protected Hashtable renderNodeObjects = new Hashtable();
        //挂载点数据
        public Hashtable bindNodeTable = new Hashtable();
        //Mesh数据
        public Hashtable meshTable = new Hashtable();
        //骨骼根节点
        protected GameObject boneRoot = null;
        private Dictionary<string, object> objectAttrs = new Dictionary<string, object>();
        private Dictionary<string, int> intAttrs = new Dictionary<string, int>();
        private Dictionary<string, double> doubleAttrs = new Dictionary<string, double>();
        private Dictionary<string, string> stringAttrs = new Dictionary<string, string>();

        #endregion
        #region 属性
        /// <summary>
        /// 客户端实例
        /// </summary>
        public GameObject GameObject { get; set; }
        public Transform Transform { get; set; }
        public Vector3 Position = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;
        public ActorParent Actor { get; set; }
        /// <summary>
        /// 现在的Entity状态，默认为Idle
        /// </summary>
        public string CurrentMotionState
        {
            get
            {
                return this.currentMotionState;
            }
            set
            {
                currentMotionState = value;
            }
        }
        /// <summary>
        /// 状态标记（改变状态在这里实现）
        /// </summary>
        public virtual ulong StateFlag
        {
            get { return this.stateFlag; }
            set
            {
                StateChange(value);
                stateFlag = value;
            }
        }
        /// <summary>
        ///速度
        /// </summary>
        public float Speed
        {
            get { return this.m_speed; }
            set
            {
                this.m_speed = value;
                //OnPropertyChanged("Speed", value);
            }
        }
        /// <summary>
        /// 头像图标
        /// </summary>
        public virtual string HeadIcon
        {
            get
            {
                return null;
            }
        }
        /// <summary>
        /// 玩家等级
        /// </summary>
        public virtual byte Level
        {
            get { return this.m_level; }
            set
            {
                this.m_level = value;
            }
        }
        /// <summary>
        /// 玩家现在的生命值
        /// </summary>
        public virtual uint CurHp
        {
            get
            {
                return this.m_curHp;
            }
            set
            {
                if (this.m_curHp != value)
                {
                    this.m_curHp = value;
                    //通知界面血量改了
                }
            }
        }

        public Dictionary<string, object> ObjectAttrs
        {
            get { return this.objectAttrs; }
            set { objectAttrs = value; }
        }
        /// <summary>
        /// 存储实体所有int类型的属性值
        /// </summary>
        public Dictionary<string, int> IntAttrs
        {
            get { return this.intAttrs; }
            set { this.intAttrs = value; }
        }
        /// <summary>
        /// 存储实体所有double类型的属性值
        /// </summary>
        public Dictionary<string, double> DoubleAttrs
        {
            get { return this.doubleAttrs; }
            set { this.doubleAttrs = value; }
        }
        /// <summary>
        /// 存储实体所有string类型的属性值
        /// </summary>
        public Dictionary<string, string> StringAttrs
        {
            get { return this.stringAttrs; }
            set { this.stringAttrs = value; }
        }
        #endregion
        #region 构造方法
        #endregion
        #region 公有方法
        public virtual void SetEntityInfo(EntityParentAttachDataBase info)
        {

        }
        #endregion
        #region 私有方法
        #endregion
    }
}
