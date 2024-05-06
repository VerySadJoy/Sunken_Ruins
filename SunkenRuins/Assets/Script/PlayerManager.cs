using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins {
    public class PlayerManager : MonoBehaviour {
        [Header("Move")]
        [SerializeField] private float maxMoveSpeed = 1.5f; // 이동속도
        [SerializeField] private float turnAcceleration = 60f;
        [SerializeField] private float moveAcceleration = 30f;
        [SerializeField] private float moveDecceleration = 50f;

        //Component
        private Rigidbody2D rb;

        private PlayerControl playerControl; // Input System
        private bool isFacingRight = true;

        private void Awake() {
            playerControl = new PlayerControl();
            playerControl.Player.Enable();

            rb = GetComponent<Rigidbody2D>();
        }

        public void SetInputEnabled(bool enable) { // 컷신이나 뭐할때 Input 죽이는 용
            if (enable) {
                playerControl.Player.Enable();
            }
            else {
                playerControl.Player.Disable();
            }
        }

        private void HandleMoveInput() {
            Vector2 moveInput = playerControl.Player.Move.ReadValue<Vector2>();
            // 원하는 속도를 계산
            float desiredVelocityX = maxMoveSpeed * moveInput.x * maxMoveSpeed;
            float desiredVelocityY = maxMoveSpeed * moveInput.y * maxMoveSpeed;

            // 방향 전환 여부에 따라 다른 가속도 사용
            float accelerationX = ChooseAcceleration(moveInput.x, desiredVelocityX);
            float accelerationY = ChooseAcceleration(moveInput.y, desiredVelocityY);

            // x축 속도가 원하는 속도에 부드럽게 도달하도록 보간
            float updatedVelocityX = Mathf.MoveTowards(rb.velocity.x, desiredVelocityX, accelerationX * Time.deltaTime);
            float updatedVelocityY = Mathf.MoveTowards(rb.velocity.y, desiredVelocityY, accelerationY * Time.deltaTime);
            rb.velocity = new Vector2(updatedVelocityX, updatedVelocityY);
        }

        private void FixedUpdate() {
            HandleMoveInput();
        }

        private void UpdateFacingDirection(float moveInput) {
            if (moveInput != 0f) {
                isFacingRight = moveInput > 0f;
            }
        }

        private float ChooseAcceleration(float moveInput, float desiredVelocityX) {
            // Case 1) 이동을 멈추는 경우
            bool isStopping = moveInput == 0f;
            if (isStopping) {
                return moveDecceleration;
            }

            // Case 2) 반대 방향으로 이동하려는 경우
            bool isTurningDirection = rb.velocity.x * desiredVelocityX < 0f;
            if (isTurningDirection) {
                return turnAcceleration;
            }
            // Case 3) 기존 방향으로 계속 이동하는 경우
            return moveAcceleration;
        }
    }
}

