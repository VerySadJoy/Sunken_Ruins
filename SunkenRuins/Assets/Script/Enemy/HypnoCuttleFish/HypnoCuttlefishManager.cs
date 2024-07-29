using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SunkenRuins
{
    public class HypnoCuttlefishManager : EnemyManager
    {
        // 변수
        public float hypnotizeTime = 1.5f;
        public float hypnotizeDelayTime = 5f;
        public float attackTime = 1f;
        public int damagePerAttack = 10;
        public int hypnotizeEscapeKeyNum = 10;
        public float retreatSpeed = 4f;
        // public bool isHypnotizePlayer { get { return player != null; } }
        private float retreatTime = 3f;
        private Vector2 startPosition;
        private float lerpAmount;
        [SerializeField] private float distanceFromPlayer = 3.0f;

        // State 변수
        // private bool isEscape { get { return keyPressCount >= totalKeyAmount; } }
        private bool canAttack = true;
        private bool isHypnotize = false;
        private bool isRetreat = false;
        private int keyPressCount = 0;

        protected override void Start()
        {
            base.Start();
            startPosition = transform.position;
            // retreatSpeed = (오브젝트 길이) * time.deltatime / (이동할 시간) <== 상의 필요
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.HypnoCuttleFishHypnotize, OnPlayerDetection_Hypnotize);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.HypnoCuttleFishHypnotize, OnPlayerDetection_Hypnotize);
        }

        private void Update()
        {
            if (canAttack && isHypnotize)
            {
                MoveToPlayer();

                if (timer > hypnotizeTime) // 최면해서 데미지를 줄 수 있으면
                {
                    // TODO:
                    // Hypnotize Damage Animation
                    AttackPlayer();
                }
                else // 최면만 하고 기다리는 중이면
                { 
                    if (Input.anyKeyDown)
                    {
                        if (++keyPressCount >= hypnotizeEscapeKeyNum)
                        {
                            Debug.Log("연타 잘해서 탈출함");
                            isRetreat = true; // 후퇴한다
                            StartCoroutine(StopHypnotizeCoroutine());
                        }
                    }

                    // TODO:
                    // 텍스트로 누른 키 횟수 표시
                }

                timer += Time.deltaTime;
            }
            else if (isRetreat)
            {
                StartCoroutine(retreatCoroutine());
            }
        }

        private IEnumerator retreatCoroutine()
        {
            rb.velocity = retreatSpeed * Vector2.left;
            yield return new WaitForSeconds(retreatTime); // 일단 하드코딩으로 1초 후퇴한다고 설정함
            rb.velocity = Vector2.zero;
            isRetreat = false;
            // rb.constraints = RigidbodyConstraints2D.FreezeAll; // 일단 하드코딩으로 아예 못 움직이게 만듦
        }

        private IEnumerator StopHypnotizeCoroutine()
        {
            // EventManager.TriggerEvent(EventType.ShellEscape, null);
            timer = 0f;
            keyPressCount = 0;
            canAttack = false; isHypnotize = false;
            player = null;

            yield return new WaitForSeconds(hypnotizeDelayTime);
            canAttack = true;
        }
        
        private void AttackPlayer()
        {
            Debug.LogError("갑오징어가 플레이어를 공격함");
            EventManager.TriggerEvent(EventType.PlayerDamaged, new Dictionary<string, object> { { "amount", damagePerAttack } });
            StartCoroutine(StopHypnotizeCoroutine());

            // TODO:
            // 공격 모션
        }

        private void MoveToPlayer()
        {
            lerpAmount += Time.deltaTime / hypnotizeTime;
            transform.position = Vector2.Lerp(startPosition, player.position + distanceFromPlayer * Vector3.left, lerpAmount);
        }

        private void OnPlayerDetection_Hypnotize(Dictionary<string, object> message)
        {
            if (!canAttack) return;

            // 최면시키는 동시에 플레이어 앞으로 이동
            isHypnotize = true;

            // 쫒아갈 플레이어 reference 받기
            player = (Transform)message["Player"];
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