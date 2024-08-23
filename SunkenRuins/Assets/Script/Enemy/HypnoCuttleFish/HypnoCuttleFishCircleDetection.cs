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
        private int circleDetectionID; public int CircleDetectionID { get { return circleDetectionID; } }
        [SerializeField] private float rayCastDistance = 6.0f;

        // LayerMasks
        [SerializeField]
        private LayerMask playerLayerMask;
        private const string playerLayerString = "Player";

        private void Awake()
        {
            circleCollider2D = GetComponent<CircleCollider2D>();
            circleDetectionID = this.GetInstanceID();
        }

        // Player 감지
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerString))
            {
                EventManager.TriggerEvent(EventType.HypnoCuttleFishHypnotize, new Dictionary<string, object>{ { "Player", other.gameObject.transform }, {"ObjectID", this.GetInstanceID()} });
            }
        }

        public void turnColliderOn()
        {
            circleCollider2D.enabled = true;
        }

        public void turnColliderOff()
        {
            circleCollider2D.enabled = false;
        }

        // 플레이어를 향한 Vector를 선으로 표현
        // Vector2 temp;
        // void OnDrawGizmos()
        // {
        //     Gizmos.DrawLine(transform.position, (Vector2)transform.position + temp * rayCastDistance);
        // }
    }
}