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
        // private bool isEscape { get { return keyPressCount >= totalKeyAmount; } }

        // State 변수
        private bool canAttack = true;
        private bool isEngulfing = false;
        private int keyPressCount = 0;

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
            if (canAttack && isAbsorbingPlayer)
            {
                if (isEngulfing) // 삼켜서 데미지를 줄 수 있으면
                {
                    // TODO:
                    // Close Shell Animation
                }
                else // 그저 빨아들이는 중이라면
                {                    
                    if (timer > shellStat.EngulfTime)
                    {
                        // 빨아들이기 중단
                        StartCoroutine(StopEngulfingCoroutine());

                        // TODO:
                        // Idle Animation
                    }
                }

                timer += Time.deltaTime;
            }
        }

        private void OnPlayerDetection_AbsorbPlayer(Dictionary<string, object> message)
        {
            Debug.LogError("조개가 플레이어를 빨아들임");

            // 타이머 재시작
            timer = 0f;
        }

        private void OnPlayerDetection_AttackPlayer(Dictionary<string, object> message)
        {
            Debug.LogError("조개가 플레이어를 공격함");
            EventManager.TriggerEvent(EventType.PlayerDamaged, new Dictionary<string, object> { { "amount", shellStat.DamagePerAttack } });
            
            // 공격하는가?
            isEngulfing = true;

            // 타이머 재시작
            timer = 0f;
        }

        private void OnPlayerEscape_ReleasePlayer(Dictionary<string, object> message)
        {
            Debug.LogError("플레이어가 조개한테서 벗어남");

            // 공격 일정 시간동안 중지
            // + 모든 값 초기화
            StartCoroutine(StopEngulfingCoroutine()); // 공격받지 않고 탈출했을 때만 발동

            // 타이머 재시작
            timer = 0f;
        }

        private IEnumerator StopEngulfingCoroutine()
        {
            keyPressCount = 0;
            isEngulfing = false;
            canAttack = false;
            EventManager.TriggerEvent(EventType.ShellEscape, null);

            yield return new WaitForSeconds(shellStat.EngulfDelayTime);
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

        // private void StopEngulf()
        // {
        //     Debug.Log("클릭 많이 해서 공격 못 함");

        //     isEngulfing = false;
        //     keyPressCount = 0;
        //     canAttack = false;
        //     Invoke(nameof(ResetAttack), engulfDelayTime); // 이 시간 동안 공격 금지
        // }

        // private void ResetAttack()
        // {
        //     canAttack = true;
        // }
    }

}