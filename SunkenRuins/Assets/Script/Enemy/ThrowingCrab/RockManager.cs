using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public class RockManager : MonoBehaviour
    {
        private Vector2 velocity;
        private PlayerManager player;
        private Rigidbody2D rb;
        private new CircleCollider2D collider2D;
        [SerializeField] private float speed = 5f;
        private float lifetime = 5f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            collider2D = GetComponent<CircleCollider2D>();
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
            Vector2 direction = ((Vector2)player.transform.position - rb.position).normalized;
            velocity = direction * speed;
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            rb.velocity = velocity;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log(LayerMask.LayerToName(collision.gameObject.layer));

            if (LayerMask.LayerToName(collision.gameObject.layer) == "Player" || LayerMask.LayerToName(collision.gameObject.layer) == "Wall")
            {
                Destroy(gameObject);
            }
        }
    }
}