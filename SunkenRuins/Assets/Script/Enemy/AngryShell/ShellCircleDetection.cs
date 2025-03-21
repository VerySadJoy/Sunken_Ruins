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

        public void turnColliderOff()
        {
            circleCollider2D.enabled = false;
        }

        public void turnColliderOn()
        {
            circleCollider2D.enabled = true;
        }

        // Player 감지
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerString))
            {
                // Player을 향하는 벡터 구하기
                Vector3 position = this.transform.position;

                EventManager.TriggerEvent(EventType.ShellAbsorb, new Dictionary<string, object>() { { "position", position + 1.5f * Vector3.up }, { "ObjectID", this.GetInstanceID() } });
            }
        }

        // Player 감지
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerString))
            {
                EventManager.TriggerEvent(EventType.ShellEscape, new Dictionary<string, object>() { { "ObjectID", this.GetInstanceID() } });
            }
        }

        // 플레이어를 향한 Vector를 선으로 표현
        // Vector2 temp;
        // void OnDrawGizmos()
        // {
        //     Gizmos.DrawLine(transform.position, (Vector2)transform.position + temp * rayCastDistance);
        // }
    }
}