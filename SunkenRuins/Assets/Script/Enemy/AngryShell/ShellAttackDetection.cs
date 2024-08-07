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

        private bool isShellAttack = false;
        public bool IsShellAttack { get { return isShellAttack; } }

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
                EventManager.TriggerEvent(EventType.ShellSwallow, new Dictionary<string, object>() { {"shellPos", transform.position } });
                // EventManager.TriggerEvent(EventType.ShellAttack, null);
            }
        }

        // public void ShellAttackEnable()
        // {
        //     isShellAttack = true;
        // }

        // public void ShellAttackDisable()
        // {
        //     isShellAttack = false;
        // }
    }
}