using System.Collections;
using System.Collections.Generic;
using SunkenRuins;
using UnityEngine;

namespace SunkenRuins
{
    public class ShellStat : EnemyStat
    {
        public float EngulfTime { get { return 3f; } }
        [SerializeField] private float engulfDelayTime = 5f; public float EngulfDelayTime { get { return engulfDelayTime; } }

        [SerializeField] private float attackCoolTime = 3f; public float AttackCoolTime { get { return attackCoolTime; } }
        [SerializeField] private int damagePerAttack = 10; public int DamagePerAttack { get { return damagePerAttack; } }

        public int TotalKeyAmount { get { return 10; } }
    }
}