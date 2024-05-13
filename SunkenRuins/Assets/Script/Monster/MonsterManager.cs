using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{

    public class MonsterManager : MonoBehaviour
    {
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private MonsterStat monsterStat;
        private bool isFacingRight = true;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            monsterStat = GetComponent<MonsterStat>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 몬스터가 플레이어한테 데미지 입는 코드
            // 필요없으면 삭제하면 될 듯?
        }
    }
}