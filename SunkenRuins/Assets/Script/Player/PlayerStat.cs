using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneTemplate;
using UnityEngine;

namespace SunkenRuins {
    public class PlayerStat : MonoBehaviour, IDamageable, IParalyzeable {
        [Header("Stat")]
        public int playerMaxHealth;
        public int playerCurrentHealth;
        public float playerMaxEnergy;
        public float playerCurrentEnergy;
        public int healthDecreaseRate;
        public float energyDecreaseRate = 20f;
        public TeamType teamType { get; set; }
        [SerializeField] private int invincibleTime = 1; // Invincibility
        [SerializeField] private float paralyzeTime = 2f;
        [SerializeField] private float hypnotizeTime = 2f; public float HypnotizeTime { get { return hypnotizeTime; } }

        [Header("Movement")]
        public float moveSpeed = 5f;
        public float defaultMoveSpeed = 5f; //부스트 미사용 최고 이동속도
        public float paralyzeMoveSpeed = 2f;
        public float hypnotizeMoveSpeed = 2f;
        public float turnAcceleration = 60f;
        public float moveAcceleration = 30f;
        public float moveDecceleration = 50f;
        public float normalBoostSpeed = 10f;
        public float perfectBoostSpeed = 15f;
        public float absorbSpeed = 6f;

        //Bool
        private bool isInvincible = false; public bool IsInvincible { get { return isInvincible;} }
        private bool isParalyzed = false; public bool IsParalyzed { get {return isParalyzed;} }
        private bool canUseEnergy = true; public bool CanUseEnergy {get {return canUseEnergy;} }

        private void Start() {
            teamType = TeamType.Player;
            playerCurrentHealth = playerMaxHealth;
            playerCurrentEnergy = playerMaxEnergy;
            StartCoroutine(DecreaseHealthOverTime());
        }

        private void FixedUpdate()
        {
            Debug.Log($"현재 체력: {playerCurrentHealth} / {playerMaxHealth}");
        }

        void OnEnable()
        {
            EventManager.StartListening(EventType.StingRayParalyze, Paralyze);
            EventManager.StartListening(EventType.HypnoCuttleFishHypnotize, Hypnotize);
            EventManager.StartListening(EventType.PlayerDamaged, Damage); 
        }

        void OnDisable()
        {
            EventManager.StopListening(EventType.StingRayParalyze, Paralyze);
            EventManager.StopListening(EventType.HypnoCuttleFishHypnotize, Hypnotize);
            EventManager.StopListening(EventType.PlayerDamaged, Damage);
        }

        private IEnumerator DecreaseHealthOverTime() {
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
            StartCoroutine(BeInvincibleOverInvincibleTime(invincibleTime));
        }

        private IEnumerator BeInvincibleOverInvincibleTime(int invincibleTime) {
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

        public void Paralyze(Dictionary<string, object> message)
        {
            // Debug.Log("플레이어 마비당함");
            StartCoroutine(ParalyzeSpeedCoroutine());

            // TODO:
            // 1. Paralyze Animation 재생 --> Idle Animation으로 변환
            // 2. Paralyze SFX 재생
        }

        private IEnumerator ParalyzeSpeedCoroutine()
        {
            moveSpeed = paralyzeMoveSpeed; // 마비되어 느린 속도로 이동
            canUseEnergy = false; // 부스트 사용 불가
            yield return new WaitForSeconds(paralyzeTime);

            moveSpeed = defaultMoveSpeed; // 본래 속도로 복귀
            canUseEnergy = true; // paralyzeTime 이후 부스트 사용 가능
        }

        public void Hypnotize(Dictionary<string, object> message)
        {
            // Debug.Log("플레이어 최면당함");
            StartCoroutine(HypnotizeSpeedCoroutine());
        }

        private IEnumerator HypnotizeSpeedCoroutine()
        {
            moveSpeed = hypnotizeMoveSpeed; // 기본 이동 속도를 잠시 변수에 보관
            canUseEnergy = false; // 부스트 사용 불가
            yield return new WaitForSeconds(hypnotizeTime);

            moveSpeed = defaultMoveSpeed; // 본래 속도로 복귀
            canUseEnergy = true;
        }

        private void Damage(Dictionary<string, object> message)
        {
            Damage((int)message["amount"]);
        }

        public void Damage(int damageAmount)
        {
            Debug.Log("체력 손실");
            playerCurrentHealth -= damageAmount;
            playerCurrentHealth = Mathf.Clamp(playerCurrentHealth, 0, playerMaxHealth);

            // 무적이 된다
            BeInvincible(invincibleTime); // 2초 동안 하드코딩
        }

    }
}
