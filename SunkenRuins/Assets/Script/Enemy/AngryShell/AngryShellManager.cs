using System;
using System.Collections;
using UnityEngine;

namespace SunkenRuins
{
    public class AngryShellManager : EnemyManager
    {
        // 변수
        public float detectRadius = 10f;
        public float attackRadius = 5f;
        public float engulfTime = 3f;
        public float engulfDelayTime = 5f;
        public float attackCoolTime = 1f;
        public int damagePerAttack = 10;
        public int totalKeyAmount = 10;
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
            shellCircleDetection.OnPlayerDetection += OnPlayerDetection_AbsorbPlayer;
            shellCircleDetection.OnPlayerEscape += OnPlayerEscape_ReleasePlayer;
            shellAttackDetection.OnPlayerDetection += OnPlayerDetection_AttackPlayer;
        }

        private void Update()
        {
            if (isAbsorbingPlayer)
            {
                if (isEngulfing) // 삼켜서 데미지를 줄 수 있으면
                {
                    // TODO:
                    // Close Shell Animation
                    AttackPlayer();
                }
                else // 그저 빨아들이는 중이라면
                {
                    Vector2 dirToPlayerNormalized = -(player.position - transform.position).normalized; // 일단 "-"로 하드코딩
                    player.GetComponent<PlayerManager>().GetAbsorbed(dirToPlayerNormalized);
                    
                    if (timer > engulfTime)
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

        private IEnumerator StopEngulfingCoroutine()
        {
            keyPressCount = 0;
            isEngulfing = false;
            canAttack = false;
            player?.GetComponent<PlayerManager>().EscapeFromShell();
            player = null;

            yield return new WaitForSeconds(engulfDelayTime);
            canAttack = true;
        }

        private void OnPlayerDetection_AbsorbPlayer(object sender, PlayerDetectionEventArgs e)
        {
            Debug.LogError("조개가 플레이어를 빨아들임");

            // EventArgs e에 플레이어 매니저 클래스를 받는다
            player = e._player;

            // 타이머 재시작
            timer = 0f;
        }

        private void OnPlayerDetection_AttackPlayer(object sender, PlayerDetectionEventArgs e)
        {
            Debug.LogError("조개가 플레이어를 공격함");

            // EventArgs e에 플레이어 매니저 클래스를 받는다
            player = e._player;

            // 공격하는가?
            isEngulfing = true;

            // 타이머 재시작
            timer = 0f;
        }

        private void OnPlayerEscape_ReleasePlayer(object sender, PlayerDetectionEventArgs e)
        {
            Debug.LogError("플레이어가 조개한테서 벗어남");

            // 공격 일정 시간동안 중지
            // + 모든 값 초기화
            StartCoroutine(StopEngulfingCoroutine()); // 공격받지 않고 탈출했을 때만 발동

            // 타이머 재시작
            timer = 0f;
        }

        private void AttackPlayer()
        {
            // 부스트 못하게 막기
            player.GetComponent<PlayerManager>().SetInputEnabled(false);
            if (timer >= attackCoolTime)
            {
                timer = 0f; // 시간 다시 초기화
                player.GetComponent<PlayerStat>().Damage(damagePerAttack);
                Debug.LogError("조개가 플레이어를 공격함");

                // TODO:
                // 공격 모션
            }

            if (Input.anyKeyDown)
            {
                // TODO:
                // 텍스트로 누른 키 횟수 표시

                if (++keyPressCount >= totalKeyAmount)
                {
                    Debug.Log("연타 잘해서 탈출함");
                    player.GetComponent<PlayerManager>().SetInputEnabled(true); // 부스트 다시 가능함
                    StartCoroutine(StopEngulfingCoroutine());
                }
                Debug.Log(keyPressCount);
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