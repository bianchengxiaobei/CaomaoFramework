using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaomaoFramework.EntityFsm
{
    public static class MotionState
    {
        static readonly public string IDLE = "idle";
        static readonly public string WALKING = "walking";
        static readonly public string DEAD = "dead";
        static readonly public string CHARGING = "charging";
        static readonly public string ATTACKING = "attacking";
        static readonly public string HIT = "hit";
        static readonly public string PREPARING = "preparing";
        static readonly public string ROLL = "roll";

        static readonly public string LOCKING = "locking";
        static readonly public string PICKING = "picking";
    }

}
