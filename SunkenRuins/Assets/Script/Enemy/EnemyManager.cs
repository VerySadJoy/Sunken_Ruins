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

        protected void UpdateFacingDirection(Vector2 moveDirection) {
            if (moveDirection.x != 0f) {
                isFacingRight = moveDirection.x > 0f;
            }
            transform.rotation = isFacingRight? Quaternion.Euler(Vector3.zero) : Quaternion.Euler(Vector3.up * 180f); // flipX?�데 collider??같이 ?�집?�록 rotate
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 몬스?��? ?�레?�어?�테 ?��?지 ?�는 코드
            // ?�요?�으�???��?�면 ????
        }
    }
}