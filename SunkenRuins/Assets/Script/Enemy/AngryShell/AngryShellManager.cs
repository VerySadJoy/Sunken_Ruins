using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public class AngryShellManager : EnemyManager
    {
        // 변수
        [SerializeField] private ShellStat shellStat;
        [SerializeField] private ShellCircleDetection shellCircleDetection;
        [SerializeField] private ShellAttackDetection shellAttackDetection;
        private bool isAbsorbingPlayer { get { return player != null; } }
        [SerializeField] private Sprite [] sprites;
        private SpriteRenderer shellSpriteRenderer;
        private int spriteIndex = 9;
        // private bool isEscape { get { return keyPressCount >= totalKeyAmount; } }

        // State 변수
        private bool canAttack = true;
        private bool isEngulfing = false;
        private int keyPressCount = 0;
        private void Awake() {
            shellSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void Start()
        {
            base.Start();
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.ShellAbsorb, OnPlayerDetection_AbsorbPlayer);
            EventManager.StartListening(EventType.ShellRelease, OnPlayerEscape_ReleasePlayer);
            EventManager.StartListening(EventType.ShellSwallow, PrepareAttack);          
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.ShellAbsorb, OnPlayerDetection_AbsorbPlayer);
            EventManager.StopListening(EventType.ShellRelease, OnPlayerEscape_ReleasePlayer);
            EventManager.StopListening(EventType.ShellSwallow, PrepareAttack); 
        }

        private void Update()
        {
            
            if (canAttack)
            {
                if (isEngulfing) // 삼켜서 데미지를 줄 수 있으면
                {
                    // TODO:
                    // Close Shell Animation
                }
                else // 그저 빨아들이는 중이라면
                {                    
                    
                }

                //timer += Time.deltaTime;
                
            }
            if (timer > shellStat.EngulfTime)
            {
                // 빨아들이기 중단
                StartCoroutine(StopEngulfingCoroutine());

                // TODO:
                // Idle Animation
                StartCoroutine(ShellClose());
                
            }
            //Debug.Log(timer);
        }
        private IEnumerator StartTimer(){
            while (timer > shellStat.EngulfTime) {
                timer += Time.deltaTime;
            }
            yield return null;
        }
        private IEnumerator ShellOpen(){
            for (int i = 0; i < 8; i++) {
                shellSpriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(0.1f);
            }
            StartCoroutine(ShellAbsorb());
            yield return null;
        }
        private IEnumerator ShellAbsorb() {
            while (timer <= shellStat.EngulfTime) {
                spriteRenderer.sprite = sprites[spriteIndex];
                spriteIndex++;
                if (spriteIndex > 14) {
                    spriteIndex = 9;
                }
                yield return new WaitForSeconds(0.15f);
            }
            timer = 0f;
            yield return null;
        }

        private IEnumerator ShellClose(){
            for (int i = 8; i >= 0; i--) {
                shellSpriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(0.1f);
            }
            yield return null;
        }

        private void OnPlayerDetection_AbsorbPlayer(Dictionary<string, object> message)
        {
            StartCoroutine(ShellOpen());
            StartCoroutine(StartTimer());
            // 타이머 재시작
            //timer = 0f;
        }

        private void OnPlayerDetection_AttackPlayer(Dictionary<string, object> message)
        {
            EventManager.TriggerEvent(EventType.PlayerDamaged, new Dictionary<string, object> { { "amount", shellStat.DamagePerAttack } });
            
            // 공격하는가?
            isEngulfing = true;

            // 타이머 재시작
            //timer = 0f;
        }

        private void OnPlayerEscape_ReleasePlayer(Dictionary<string, object> message)
        {

            // 공격 일정 시간동안 중지
            // + 모든 값 초기화
            StartCoroutine(StopEngulfingCoroutine()); // 공격받지 않고 탈출했을 때만 발동

            // 타이머 재시작
            timer = 0f;
        }

        private IEnumerator StopEngulfingCoroutine()
        {
            timer = 0f;
            keyPressCount = 0;
            isEngulfing = false;
            canAttack = false;
            EventManager.TriggerEvent(EventType.ShellEscape, null);
            shellCircleDetection.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            StartCoroutine(ShellClose());
            yield return new WaitForSeconds(shellStat.EngulfDelayTime);
            shellCircleDetection.gameObject.GetComponent<CircleCollider2D>().enabled = true;
            canAttack = true;
        }

        private void PrepareAttack(Dictionary<string, object> message)
        {
            if (timer >= shellStat.AttackCoolTime)
            {
                timer = 0f; // 시간 다시 초기화
                EventManager.TriggerEvent(EventType.PlayerDamaged, new Dictionary<string, object> { { "amount", shellStat.DamagePerAttack } });
                Debug.LogError("조개가 플레이어를 공격함");

                // TODO:
                // 공격 모션
            }

            if (Input.anyKeyDown)
            {
                // TODO:
                // 텍스트로 누른 키 횟수 표시

                if (++keyPressCount >= shellStat.TotalKeyAmount)
                {
                    Debug.Log("연타 잘해서 탈출함");
                    player.GetComponent<PlayerManager>().SetInputEnabled(true); // 부스트 다시 가능함
                    StartCoroutine(StopEngulfingCoroutine());
                }
            }
        }
    }

}