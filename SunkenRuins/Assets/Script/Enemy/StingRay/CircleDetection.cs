using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public class CircleDetection : MonoBehaviour
    {
        private static int totalObjectNumber = 0;
        [SerializeField] private int thisObjectNumber; 
        public int ThisObjectNumber { get { return thisObjectNumber; } }  

        // Components
        private CircleCollider2D circleCollider2D;
        [SerializeField] private float rayCastDistance = 4.0f;

        // LayerMasks
        [SerializeField]
        private LayerMask playerLayerMask;
        private const string playerLayerString = "Player";

        private void Awake()
        {
            circleCollider2D = GetComponent<CircleCollider2D>();
        }

        private void Start()
        {
            thisObjectNumber = ++totalObjectNumber;
        }

        // 이런 식으로 카메라 범위 밖일 때 collider 끄는 게 성능적 측면에서 좋지 않을까?
        // 근데 방법은 잘 모르겠음...
        private void OnBecameInvisible()
        {
            circleCollider2D.enabled = false;
        }

        private void OnBecameVisible()
        {
            circleCollider2D.enabled = true;
        }

        // Player 감지
        // Enter 쓰려고 했는데 아이템에 가로막히면 오류가 나더라고
        // 아이템을 먹어서 가오리가 플레이어를 직시하고 있어도 인식을 못해 Enter함수라서...
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerString))
            {
                // Player을 향하는 벡터 구하기
                Vector2 dirToPlayerNormalized = (other.gameObject.transform.position - transform.position).normalized;

                RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, dirToPlayerNormalized, rayCastDistance, playerLayerMask);
                if (raycastHit2D)
                {
                    Debug.Log("원: 플레이어 감지!");
                    EventManager.TriggerEvent(EventType.StingRayPrepareAttack, new Dictionary<string, object>{ { "Player", other.gameObject.transform }, {"Enemy", ThisObjectNumber } });
                }
            }
        }
    }
}