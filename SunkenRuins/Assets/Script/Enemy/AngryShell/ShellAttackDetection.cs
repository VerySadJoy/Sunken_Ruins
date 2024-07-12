using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public class ShellAttackDetection : MonoBehaviour
    {
        // Components
        private BoxCollider2D boxCollider2D;
        [SerializeField] private float rayCastDistance = 7.0f;

        // LayerMasks
        [SerializeField]
        private LayerMask playerLayerMask;
        private const string playerLayerString = "Player";

        private void Awake()
        {
            boxCollider2D = GetComponent<BoxCollider2D>();
        }

        // Player 감지
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerString))
            {
                Debug.Log("네모: 플레이어 감지!");
                EventManager.TriggerEvent(EventType.ShellAttack, new Dictionary<string, object>() { { "Player", other.gameObject.transform } });
            }
        }
    }
}