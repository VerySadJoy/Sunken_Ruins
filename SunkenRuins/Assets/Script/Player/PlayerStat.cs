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

        [Header("Movement")]
        public float initialMoveSpeed = 5f; //부스트 미사용 최고 이동속도
        public float boostMoveSpeed = 10f;
        public float turnAcceleration = 60f;
        public float moveAcceleration = 30f;
        public float moveDecceleration = 50f;

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

        public void Damage(TeamType other)
        {
            if(other == TeamType.Monster) Debug.Log("플레이어가 피해를 입습니다.");

            // TODO:
            // 1. 데미지 모션, 효과
            // 2. 입고 무적 판정
            // 3. 데미지 입는 float OR int값 설정
        }
    }
}
