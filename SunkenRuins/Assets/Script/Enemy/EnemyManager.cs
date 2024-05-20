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

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            enemyStat = GetComponent<EnemyStat>();
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

        protected void UpdateFacingDirection(float moveDirectionX) {
            if (moveDirectionX != 0f) {
                isFacingRight = moveDirectionX > 0f;
            }
            transform.rotation = isFacingRight? Quaternion.Euler(Vector3.zero) : Quaternion.Euler(Vector3.up * 180f); // flipX인데 collider도 같이 뒤집도록 rotate
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 몬스터가 플레이어한테 데미지 입는 코드
            // 필요없으면 삭제하면 될 듯?
        }
    }
}