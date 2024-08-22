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
        private Vector3 initialPosition;
        private float lerpAmount;
        [SerializeField] private float distanceFromPlayer = 3.0f;
        [Header("Boost")]
        private int boostCount = 0;
        public float boostTime = 1f;
        public float boostVelocity = 5f;
        public int maxBoostAmount = 3;
        private Vector2 detectionVelocity;
        public float decelerationDuration = 1f;

        // State 변수
        // private bool isEscape { get { return keyPressCount >= totalKeyAmount; } }
        private bool canAttack = true;
        private bool isHypnotize = false;
        private bool isRetreat = false;
        private int keyPressCount = 0;
        private Animator animator;
        private void Awake() {
            animator = GetComponent<Animator>();
        }

        protected override void Start()
        {
            base.Start();
            initialPosition = transform.position;
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

        private void FixedUpdate()
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
            rb.velocity = (initialPosition - transform.position).normalized * retreatSpeed;
            UpdateFacingDirection(initialPosition.x > transform.position.x ? Vector2.right : Vector2.left);
            while (Vector3.Distance(transform.position, initialPosition) > 0.1f)
            {
                yield return null; // Wait for the next frame
            }
            rb.velocity = Vector3.zero;
            isRetreat = false;
        }

        private IEnumerator StopHypnotizeCoroutine()
        {
            // EventManager.TriggerEvent(EventType.ShellEscape, null);
            timer = 0f;
            keyPressCount = 0;
            canAttack = false;
            isHypnotize = false;
            //player = null;

            yield return new WaitForSeconds(hypnotizeDelayTime);
            canAttack = true;
        }
        
        private void AttackPlayer()
        {
            animator.SetTrigger("Attack");
            UpdateFacingDirection(player.transform.position.x > this.transform.position.x ? Vector2.right : Vector2.left);
            //player.gameObject.GetComponent<PlayerStat>().Damage(damagePerAttack);
            Debug.Log("갑오징어가 플레이어를 공격함");
            StartCoroutine(StopHypnotizeCoroutine());

            // TODO:
            // 공격 모션
        }

        private void MoveToPlayer()
        {
            // Calculate the direction towards the player
            Vector2 directionToPlayer = (player.position - transform.position).normalized;

            // Set the initial velocity towards the player
            detectionVelocity = directionToPlayer * boostVelocity * directionToPlayer.magnitude;

            // Apply the velocity to the Rigidbody2D
            rb.velocity = detectionVelocity;
            if (directionToPlayer.magnitude < 0.1f) {
                rb.velocity = Vector3.zero;
            }
            // Start the deceleration coroutine
            //StartCoroutine(DecelerateVelocity());
        }

        private IEnumerator DecelerateVelocity()
        {
            float elapsedTime = 0f;

            while (elapsedTime < decelerationDuration)
            {
                // Gradually reduce the velocity over time
                rb.velocity = Vector2.Lerp(detectionVelocity, Vector2.zero, elapsedTime / decelerationDuration);

                // Increment the elapsed time
                elapsedTime += Time.deltaTime;

                // Wait for the next frame
                yield return null;
            }

            // Ensure the velocity is set to zero at the end
            rb.velocity = Vector2.zero;

            Debug.Log("Movement stopped.");
        }

        private void OnPlayerDetection_Hypnotize(Dictionary<string, object> message)
        {
            if (!canAttack) return;

            // 최면시키는 동시에 플레이어 앞으로 이동
            isHypnotize = true;

            // 쫒아갈 플레이어 reference 받기
            player = (Transform)message["Player"];
        }

        private void SquidAnimation () {
            if (isRetreat) {
                return;
            }
            StartCoroutine(ApplyImpulse());
        }
        private IEnumerator ApplyImpulse() {

            float ccibal = 0f;
            Vector3 impulseVelocity = (boostCount % (2 * maxBoostAmount) < maxBoostAmount ? 1 : -1) * Vector3.left * boostVelocity;
            boostCount++;
            while (ccibal < boostTime)
            {
                rb.velocity = Vector3.Lerp(impulseVelocity, Vector3.zero, ccibal);
                ccibal += Time.deltaTime;
                yield return null;
            }
            rb.velocity = Vector3.zero;
            
        }
        private void ChangeDirection() {
            UpdateFacingDirection(boostCount % (2 * maxBoostAmount) < maxBoostAmount ? Vector3.right : Vector3.left);
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