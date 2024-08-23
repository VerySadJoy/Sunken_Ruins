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
        public TeamType teamType { get; set; }
        public int damageAmount { get; protected set;}

        protected void Start()
        {
            // teamType을 Monster로 해서 Damage 받을 때 구분 가능하게 함
            // 근데 솔직히 layer 쓸 거면 이렇게 enum 안 써도 될 듯?
            teamType = TeamType.Monster;
            monsterCurrentHealth = monsterMaxHealth;
        }

        public void Damage(int damageAmount)
        {
            // 공격받을 수 있나요?
        }
    }
}