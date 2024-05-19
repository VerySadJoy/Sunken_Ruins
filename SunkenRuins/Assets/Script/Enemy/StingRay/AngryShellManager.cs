using UnityEngine;

namespace SunkenRuins {
    public class AngryShellManager : MonoBehaviour {
        // 변수
        public float detectRadius = 10f;
        public float attackRadius = 5f;
        public float engulfTime = 3f;
        public float engulfDelayTime = 5f;
        public float attackCoolTime = 1f;
        public int damagePerAttack = 10;
        public int totalKeyAmount = 10;

        // Player reference
        private GameObject player;

        // State 변수
        private bool canAttack = true;
        private bool isEngulfing = false;
        private float engulfTimer = 0f;
        private int keyPressCount = 0;

        private void Update() {
            if (player == null) {
                player = GameObject.FindGameObjectWithTag("Player"); //나중에 공격범위 설정좀
                if (player == null) {
                    return;
                }
            }

            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= detectRadius && canAttack) {
                StartEngulf();
            }

            if (isEngulfing) {
                EngulfPlayer();
            }
        }

        private void StartEngulf() {
            isEngulfing = true;
            engulfTimer = 0f;
        }

        private void EngulfPlayer() {
            engulfTimer += Time.deltaTime;
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (engulfTimer < engulfTime) {
                Vector3 direction = (transform.position - player.transform.position).normalized;
                player.transform.position += direction * Time.deltaTime;

                if (distanceToPlayer <= attackRadius) {
                    AttackPlayer();
                }
            }
            else {
                StopEngulf();
            }
        }

        private void AttackPlayer() { // 이걸 어디서 처리하는게 맞을까영
            int playerHealth = player.GetComponent<PlayerStat>().playerCurrentHealth;
            if (playerHealth != null) {
                playerHealth -= damagePerAttack;
                Debug.Log("이따이");
            }
            if (Input.anyKeyDown) {
                keyPressCount++;
            }

            if (keyPressCount >= totalKeyAmount) {
                StopEngulf();
            }
        }

        private void StopEngulf() {
            isEngulfing = false;
            keyPressCount = 0;
            canAttack = false;
            Invoke(nameof(ResetAttack), engulfDelayTime);
        }

        private void ResetAttack() {
            canAttack = true;
        }
    }

}