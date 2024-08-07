using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SunkenRuins
{
    public class ThrowingCrabManager : EnemyManager
    {
        // 변수
        private Vector2 startPosition;

        //Component
        public ThrowingCrabStat throwingCrabStat;

        // State 변수
        // private bool isEscape { get { return keyPressCount >= totalKeyAmount; } }
        private bool canAttack = true;
        private bool isHypnotize = false;
        private bool isRetreat = false;
        private int keyPressCount = 0;

        private void Awake() {
            throwingCrabStat = GetComponent<ThrowingCrabStat>();
        }

        protected override void Start()
        {
            base.Start();
            startPosition = transform.position;
            // retreatSpeed = (오브젝트 길이) * time.deltatime / (이동할 시간) <== 상의 필요
        }

        private void Update()
        {
            PerformPatrolMovement();
        }

        private void PerformPatrolMovement()
        {
            float offsetFromInitialPosition = transform.position.x - startPosition.x;

            // 순찰 경계를 넘어서면 방향 전환
            if (offsetFromInitialPosition < -throwingCrabStat.patrolRange)
            {
                // Collider도 같이 뒤집어야 해서 각도 회전하는 게 맞는 듯!
                // 방향 전환하기
                UpdateFacingDirection(Vector2.right); // collider도 맞추어서 회전

                // spriteRenderer.flipX = false;
            }
            else if (offsetFromInitialPosition > throwingCrabStat.patrolRange)
            {
                UpdateFacingDirection(Vector2.left); // collider도 맞추어서 회전
            }

            // 속도 설정
            rb.velocity = new Vector2(throwingCrabStat.initialMoveSpeed * (isFacingRight ? 1f : -1f), 0);
        }
    }
}