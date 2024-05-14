using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public class EnemyStat : MonoBehaviour, IDamageable
    {
        [Header("Stat")]
        public int monsterMaxHealth;
        public int monsterCurrentHealth;

        [Header("Movement")]
        public float objectMoveSpeed = 5f;
        // 일단 가오리Stat 따로 상속받아서 만들지는 않았어
        // 다른 애들 만들면서 생각해도 될 듯?
        public float dashMoveSpeed = 10f; // 이동 속도가 즉시 변경된다 해서 그냥 변수로 보관함
        public float turnAcceleration = 60f;
        public float moveAcceleration = 30f;
        public float moveDecceleration = 50f;
        public TeamType teamType { get; set; }

        [Header("Time")]
        public float dashContinueTime = 2f;

        private void Start()
        {
            // teamType을 Monster로 해서 Damage 받을 때 구분 가능하게 함
            // 근데 솔직히 layer 쓸 거면 이렇게 enum 안 써도 될 듯?
            teamType = TeamType.Monster;
            monsterCurrentHealth = monsterMaxHealth;
        }

        public void Damage(TeamType other)
        {
            if(other == TeamType.Player) Debug.Log("몬스터가 피해를 입습니다.");
        }
    }
}