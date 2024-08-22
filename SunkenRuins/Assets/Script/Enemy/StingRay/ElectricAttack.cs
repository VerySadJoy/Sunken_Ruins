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
        private static int totalObjectNumber = 0;
        [SerializeField] private int thisObjectNumber; 
        public int ThisObjectNumber { get { return thisObjectNumber; } }  

        private const string playerLayerString = "Player";
        [SerializeField] private GameObject attackSpriteObject;
        [SerializeField] private GameObject attackRangeObject;
        public float showSpriteTime = 3f;
        private bool isAttack = false;

        private void Start()
        {
            thisObjectNumber = ++totalObjectNumber;
        }

        public void ShowAttackRange()
        {
            // Debug.Log("전기 가오리 공격 범위");
            attackRangeObject.SetActive(true);
        }

        public void HideAttackRange()
        {
            // Debug.Log("전기 가오리 공격 범위");
            attackRangeObject.SetActive(false);
        }

        public void Attack()
        {
            Debug.Log("플레이어 공격 시도함");
            StartCoroutine(ShowSpriteCoroutine(attackSpriteObject));
            
        }

        private IEnumerator ShowSpriteCoroutine(GameObject gameObject)
        {
            gameObject.SetActive(isAttack = true); // spriteObject 스크립트를 만들어서 따로 알아서 처리하게 하기?
            yield return new WaitForSeconds(showSpriteTime);
            gameObject.SetActive(isAttack = false);            
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (isAttack && other.gameObject.layer == LayerMask.NameToLayer(playerLayerString))
            {
                isAttack = false; // OnTriggerStay가 여러 번 call되는 것을 방지함
                
                EventManager.TriggerEvent(EventType.StingRayParalyze, new Dictionary<string, object> { { "Enemy", ThisObjectNumber }});
            }
        }
    }
}