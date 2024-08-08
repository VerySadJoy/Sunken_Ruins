using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public class ThrowingCrabCircleDetection : MonoBehaviour
    {
        // Detection Event <--> EnemyManager
        //public event EventHandler<PlayerDetectionEventArgs> OnPlayerDetection;

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
            EventManager.TriggerEvent(EventType.ThrowingCrabThrowRock, new Dictionary<string, object>{ { "Player", other.gameObject.transform } });
        }
    }
}