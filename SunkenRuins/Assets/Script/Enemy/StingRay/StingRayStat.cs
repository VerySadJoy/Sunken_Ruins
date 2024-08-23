using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public class StingRayStat : EnemyStat
    {
        // Time
        [Header("Time")]
        [SerializeField] private float dashContinueTime = 2f; public float DashContinueTime { get { return 2f; } }
        [SerializeField] private float dashDelayTime = 2f; public float DashDelayTime { get { return dashDelayTime; } } 
        [SerializeField] private float fullChargeTime = 1f; public float FullChargeTime { get { return fullChargeTime; } } 

        // Movement
        [Header("Movement")]
        [SerializeField] private float dashMoveSpeed = 7f; public float DashMoveSpeed { get { return dashMoveSpeed; } } // 이동 속도가 즉시 변경된다 해서 그냥 변수로 보관함
        [SerializeField] private float initialMoveSpeed = 4f; public float InitialMoveSpeed { get { return initialMoveSpeed; } }
        [SerializeField] private float patrolRange = 5f; public float PatrolRange { get { return patrolRange; } }

        // Interaction
        [Header("Damage")]
        [SerializeField] private int stingRayDamageAmount = 5;
        private void Awake()
        {
            damageAmount = stingRayDamageAmount;
        }
    }
}