using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace SunkenRuins
{
    public class StingRayManager : EnemyManager
    {

        // 삼각형 감지 범위랑 동그라미 감지 범위 컴포넌트로 받기
        [SerializeField] private TriangleDetection triangleDetection;
        [SerializeField] private CircleDetection circleDetection;

        // Component
        private bool isChasingPlayer { get { return player != null; } }
        private bool isDashDelayTime = false;
        private Vector3 initialPosition;
        [SerializeField] private StingRayStat stingRayStat;

        protected override void Start()
        {
            base.Start();
            triangleDetection.OnPlayerDetection += OnPlayerDetection_MoveTowardsPlayer;
            circleDetection.OnPlayerDetection += OnPlayerDetection_MoveTowardsPlayer;
            initialPosition = transform.position;
        }

        private void Update()
        {
            if (isChasingPlayer)
            {
                Vector2 dirToPlayerNormalized = (player.position - transform.position).normalized; // 플레이어를 향한 단위 벡터
                UpdateFacingDirection(dirToPlayerNormalized.x); // 왼쪽 오른쪽 바라보는 방향 설정
                rb.velocity = dirToPlayerNormalized * stingRayStat.dashMoveSpeed; // 대시 속도로 변경
                timer += Time.deltaTime; // 타이머 시간 증가

                // 추격에 주어진 시간이 다하면
                if (timer >= stingRayStat.dashContinueTime)
                {
                    Debug.Log("플레이어 추적 중단");

                    // 플레이어와 반대방향으로 이동함
                    StartCoroutine(returnDuringDashDelayTime(dirToPlayerNormalized));
                }
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
            rb.velocity = -dirToPlayerNormalized * stingRayStat.initialMoveSpeed;
            yield return new WaitForSeconds(stingRayStat.dashDelayTime);

            isDashDelayTime = false; // 지금부터 patrol movement을 할 수 있다
            initialPosition = transform.position; // 새로운 위치에서 patrol movement 실시
        }

        private void PerformPatrolMovement()
        {
            float offsetFromInitialPosition = transform.position.x - initialPosition.x;

            // 순찰 경계를 넘어서면 방향 전환
            if (offsetFromInitialPosition < -stingRayStat.patrolRange || offsetFromInitialPosition > stingRayStat.patrolRange)
            {
                // Collider도 같이 뒤집어야 해서 각도 회전하는 게 맞는 듯!

                // 방향 전환하기
                UpdateFacingDirection(-rb.velocity.x); // collider도 맞추어서 회전

                // spriteRenderer.flipX = false;
            }

            // 속도 설정
            rb.velocity = new Vector2(stingRayStat.initialMoveSpeed * (isFacingRight ? 1f : -1f), 0);
        }
    }
}