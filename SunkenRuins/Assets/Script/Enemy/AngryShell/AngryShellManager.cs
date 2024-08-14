using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public class AngryShellManager : EnemyManager
    {
        // 변??
        [SerializeField] private ShellStat shellStat;
        [SerializeField] private ShellCircleDetection shellCircleDetection;
        [SerializeField] private ShellAttackDetection shellAttackDetection;
        private bool isAbsorbingPlayer { get { return player != null; } }
        // private bool isEscape { get { return keyPressCount >= totalKeyAmount; } }

        // State 변??
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
                if (isEngulfing) // ?�켜???��?지�?�????�으�?
                {
                    // TODO:
                    // Close Shell Animation
                }
                else // 그�? 빨아?�이??중이?�면
                {                    
                    if (timer > shellStat.EngulfTime)
                    {
                        // 빨아?�이�?중단
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
            // ?�?�머 ?�시??
            timer = 0f;
        }

        private void OnPlayerDetection_AttackPlayer(Dictionary<string, object> message)
        {
            EventManager.TriggerEvent(EventType.PlayerDamaged, new Dictionary<string, object> { { "amount", shellStat.DamagePerAttack } });
            
            // 공격?�는가?
            isEngulfing = true;

            // ?�?�머 ?�시??
            timer = 0f;
        }

        private void OnPlayerEscape_ReleasePlayer(Dictionary<string, object> message)
        {
            // 공격 ?�정 ?�간?�안 중�?
            // + 모든 �?초기??
            StartCoroutine(StopEngulfingCoroutine()); // 공격받�? ?�고 ?�출?�을 ?�만 발동

            // ?�?�머 ?�시??
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
                timer = 0f; // ?�간 ?�시 초기??
                EventManager.TriggerEvent(EventType.PlayerDamaged, new Dictionary<string, object> { { "amount", shellStat.DamagePerAttack } });

                // TODO:
                // 공격 모션
            }

            if (Input.anyKeyDown)
            {
                // TODO:
                // ?�스?�로 ?�른 ???�수 ?�시

                if (++keyPressCount >= shellStat.TotalKeyAmount)
                {
                    player.GetComponent<PlayerManager>().SetInputEnabled(true); // 부?�트 ?�시 가?�함
                    StartCoroutine(StopEngulfingCoroutine());
                }
            }
        }

        // private void StopEngulf()
        // {
        //     Debug.Log("?�릭 많이 ?�서 공격 �???);

        //     isEngulfing = false;
        //     keyPressCount = 0;
        //     canAttack = false;
        //     Invoke(nameof(ResetAttack), engulfDelayTime); // ???�간 ?�안 공격 금�?
        // }

        // private void ResetAttack()
        // {
        //     canAttack = true;
        // }
    }

}