using System.Collections;
using System.Collections.Generic;
using SunkenRuins;
using UnityEngine;

namespace SunkenRuins
{
    public class ShellStat : EnemyStat
    {
        public float EngulfTime { get { return 3f; } }
        public float EngulfDelayTime { get { return 5f; } }

        public float AttackCoolTime { get { return 1f; } }
        public int DamagePerAttack { get { return 10; } }

        public int TotalKeyAmount { get { return 10; } }
    }
}