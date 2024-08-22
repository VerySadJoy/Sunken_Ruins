using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SunkenRuins
{
    public enum CrabState
    {
        Patrolling,
        Throwing,
        Returning
    }

    public class ThrowingCrabManager : EnemyManager
    {
        // Variables
        private Vector2 startPosition;

        // Components
        private ThrowingCrabStat throwingCrabStat;

        // Wall LayerMask
        [SerializeField] private LayerMask wallLayer;
        
        // State Management
        private CrabState currentState;
        [SerializeField] private GameObject rockPrefab;
        
        // Sprite Handling
        private SpriteRenderer crabSpriteRenderer;
        [SerializeField] private Sprite[] sprites;
        private int spriteIndex = 0;

        private void Awake()
        {
            throwingCrabStat = GetComponent<ThrowingCrabStat>();
            crabSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void Start()
        {
            base.Start();
            startPosition = transform.position;
            currentState = CrabState.Patrolling;
            StartCoroutine(ManageState());
        }

        private void Update(){
            if (!Physics2D.Raycast(transform.position + (isFacingRight ? 1.6f : -1.6f) * Vector3.right + 1.5f * Vector3.down, Vector3.down, 0.6f, wallLayer)
                || Physics2D.Raycast(transform.position + (isFacingRight ? 1.6f : -1.6f) * Vector3.right, Vector3.down, 0.6f, wallLayer))
            {
                UpdateFacingDirection(isFacingRight ? Vector3.left : Vector3.right);
            }

            if (currentState == CrabState.Patrolling)
            {
                PerformPatrolMovement();
            }
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.ThrowingCrabThrowRock, OnPlayerDetection_ThrowRock);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.ThrowingCrabThrowRock, OnPlayerDetection_ThrowRock);
        }

        private void OnPlayerDetection_ThrowRock(Dictionary<string, object> message)
        {
            if (currentState == CrabState.Patrolling && this.GetComponentInChildren<ThrowingCrabCircleDetection>().GetInstanceID() == (int)message["ObjectID"])
            {
                currentState = CrabState.Throwing;
                player = (Transform)message["Player"];
            }
        }

        private IEnumerator ManageState()
        {
            while (true)
            {
                switch (currentState)
                {
                    case CrabState.Patrolling:
                        PerformPatrolMovement();
                        yield return PatrolAnimation();
                        break;
                    case CrabState.Throwing:
                        yield return ThrowAnimation();
                        currentState = CrabState.Returning;
                        break;
                    case CrabState.Returning:
                        yield return BackToPatrol();
                        currentState = CrabState.Patrolling;
                        break;
                }
                yield return null;
            }
        }

        private IEnumerator PatrolAnimation()
        {
            while (currentState == CrabState.Patrolling)
            {
                crabSpriteRenderer.sprite = sprites[spriteIndex];
                yield return new WaitForSeconds(0.1f);
                spriteIndex = (spriteIndex + 1) % 8;
            }
        }

        private IEnumerator ThrowAnimation()
        {
            rb.velocity = Vector2.zero;
            for (int i = 7; i <= 11; i++)
            {
                crabSpriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(0.1f);
            }
            ThrowRock();
        }

        private void ThrowRock()
        {
            Instantiate(rockPrefab, new Vector2(rb.position.x, rb.position.y + 0.5f), Quaternion.identity);
        }

        private IEnumerator BackToPatrol()
        {
            yield return new WaitForSeconds(1f); // Adjust as needed
        }

        private void PerformPatrolMovement()
        {
            float offsetFromInitialPosition = transform.position.x - startPosition.x;
            if (offsetFromInitialPosition < -throwingCrabStat.patrolRange)
            {
                UpdateFacingDirection(Vector2.right);
            }
            else if (offsetFromInitialPosition > throwingCrabStat.patrolRange)
            {
                UpdateFacingDirection(Vector2.left);
            }

            rb.velocity = new Vector2(throwingCrabStat.initialMoveSpeed * (isFacingRight ? 1f : -1f), 0);
        }
    }
}
