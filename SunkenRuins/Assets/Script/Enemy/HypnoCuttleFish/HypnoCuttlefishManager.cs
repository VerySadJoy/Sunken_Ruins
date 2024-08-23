using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SunkenRuins
{
    public enum HypnoCuttlefishState
    {
        Idle,
        Hypnotizing,
        Attacking,
        Retreating,
    }
    public class HypnoCuttlefishManager : EnemyManager
    {
        public float hypnotizeTime = 1.5f;
        public float hypnotizeDelayTime = 5f;
        public float attackTime = 1f;
        public int damagePerAttack = 10;
        public int hypnotizeEscapeKeyNum = 10;
        public float retreatSpeed = 4f;
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

        private int keyPressCount = 0;
        private Animator animator;
        private HypnoCuttlefishState currentState;
        private float newTimer;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            currentState = HypnoCuttlefishState.Idle;
        }

        protected override void Start()
        {
            base.Start();
            initialPosition = transform.position;
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
            //Debug.Log(currentState);
            switch (currentState)
            {
                case HypnoCuttlefishState.Idle:
                    // Idle behavior (e.g., patrolling or waiting)
                    //SquidAnimation();
                    break;
                case HypnoCuttlefishState.Hypnotizing:
                    HandleHypnotizing();
                    break;
                case HypnoCuttlefishState.Attacking:
                    AttackPlayer();
                    break;
                case HypnoCuttlefishState.Retreating:
                    StartCoroutine(RetreatCoroutine());
                    break;
            }
        }

        private void HandleHypnotizing()
        {
            this.GetComponentInChildren<CircleCollider2D>().enabled = false;
            MoveToPlayer();

            if ((player.position - transform.position).magnitude < 0.1f)
            {
                currentState = HypnoCuttlefishState.Attacking;
            }
            else if (Input.anyKeyDown)
            {
                if (++keyPressCount >= hypnotizeEscapeKeyNum)
                {
                    currentState = HypnoCuttlefishState.Retreating;
                    StartCoroutine(StopHypnotizeCoroutine());
                }
            }
        }

        private IEnumerator RetreatCoroutine()
        {
            rb.velocity = (initialPosition - transform.position).normalized * retreatSpeed;
            UpdateFacingDirection(initialPosition.x <= this.transform.position.x ? Vector2.right : Vector2.left);

            while (Vector3.Distance(transform.position, initialPosition) > 0.1f)
            {
                yield return null;
            }

            rb.velocity = Vector3.zero;
            currentState = HypnoCuttlefishState.Idle;
            this.GetComponentInChildren<CircleCollider2D>().enabled = true;
        }

        private IEnumerator StopHypnotizeCoroutine()
        {
            //yield return new WaitForSeconds(1.25f);
            newTimer = 0f;
            keyPressCount = 0;
            currentState = HypnoCuttlefishState.Retreating;
            yield return null;
        }

        private void AttackPlayer()
        {
            if (currentState != HypnoCuttlefishState.Attacking) return;
            animator.SetTrigger("Attack");
            rb.velocity = Vector3.zero;
            //StartCoroutine(StopHypnotizeCoroutine());
        }
        private void StopAttack() {
            animator.ResetTrigger("Attack");
            StartCoroutine(StopHypnotizeCoroutine());
        }

        private void MoveToPlayer()
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            detectionVelocity = directionToPlayer * boostVelocity * directionToPlayer.magnitude;
            rb.velocity = detectionVelocity;
            UpdateFacingDirection(-directionToPlayer);
            if ((player.position - transform.position).magnitude < 0.1f)
            {
                rb.velocity = Vector3.zero;
            }
        }

        private void OnPlayerDetection_Hypnotize(Dictionary<string, object> message)
        {
            if (currentState != HypnoCuttlefishState.Idle) return;

            //isHypnotize = true;
            currentState = HypnoCuttlefishState.Hypnotizing;
            player = (Transform)message["Player"];
        }

        private void SquidAnimation()
        {
            if (currentState != HypnoCuttlefishState.Idle) return;
            StartCoroutine(ApplyImpulse());
        }

        private IEnumerator ApplyImpulse()
        {
            float elapsedTime = 0f;
            Vector3 impulseVelocity = (boostCount % (2 * maxBoostAmount) < maxBoostAmount ? 1 : -1) * Vector3.left * boostVelocity;
            boostCount++;

            while (elapsedTime < boostTime)
            {
                rb.velocity = Vector3.Lerp(impulseVelocity, Vector3.zero, elapsedTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rb.velocity = Vector3.zero;
        }

        private void ChangeDirection()
        {
            if (currentState == HypnoCuttlefishState.Retreating) return;
            UpdateFacingDirection(boostCount % (2 * maxBoostAmount) < maxBoostAmount ? Vector3.right : Vector3.left);
        }
    }
}