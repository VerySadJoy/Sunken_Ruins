using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

namespace SunkenRuins
{
    public class StingRayManager : EnemyManager
    {
        // 삼각형 감지 범위랑 동그라미 감지 범위 컴포넌트로 받기
        [SerializeField] private TriangleDetection triangleDetection;
        [SerializeField] private CircleDetection circleDetection;
        [SerializeField] private ElectricAttack electricAttack;

        // Component
        private bool isChasingPlayer { get { return player != null; } }
        private bool isDashDelayTime = false; // 이게 곧 canAttack이다!
        private bool isPrepareAttack = false;
        [SerializeField] private float lerpAmount = 0.05f;
        private Vector3 initialPosition;
        [SerializeField] private StingRayStat stingRayStat;
        private BoxCollider2D boxCollider2D;
        private Animator animator;

        // Used for StingRay Attack CoolTime
        private Time attackTime;
        private void Awake() {
            boxCollider2D = GetComponent<BoxCollider2D>();
            animator = GetComponent<Animator>();
        }

        protected override void Start()
        {
            base.Start();
            initialPosition = transform.position;

        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.StingRayMoveTowardsPlayer, OnPlayerDetection_MoveTowardsPlayer);
            EventManager.StartListening(EventType.StingRayPrepareAttack, OnPlayerDetection_PrepareAttack);
            EventManager.StartListening(EventType.StingRayParalyze, OnPlayerDetection_AttackPlayer);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.StingRayMoveTowardsPlayer, OnPlayerDetection_MoveTowardsPlayer);
            EventManager.StopListening(EventType.StingRayPrepareAttack, OnPlayerDetection_PrepareAttack);
            EventManager.StopListening(EventType.StingRayParalyze, OnPlayerDetection_AttackPlayer);
        }

        private void Update()
        {
            if (isChasingPlayer)
            {
                Vector2 dirToPlayerNormalized = (player.position - transform.position).normalized; // 플레이어를 향한 단위 벡터
                UpdateFacingDirection(dirToPlayerNormalized); // 왼쪽 오른쪽 바라보는 방향 설정

                // 속도를 줄여 공격해야 한다면
                if (isPrepareAttack)
                {
                    // 플레이어한테 서서히 움직이는 모션 (그러다 속도 = 0이 됨)
                    rb.velocity = Vector2.zero;//dirToPlayerNormalized * stingRayStat.dashMoveSpeed * Mathf.Pow(0.3f, timer);
                    
                    // 움직이면서 공격 범위를 보여줌
                    electricAttack.ShowAttackRange();

                    // 속도를 다 줄이고 공격이 준비되었을 때
                    if (timer > stingRayStat.fullChargeTime)
                    {
                        isPrepareAttack = false;
                        electricAttack.HideAttackRange();
                        electricAttack.Attack(); // 공격 실시!!!

                        // fullChargeTime이 지났을 때
                        StartCoroutine(returnDuringDashDelayTime(dirToPlayerNormalized));
                    }
                }
                else // 그저 쫓아가는 것이면
                {
                    //animator.SetBool("Warning", false);
                    rb.velocity = dirToPlayerNormalized * stingRayStat.dashMoveSpeed; // 대시 속도로 변경

                    // 추격에 주어진 시간이 다하면
                    if (timer > stingRayStat.dashContinueTime)
                    {
                        // 플레이어와 반대방향으로 이동함
                        StartCoroutine(returnDuringDashDelayTime(dirToPlayerNormalized));
                    }
                }

                timer += Time.deltaTime; // 타이머 시간 증가
            }
            else if (!isDashDelayTime)
            {
                PerformPatrolMovement();
            }
        }

        private IEnumerator returnDuringDashDelayTime(Vector2 dirToPlayerNormalized)
        {
            isDashDelayTime = true;
            player = null; // 플레이어 초기화해서 플레이어 추적 불가

            yield return new WaitForSeconds(electricAttack.showSpriteTime); // 잠깐 멈춘다 (공격 모션 등의 이유)
            rb.velocity = (initialPosition - transform.position).normalized * stingRayStat.initialMoveSpeed;
            UpdateFacingDirection(initialPosition.x > transform.position.x ? Vector2.right : Vector2.left);
            while (Vector3.Distance(transform.position, initialPosition) > 0.1f)
            {
                yield return null; // Wait for the next frame
            }
            // yield return new WaitForSeconds(stingRayStat.dashDelayTime);

            isDashDelayTime = false; // 지금부터 patrol movement을 할 수 있다
            initialPosition = transform.position; // 새로운 위치에서 patrol movement 실시
            rb.velocity = Vector3.zero;
        }

        private void PerformPatrolMovement()
        {
            float offsetFromInitialPosition = transform.position.x - initialPosition.x;
            if (offsetFromInitialPosition < -stingRayStat.patrolRange)
            {
                UpdateFacingDirection(Vector2.right);
            }
            else if (offsetFromInitialPosition > stingRayStat.patrolRange)
            {
                UpdateFacingDirection(Vector2.left);
            }
            // UpdateFacingDirection(offsetFromInitialPosition < -stingRayStat.patrolRange ? Vector2.right : Vector2.left);
            // 속도 설정
            rb.velocity = new Vector2(stingRayStat.initialMoveSpeed * (isFacingRight ? 1f : -1f), 0);
        }

        private void OnPlayerDetection_MoveTowardsPlayer(Dictionary<string, object> message)
        {
            if (isChasingPlayer || (int)message["Enemy"] != triangleDetection.ThisObjectNumber) 
            {
                return;
            }

            // EventArgs e에 플레이어 매니저 클래스를 받는다
            player = (Transform)message["Player"];

            // 타이머 재시작
            timer = 0f;
        }

        private void OnPlayerDetection_PrepareAttack(Dictionary<string, object> message)
        {
            if (isPrepareAttack || (int)message["Enemy"] != circleDetection.ThisObjectNumber) 
            {
                return; // 몹 중간에 플레이어가 있을 때 발생하는 버그 수정
            }
            animator.SetTrigger("Warning");
            // EventArgs e에 플레이어 매니저 클래스를 받는다
            player = (Transform)message["Player"];

            // 공격 flag 변수
            isPrepareAttack = true;

            // 타이머 재시작
            timer = 0f;
        }

        private void OnPlayerDetection_AttackPlayer(Dictionary<string, object> message)
        {
            if ((int)message["Enemy"] != electricAttack.ThisObjectNumber) return;

            EventManager.TriggerEvent(EventType.PlayerDamaged, new Dictionary<string, object> { { "amount", stingRayStat.damageAmount } });
        }

        private void OnTriggerEnter2D (Collider2D collision) {
            if (collision != boxCollider2D) {
                return;
            }
            if (LayerMask.LayerToName(collision.gameObject.layer) == "Wall") {

            }
        }


    }
}