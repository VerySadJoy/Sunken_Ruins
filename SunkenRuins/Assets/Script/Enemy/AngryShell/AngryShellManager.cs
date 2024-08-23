using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public class AngryShellManager : EnemyManager
    {
        [SerializeField] private ShellStat shellStat;
        [SerializeField] private ShellCircleDetection shellCircleDetection;
        [SerializeField] private ShellAttackDetection shellAttackDetection;
        private Animator animator;
        
        private int keyPressCount = 0; private float engulfTimer = 0f;
        private bool isAttack = false; private bool isInCoroutine = false;

        int circleDetectID; int attackDetectID;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            shellCircleDetection = GetComponentInChildren<ShellCircleDetection>();
        }

        protected override void Start()
        {
            base.Start();
            
            circleDetectID = shellCircleDetection.GetInstanceID();
            attackDetectID = shellAttackDetection.GetInstanceID();
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.ShellAbsorb, OnPlayerDetection_AbsorbPlayer);
            EventManager.StartListening(EventType.ShellSwallow, PrepareAttack);
            EventManager.StartListening(EventType.ShellEscape, OnPlayerEscape_ReleasePlayer);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.ShellAbsorb, OnPlayerDetection_AbsorbPlayer);
            EventManager.StopListening(EventType.ShellSwallow, PrepareAttack);
            EventManager.StopListening(EventType.ShellEscape, OnPlayerEscape_ReleasePlayer);
        }

        private IEnumerator StartTimer()
        {
            if (engulfTimer >= 0f) yield return null;

            engulfTimer = 0f;
            while (engulfTimer <= shellStat.EngulfTime) {
                engulfTimer += Time.deltaTime;
                yield return null;
            }

            if (!shellAttackDetection.IsShellAttack) 
            {
                engulfTimer = 0f;
                EventManager.TriggerEvent(EventType.ShellEscape, new Dictionary<string, object>(){ { "ObjectID", circleDetectID } });
                StartCoroutine(StopEngulfingCoroutine());
            }
            else 
            {
                engulfTimer = 0f;
                yield return null;
            }
        }

        private void OnPlayerDetection_AbsorbPlayer(Dictionary<string, object> message)
        {
            if (circleDetectID == (int)message["ObjectID"])
            {
                animator.SetBool("isIdle", false);
                animator.SetBool("Absorb", true);
                StartCoroutine(StartTimer());
            }
        }

        private void OnPlayerEscape_ReleasePlayer(Dictionary<string, object> message)
        {
            if (circleDetectID == (int)message["ObjectID"]) StartCoroutine(StopEngulfingCoroutine());
        }

        private IEnumerator StopEngulfingCoroutine()
        {
            if (isInCoroutine) yield return null;

            animator.SetBool("Absorb", false);
            animator.SetBool("isIdle", true); // Idle Animation
            isInCoroutine = true;
            shellCircleDetection.turnColliderOff();
            
            yield return new WaitForSeconds(shellStat.EngulfDelayTime);

            isInCoroutine = false;
            shellCircleDetection.turnColliderOn();
            shellAttackDetection.turnColliderOn();
            yield return null;
        }

        private void Update()
        {
            if (isAttack)
            {
                timer += Time.deltaTime;
                if (timer >= shellStat.AttackCoolTime)
                {
                    keyPressCount = 0;
                    timer = 0f; isAttack = false;
                    EventManager.TriggerEvent(EventType.ShellEscape, new Dictionary<string, object>(){ { "ObjectID", circleDetectID } });
                    EventManager.TriggerEvent(EventType.PlayerDamaged, new Dictionary<string, object> { { "amount", shellStat.DamagePerAttack } });
                    StartCoroutine(StopEngulfingCoroutine());
                }

                else if (Input.anyKeyDown)
                {
                    if (++keyPressCount >= shellStat.TotalKeyAmount)
                    {
                        keyPressCount = 0;
                        timer = 0f; isAttack = false;
                        EventManager.TriggerEvent(EventType.ShellEscape, new Dictionary<string, object>(){ { "ObjectID", circleDetectID } });
                        StartCoroutine(StopEngulfingCoroutine());
                    }
                }
            }
        }

        private void PrepareAttack(Dictionary<string, object> message)
        {
            if (attackDetectID == (int)message["ObjectID"])
            {
                animator.SetBool("Absorb", false); // Close Animation
                shellAttackDetection.turnColliderOff();
                isAttack = true;
            }
        }
    }
}