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
        [SerializeField] private HypnoFishCircleDetection hypnoFishCircleDetection;

        // 변수
        public float hypnotizeTime = 1f;
        public float hypnotizeDelayTime = 5f;
        public float attackTime = 1f;
        public int damagePerAttack = 10;
        public int totalKeyAmount = 10;
        public float retreatSpeed = 4f;
        public bool isHypnotizePlayer { get { return player != null; } }
        private float retreatTime = 1.5f;
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
            hypnoFishCircleDetection.OnPlayerDetection += OnPlayerDetection_HypnotizePlayer;
        }

        private void Update()
        {
            if (canAttack && isHypnotizePlayer)
            {
                lerpAmount += Time.deltaTime / hypnotizeTime;
                transform.position = Vector2.Lerp(startPosition, player.position + distanceFromPlayer * Vector3.left, lerpAmount);

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
                        if (++keyPressCount >= totalKeyAmount)
                        {
                            Debug.Log("연타 잘해서 탈출함");
                            isRetreat = true; // 후퇴한다
                            StartCoroutine(StopHypnotizeCoroutine());
                        }
                        Debug.Log(keyPressCount);
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
            player?.GetComponent<PlayerManager>().EscapeFromEnemy();
            player?.GetComponent<PlayerManager>().SetInputEnabled(true); // 부스트 다시 가능함
            timer = 0f;
            keyPressCount = 0;
            canAttack = false;
            player = null;

            yield return new WaitForSeconds(hypnotizeDelayTime);
            canAttack = true;
        }

        private void OnPlayerDetection_HypnotizePlayer(object sender, PlayerDetectionEventArgs e)
        {
            Debug.LogError("갑오징어가 플레이어를 최면시킴");

            // EventArgs e에 플레이어 매니저 클래스를 받는다
            player = e._player;

            // 부스트 못하게 막기
            player.GetComponent<PlayerManager>().SetInputEnabled(false);

            // 타이머 재시작
            timer = 0f;
        }

        private void AttackPlayer()
        {
            player.GetComponent<PlayerStat>().Damage(damagePerAttack);
            Debug.LogError("갑오징어가 플레이어를 공격함");
            StartCoroutine(StopHypnotizeCoroutine());

            // TODO:
            // 공격 모션
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