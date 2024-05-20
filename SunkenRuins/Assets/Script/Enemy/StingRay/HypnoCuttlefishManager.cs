using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SunkenRuins
{
    public class HypnoCuttlefishManager : MonoBehaviour
    {
        [SerializeField] private HypnoCuttleFishCircleDetection hypnoCuttleFishCircleDetection;

        // 변수
        public float detectRadius = 10f;
        public float attackRadius = 5f;
        public float engulfTime = 3f;
        public float hypnotizeDelayTime = 5f;
        public float attackCoolTime = 1f;
        public int damagePerAttack = 10;
        public int totalKeyAmount = 10;

        // Player reference
        private GameObject player;

        // State 변수
        private bool canAttack = true;
        private bool isEngulfing = false;
        private float engulfTimer = 0f;
        private int keyPressCount = 0;

        private void Start()
        {
            hypnoCuttleFishCircleDetection.OnPlayerDetection += OnPlayerDetection_StartEngulf;
        }

        private void OnPlayerDetection_StartEngulf(object sender, PlayerDetectionEventArgs e)
        {
            player = e._player.gameObject;
            if(canAttack) transform.position = player.transform.position + Vector3.left; // 플레이어 앞으로

            StartEngulf();
        }

        private void Update()
        {
            if (isEngulfing)
            {
                EngulfPlayer();
            }
        }

        private void StartEngulf()
        {
            isEngulfing = true;
            engulfTimer = 0f;
            player.GetComponent<Rigidbody2D>().isKinematic = true;
            Debug.Log(player.GetComponent<Rigidbody2D>().isKinematic);
        }

        private void EngulfPlayer()
        {
            engulfTimer += Time.deltaTime;
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (engulfTimer < engulfTime)
            {
                Vector3 direction = (transform.position - player.transform.position).normalized;
                player.transform.position += direction * Time.deltaTime;

                if (distanceToPlayer <= attackRadius)
                {
                    AttackPlayer();
                }
            }
            else
            {
                StopEngulf();
            }
        }

        private void AttackPlayer()
        { 
            // 이걸 어디서 처리하는게 맞을까영
            int playerHealth = player.GetComponent<PlayerStat>().playerCurrentHealth;
            if (playerHealth != null)
            {
                playerHealth -= damagePerAttack;
                Debug.Log("데미지를 입음");
                player.GetComponent<Rigidbody2D>().isKinematic = false;
                Invoke(nameof(ResetAttack), hypnotizeDelayTime);
            }
            if (Input.anyKeyDown)
            {
                keyPressCount++;
            }

            if (keyPressCount >= totalKeyAmount)
            {
                transform.position = player.transform.position + Vector3.left * 3f;
                StopEngulf();
            }
        }

        private void StopEngulf()
        {
            player.GetComponent<Rigidbody2D>().isKinematic = false;
            isEngulfing = false;
            keyPressCount = 0;
            canAttack = false;
            Invoke(nameof(ResetAttack), hypnotizeDelayTime);
        }

        private void ResetAttack()
        {
            canAttack = true;
        }
    }
}