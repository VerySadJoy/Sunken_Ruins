using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public class ShellCircleDetection : MonoBehaviour
    {
        // Components
        private CircleCollider2D circleCollider2D;
        [SerializeField] private float rayCastDistance = 5.0f;

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
            Debug.Log("call");
            if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerString))
            {
                // Player을 향하는 벡터 구하기
                Vector3 position = this.transform.position;
                temp = position;

                //RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, position, rayCastDistance, playerLayerMask);

                EventManager.TriggerEvent(EventType.ShellAbsorb, new Dictionary<string, object>() { { "position", position } });
                
            }
        }

        // Player 감지
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerString))
            {
                EventManager.TriggerEvent(EventType.ShellRelease, null);
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