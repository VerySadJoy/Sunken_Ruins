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
        private bool isAbsorbingPlayer { get { return player != null; } }
        [SerializeField] private Sprite[] sprites;
        private SpriteRenderer shellSpriteRenderer;
        private int spriteIndex = 9;
        
        private bool canAttack = true;
        private bool isEngulfing = false;
        private int keyPressCount = 0;
        private float lastAbsorbTime = 0f; // Time since the last absorb

        private void Awake() {
            shellSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void Start()
        {
            base.Start();
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.ShellAbsorb, OnPlayerDetection_AbsorbPlayer);
            EventManager.StartListening(EventType.ShellRelease, OnPlayerEscape_ReleasePlayer);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.ShellAbsorb, OnPlayerDetection_AbsorbPlayer);
            EventManager.StopListening(EventType.ShellRelease, OnPlayerEscape_ReleasePlayer);
        }

        private void Update()
        {

        }

        private IEnumerator StartTimer()
        {
            while (timer <= shellStat.EngulfTime) {
                timer += Time.deltaTime;
            }
            yield return null;
        }

        private IEnumerator ShellOpen()
        {
            for (int i = 0; i < 8; i++) {
                shellSpriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(0.1f);
            }
            StartCoroutine(ShellAbsorb());
            yield return null;
        }

        private IEnumerator ShellAbsorb()
        {
            float sibal = 0f;
            while (sibal <= shellStat.EngulfTime) {
                shellSpriteRenderer.sprite = sprites[spriteIndex];
                spriteIndex++;
                if (spriteIndex > 13) {
                    spriteIndex = 9;
                }
                sibal += 0.15f;
                yield return new WaitForSeconds(0.15f);
            }
            StartCoroutine(StopEngulfingCoroutine());
            yield return null;
        }

        private IEnumerator ShellClose()
        {
            for (int i = 8; i >= 0; i--) {
                shellSpriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(0.1f);
            }
            yield return null;
        }

        private void OnPlayerDetection_AbsorbPlayer(Dictionary<string, object> message)
        {

            StartCoroutine(ShellOpen());
            StartCoroutine(StartTimer());
        }

        private void OnPlayerDetection_AttackPlayer(Dictionary<string, object> message)
        {
            EventManager.TriggerEvent(EventType.PlayerDamaged, new Dictionary<string, object> { { "amount", shellStat.DamagePerAttack } });
            isEngulfing = true;
        }

        private void OnPlayerEscape_ReleasePlayer(Dictionary<string, object> message)
        {
        }

        private IEnumerator StopEngulfingCoroutine()
        {
            keyPressCount = 0;
            isEngulfing = false;
            canAttack = false;
            EventManager.TriggerEvent(EventType.ShellEscape, null);
            shellCircleDetection.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            StartCoroutine(ShellClose());
            yield return new WaitForSeconds(shellStat.EngulfDelayTime);
            shellCircleDetection.gameObject.GetComponent<CircleCollider2D>().enabled = true;
            canAttack = true;
            yield return null;
        }

        private void PrepareAttack(Dictionary<string, object> message)
        {
            if (timer >= shellStat.AttackCoolTime)
            {
                timer = 0f;
                EventManager.TriggerEvent(EventType.PlayerDamaged, new Dictionary<string, object> { { "amount", shellStat.DamagePerAttack } });
            }

            if (Input.anyKeyDown)
            {
                if (++keyPressCount >= shellStat.TotalKeyAmount)
                {
                    player.GetComponent<PlayerManager>().SetInputEnabled(true);
                    StartCoroutine(StopEngulfingCoroutine());
                }
            }
        }
    }
}