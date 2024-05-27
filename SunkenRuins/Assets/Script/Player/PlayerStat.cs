using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins {
    public class PlayerStat : MonoBehaviour, IDamageable {
        [Header("Stat")]
        public int playerMaxHealth;
        public int playerCurrentHealth;
        public float playerMaxEnergy;
        public float playerCurrentEnergy;
        public int healthDecreaseRate;
        public float energyDecreaseRate;
        public TeamType teamType { get; set; }
        [SerializeField] private int invincibleTime = 2; // Invincibility

        [Header("Movement")]
        public float initialMoveSpeed = 5f; //부스트 미사용 최고 이동속도
        public float boostMoveSpeed = 10f;
        public float turnAcceleration = 60f;
        public float moveAcceleration = 30f;
        public float moveDecceleration = 50f;
        public float normalBoostSpeed = 10f;
        public float perfectBoostSpeed = 15f;
        public float absorbSpeed = 3.5f;

        //Bool
        private bool isInvincible = false;

        private void Start() {
            teamType = TeamType.Player;
            playerCurrentHealth = playerMaxHealth;
            playerCurrentEnergy = playerMaxEnergy;
            StartCoroutine(DecreaseHealthOverTime());
        }

        private System.Collections.IEnumerator DecreaseHealthOverTime() {
            while (playerCurrentHealth > 0) {
                if (!isInvincible) {
                    yield return new WaitForSeconds(1f);
                    playerCurrentHealth -= healthDecreaseRate;
                    Debug.LogWarning("무적이 아닐 때 체력 꾸준히 감소");
                }
                else {
                    yield return null;
                }
            }
            Debug.Log("끼엑 사망");
            // TODO:
            // 1. 사망 모션
            // 2. 사망 UI
        }

        public void RestoreHealth(int healAmount){
            playerCurrentHealth += healAmount;
            Debug.Log("체력 회복");
            playerCurrentHealth = Mathf.Clamp(playerCurrentHealth, 0, playerMaxHealth);
            // TODO:
            // 1. 체력 회복 모션
            // 2. 체력 회복 UI
        }

        public void RestoreEnergy(float energyAmount){
            playerCurrentEnergy += energyAmount;
            Debug.Log("에너지 회복");
            playerCurrentEnergy = Mathf.Clamp(playerCurrentEnergy, 0, playerMaxEnergy);
            // TODO:
            // 1. 에너지 회복 모션
            // 2. 에너지 회복 UI
        }

        public void BeInvincible(int invincibleTime){
            isInvincible = true;
            StartCoroutine(beInvincibleOverInvincibleTime(invincibleTime));
        }

        private System.Collections.IEnumerator beInvincibleOverInvincibleTime(int invincibleTime) {
            // Player의 BoxCollider를 가져와서 끈다 <-- 이거 안 좋은 구조 같은데 의견 부탁해요...
            BoxCollider2D tempPlayerCollider = this.GetComponent<BoxCollider2D>();
            isInvincible = true;
            Debug.Log("무적 시작");
            tempPlayerCollider.enabled = false;
            yield return new WaitForSeconds(invincibleTime); // 인자로 받은 무적 시간이 끝나면

            // Player의 BoxCollider를 다시 켠다
            isInvincible = false;
            Debug.Log("무적 풀림");
            tempPlayerCollider.enabled = true;
            
            // TODO:
            // 1. 무적 모션, 효과
            // 2. 무적 UI
        }

        public void Damage(int damageAmount)
        {
            playerCurrentHealth -= damageAmount;
            Debug.Log("체력 손실");
            playerCurrentHealth = Mathf.Clamp(playerCurrentHealth, 0, playerMaxHealth);

            // 무적이 된다
            BeInvincible(invincibleTime); // 2초 동안 하드코딩

            // TODO:
            // 1. 데미지 모션, 효과
            // 2. 입고 무적 판정
        }
    }
}
