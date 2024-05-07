using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins {
    public class PlayerStat : MonoBehaviour {
        [Header("Stat")]
        public int playerMaxHealth;
        public int playerCurrentHealth;
        public float playerMaxEnergy;
        public float playerCurrentEnergy;
        public int healthDecreaseRate;
        public float energyDecreaseRate;

        [Header("Movement")]
        public float initialMoveSpeed = 5f; //부스트 미사용 최고 이동속도
        public float boostMoveSpeed = 10f;
        public float turnAcceleration = 60f;
        public float moveAcceleration = 30f;
        public float moveDecceleration = 50f;

        [Header("Player Manager")]
        [SerializeField] private PlayerManager playerManager;

        private void Start() {
            playerCurrentHealth = playerMaxHealth;
            playerCurrentEnergy = playerMaxEnergy;
            playerManager.OnItemInteractAction += playerManager_OnItemInteractAction;
            StartCoroutine(DecreaseHealthOverTime());
        }

        private void playerManager_OnItemInteractAction(object sender, EventArgs e)
        {
            // HealthPotion과 상호작용했을 때 <-- 이걸 EventArgs의 ItemSO로부터 가져온 enum ItemType으로 Switch를 써서 알 수 있나요?
            int healAmount = 0; // EventArgs로부터 healAmount를 받을 수 있나요?
            Heal(healAmount);
        }

        private System.Collections.IEnumerator DecreaseHealthOverTime() {
            while (playerCurrentHealth > 0) {
                yield return new WaitForSeconds(1f);
                playerCurrentHealth -= healthDecreaseRate;
            }
            Debug.Log("끼엑 사망");
            // TODO:
            // 1. 사망 모션
            // 2. 사망 UI
        }

        public void Heal(int healAmount){
            playerCurrentHealth += healAmount;
            Debug.Log("회복");
            // TODO:
            // 1. 회복 모션
            // 2. 회복 UI
        }
    }
}
