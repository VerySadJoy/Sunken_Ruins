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
        [SerializeField] private LayerMask wallLayer;
        private HypnoCuttleFishCircleDetection circleDetection;
        private HypnoCuttleFishStat hypnoCuttleFishStat;

        [Header("Boost")]
        private int boostCount = 0;
        public float boostTime = 1f;
        public float boostVelocity = 3f;
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
            hypnoCuttleFishStat = GetComponent<HypnoCuttleFishStat>();
            currentState = HypnoCuttlefishState.Idle;
        }

        protected override void Start()
        {
            base.Start();
            initialPosition = transform.position;
            circleDetection = GetComponentInChildren<HypnoCuttleFishCircleDetection>();
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.HypnoCuttleFishHypnotize, OnPlayerDetection_Hypnotize);
            EventManager.StartListening(EventType.HypnoCuttleFishEscape, null);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.HypnoCuttleFishHypnotize, OnPlayerDetection_Hypnotize);
            EventManager.StopListening(EventType.HypnoCuttleFishEscape, null);
        }

        private void FixedUpdate()
        {
            Debug.Log(currentState);

            switch (currentState)
            {
                case HypnoCuttlefishState.Idle:
                    if (Physics2D.Raycast(transform.position + (isFacingRight ? 2.5f : -2.5f) * Vector3.right, Vector3.down, 0.6f, wallLayer))
                    {
                        UpdateFacingDirection(isFacingRight ? Vector3.left : Vector3.right);
                    }
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
            MoveToPlayer();

            if (Input.anyKeyDown)
            {
                if (++keyPressCount >= hypnotizeEscapeKeyNum)
                {
                    keyPressCount = 0;
                    currentState = HypnoCuttlefishState.Retreating;
                    EventManager.TriggerEvent(EventType.HypnoCuttleFishEscape, null);
                }
            }
        }

        private void MoveToPlayer()
        {
            Vector2 directionToPlayer;
            if (player.GetComponent<PlayerManager>().IsFacingRight)
            {
                directionToPlayer = (player.position - transform.position + Vector3.right).normalized;
            }
            else directionToPlayer = (player.position - transform.position + Vector3.left).normalized;

            detectionVelocity = directionToPlayer * boostVelocity; // * directionToPlayer.magnitude;
            rb.velocity = detectionVelocity;
            // UpdateFacingDirection(-directionToPlayer);

            if ((player.position - transform.position).magnitude < 4.5f)
            {
                rb.velocity = Vector3.zero;
                currentState = HypnoCuttlefishState.Attacking;
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

            StartCoroutine(colliderDelayCoroutine());
        }

        private IEnumerator colliderDelayCoroutine()
        {
            yield return new WaitForSeconds(hypnotizeDelayTime);
            circleDetection.turnColliderOn();
        }

        private void AttackPlayer()
        {
            if (currentState != HypnoCuttlefishState.Attacking) return;
            animator.SetTrigger("Attack");
            rb.velocity = Vector3.zero;

            StartCoroutine(AttackCoroutine());

        }

        private IEnumerator AttackCoroutine()
        {
            yield return new WaitForSeconds(attackTime);

            EventManager.TriggerEvent(EventType.HypnoCuttleFishEscape, null);
            EventManager.TriggerEvent(EventType.PlayerDamaged, new Dictionary<string, object>() {{"amount", hypnoCuttleFishStat.damageAmount }});
            StopAttack();
        }

        private void StopAttack() {
            animator.ResetTrigger("Attack");
            currentState = HypnoCuttlefishState.Retreating;
        }


        private void OnPlayerDetection_Hypnotize(Dictionary<string, object> message)
        {
            if (circleDetection.CircleDetectionID != (int)message["ObjectID"]) 
            {
                return;
            }
            else
            {
                player = (Transform)message["Player"];
                currentState = HypnoCuttlefishState.Hypnotizing;
                circleDetection.turnColliderOff();
            }
        }

        private void SquidAnimation()
        {
            if (currentState != HypnoCuttlefishState.Idle) return;
            StartCoroutine(ApplyImpulse());
        }

        private IEnumerator ApplyImpulse()
        {
            if (currentState != HypnoCuttlefishState.Idle) yield return null;

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
            if (currentState != HypnoCuttlefishState.Idle) return;
            UpdateFacingDirection(boostCount % (2 * maxBoostAmount) < maxBoostAmount ? Vector3.right : Vector3.left);
        }
        private void PlayHypnoSFX() {
            SFXManager.instance.PlaySFX(8);
        }
    }
}