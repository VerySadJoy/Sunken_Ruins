using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;

namespace SunkenRuins
{
    public class ElectricAttack : MonoBehaviour
    {
        private const string playerLayerString = "Player";

        public void Attack()
        {
            Debug.Log("플레이어 공격 시도함");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerString))
            {
                Debug.LogWarning("플레이어가 피격당함");
            }
        }
    }
}