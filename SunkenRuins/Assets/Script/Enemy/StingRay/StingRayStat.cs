using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public class StingRayStat : EnemyStat
    {
        [Header("Time")]
        public float dashContinueTime = 2f;
        public float dashDelayTime = 2f;
        
        [Header("Movement")]
        public float patrolRange = 5;
        public float initialMoveSpeed = 5f;
        public float dashMoveSpeed = 10f; // 이동 속도가 즉시 변경된다 해서 그냥 변수로 보관함
    }
}