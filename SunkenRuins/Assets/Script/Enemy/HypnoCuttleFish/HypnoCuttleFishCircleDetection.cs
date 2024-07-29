using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SunkenRuins
{
    public class HypnoCuttleFishCircleDetection : MonoBehaviour
    {
        // Components
        private CircleCollider2D circleCollider2D;
        [SerializeField] private float rayCastDistance = 6.0f;

        // LayerMasks
        [SerializeField]
        private LayerMask playerLayerMask;
        private const string playerLayerString = "Player";

        private void Awake()
        {
            circleCollider2D = GetComponent<CircleCollider2D>();
        }

        // Player 감지
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerString))
            {
                // Player을 향하는 벡터 구하기
                Vector2 dirToPlayerNormalized = (other.gameObject.transform.position - transform.position).normalized;
                temp = dirToPlayerNormalized;

                RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, dirToPlayerNormalized, rayCastDistance, playerLayerMask);
                if (raycastHit2D)
                {
                    EventManager.TriggerEvent(EventType.HypnoCuttleFishHypnotize, new Dictionary<string, object>{ { "Player", other.gameObject.transform } });
                }
            }
        }

        // 플레이어를 향한 Vector를 선으로 표현
        Vector2 temp;
        void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + temp * rayCastDistance);
        }
    }
}