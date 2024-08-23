using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
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

        public void turnColliderOff()
        {
            boxCollider2D.enabled = false;
        }

        public void turnColliderOn()
        {
            boxCollider2D.enabled = true;
        }

        // Player 감지
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerString))
            {
                EventManager.TriggerEvent(EventType.ShellSwallow, new Dictionary<string, object>() { {"shellPos", transform.position + Vector3.up }, { "ObjectID", this.GetInstanceID() } });
                StartCoroutine(shellAttackCoroutine());
            }
        }

        private IEnumerator shellAttackCoroutine()
        {
            isShellAttack = true;
            yield return new WaitForSeconds(7f);
            isShellAttack = false;
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