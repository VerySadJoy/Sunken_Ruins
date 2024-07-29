using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public class StingRayStat : EnemyStat
    {
        // Time
        public float dashContinueTime { get { return 2f; } }
        public float dashDelayTime { get { return 2f; } } 
        public float fullChargeTime { get { return 1.5f; } } 

        // Movement
        public float dashMoveSpeed { get { return 7f; } } // 이동 속도가 즉시 변경된다 해서 그냥 변수로 보관함
        public float initialMoveSpeed { get { return 4f; } }
        public float patrolRange { get { return 5f; } }

        // Interaction
        private int stingRayDamageAmount = 5;
        private void Awake()
        {
            damageAmount = stingRayDamageAmount;
        }
    }
}