using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public class TriangleDetection : MonoBehaviour
    {
        // Components
        private PolygonCollider2D polygonCollider2D;
        [SerializeField] private float rayCastDistance = 6.0f;
        Vector2 temp;

        // LayerMasks
        [SerializeField]
        private LayerMask playerLayerMask;
        private const string playerLayerString = "Player";

        private void Awake()
        {
            polygonCollider2D = GetComponent<PolygonCollider2D>();
        }

        // 이런 식으로 카메라 범위 밖일 때 collider 끄는 게 성능적 측면에서 좋지 않을까?
        // 근데 방법은 잘 모르겠음...
        private void OnBecameInvisible()
        {
            Debug.Log("카메라에서 안 보임");
            polygonCollider2D.enabled = false;
        }

        private void OnBecameVisible()
        {
            Debug.Log("카메라에서 보임");
            polygonCollider2D.enabled = true;
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
                temp = dirToPlayerNormalized;

                // RayCast해서 플레이어가 벽 같은 장애물에 가려져 있는지 확인
                // 이거 LayerMask.NameToLayer쓰면 플레이어 인식이 안 돼서 일단 layermask 따로 serializefield로 받아놨어
                // 오류 고치는 방법 있으면 알려줘...!
                RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, dirToPlayerNormalized, rayCastDistance, playerLayerMask);
                if (raycastHit2D)
                {
                    Debug.Log("삼각형: 플레이어 감지!");
                    EventManager.TriggerEvent(EventType.StingRayMoveTowardsPlayer, new Dictionary<string, object>{ { "Player", other.gameObject.transform } });
                }
            }
        }

        // 플레이어를 향한 Vector를 선으로 표현
        // void OnDrawGizmos()
        // {
        //     Gizmos.DrawLine(transform.position, (Vector2)transform.position + temp * rayCastDistance);
        // }
    }
}