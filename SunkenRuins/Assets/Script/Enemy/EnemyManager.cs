using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SunkenRuins
{

    public class EnemyManager : MonoBehaviour
    {
        // Component
        protected Rigidbody2D rb;
        protected SpriteRenderer spriteRenderer;
        protected EnemyStat enemyStat;
        protected Transform player;
        protected float timer = 0f;
        protected bool isFacingRight = true;

        // 삼각형 감지 범위랑 동그라미 감지 범위 컴포넌트로 받기
        [SerializeField] private TriangleDetection triangleDetection;
        [SerializeField] private CircleDetection circleDetection;

        private int moveSpeed = 4;
        private float patrolRange = 5;
        private Vector3 initialPosition;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            enemyStat = GetComponent<EnemyStat>();

            // 각각의 Event에 함수를 할당할 거면 
            // Circle, TriangleDetection가 상속받는 Detection Class를 만드는 것도 괜찮지 않을까?
            triangleDetection.OnPlayerDetection += OnPlayerDetection_MoveTowardsPlayer;
            circleDetection.OnPlayerDetection += OnPlayerDetection_MoveTowardsPlayer;

            initialPosition = transform.position;
        }

        protected void OnPlayerDetection_MoveTowardsPlayer(object sender, PlayerDetectionEventArgs e)
        {
            Debug.Log("플레이어를 향해 이동 중");

            // EventArgs e에 플레이어 매니저 클래스를 받는다
            player = e._player;

            // 타이머 재시작
            // FixedUpdate에서 초기화하면 플레이어 인식할 때 렉 걸림
            timer = 0f;
        }

        protected virtual void FixedUpdate() {
            if (player != null) {
                Vector2 dirToPlayerNormalized = (player.position - transform.position).normalized; // 플레이어를 향한 단위 벡터
                UpdateFacingDirection(dirToPlayerNormalized.x); // 왼쪽 오른쪽 바라보는 방향 설정
                rb.velocity = dirToPlayerNormalized * enemyStat.dashMoveSpeed; // 대시 속도로 변경
                timer += Time.deltaTime; // 타이머 시간 증가
                Debug.Log("hi");
                if (timer >= enemyStat.dashContinueTime) {
                    Debug.Log("플레이어 추적 중단");
                    rb.velocity = Vector2.zero; // idle 상태를 속도 0으로 설정
                    
                    // 플레이어 초기화해서 if 전체가 실행되지 않게 함
                    player = null;
                }
            }
            else {
                PerformPatrolMovement();
            }
        }

        protected void UpdateFacingDirection(float moveDirectionX) {
            if (moveDirectionX != 0f) {
                isFacingRight = moveDirectionX > 0f;
            }
            transform.rotation = isFacingRight? Quaternion.Euler(Vector3.zero) : Quaternion.Euler(Vector3.forward * 180f); // flipX인데 collider도 같이 뒤집도록 rotate
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 몬스터가 플레이어한테 데미지 입는 코드
            // 필요없으면 삭제하면 될 듯?
        }

        private void PerformPatrolMovement() {
            float offsetFromInitialPosition = transform.position.x - initialPosition.x;
            
            // 왼쪽 순찰 경계를 넘어서면 오른쪽으로 방향 전환
            if (offsetFromInitialPosition < -patrolRange) {
                spriteRenderer.flipX = false;
            }
            // 오른쪽 순찰 경계를 넘어서면 왼쪽으로 방향 전환
            else if (offsetFromInitialPosition > patrolRange) {
                spriteRenderer.flipX = true;
            }

            // 바라보는 방향으로 속도 일정하게 유지
            rb.velocity = new Vector2(moveSpeed * (spriteRenderer.flipX ? -1f : 1f), rb.velocity.y);
        }
    }
}