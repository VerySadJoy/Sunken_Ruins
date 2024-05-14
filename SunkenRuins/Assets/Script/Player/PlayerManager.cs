using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

namespace SunkenRuins {
    public class PlayerManager : MonoBehaviour {
        [Header("Follow Camera Target")]
        [SerializeField] private GameObject cameraFollowTarget;
        // 바라보는 방향으로 얼마나 앞에 있는 지점을 카메라가 추적할 것인지
        [SerializeField, Range(0f, 2f)] private float cameraLookAheadDistance = 1f;
        [SerializeField]private float zoomSpeed = 2f;

        //Component
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private PlayerStat playerStat;
        private CinemachineVirtualCamera virtualCamera;
        private float defaultOrthographicSize;
        private PlayerControl playerControl; // Input System
        private bool isFacingRight = true;

        // Layermask String
        private const string itemLayerString = "Item";
        private const string enemyLayerString = "Enemy";

        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            playerStat = GetComponent<PlayerStat>();
            virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            defaultOrthographicSize = virtualCamera.m_Lens.OrthographicSize;
        }

        private void OnTriggerEnter2D(Collider2D other) { // 임시 처리
            //소신발언
            //충돌로만 Item 획득이 가능하면 굳이 EventHandler 써야할 이유가 있는지?
            if (other.gameObject.layer == LayerMask.NameToLayer(itemLayerString)) { //Item 획득
                ItemSO itemSO = other.gameObject.GetComponent<Item>().GetItemSO();
                if (itemSO != null) {
                    switch (itemSO.itemType) {
                        case ItemType.HealthPotion:
                            playerStat.RestoreHealth(100);
                            break;
                        case ItemType.PowerBattery:
                            playerStat.RestoreEnergy(100f);
                            break;
                        case ItemType.BubbleShield:
                            playerStat.BeInvincible(2); // 일단은 하드코딩으로 invincibleTime 인자로 받음
                            break;
                        default:
                            Debug.LogError("이거 나오면 안됨");
                            break;
                    }
                }
                Destroy(other.gameObject); //아이템 삭제
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer(enemyLayerString))
            {
                EnemyStat monsterStat = other.gameObject.GetComponent<EnemyStat>();
                playerStat.Damage(monsterStat.teamType);
            }
        }

        private void OnEnable() {
            playerControl = new PlayerControl();
            playerControl.Player.Enable();
        }

        private void FixedUpdate() {
            HandleMoveInput();

            UpdateCameraFollowTarget();
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
            float desiredVelocityX = isBoosting(moveInput) ? playerStat.boostMoveSpeed * moveInput.x : playerStat.initialMoveSpeed * moveInput.x;
            float desiredVelocityY = isBoosting(moveInput) ? playerStat.boostMoveSpeed * moveInput.y : playerStat.initialMoveSpeed * moveInput.y;

            // 방향 전환 여부에 따라 다른 가속도 사용
            float accelerationX = ChooseAcceleration(moveInput.x, desiredVelocityX);
            float accelerationY = ChooseAcceleration(moveInput.y, desiredVelocityY);

            // x축 속도가 원하는 속도에 부드럽게 도달하도록 보간
            float updatedVelocityX = Mathf.MoveTowards(rb.velocity.x, desiredVelocityX, accelerationX * Time.deltaTime);
            float updatedVelocityY = Mathf.MoveTowards(rb.velocity.y, desiredVelocityY, accelerationY * Time.deltaTime);
            rb.velocity = new Vector2(updatedVelocityX, updatedVelocityY);
            HandleBoost(moveInput);
            UpdateFacingDirection(moveInput.x);
        }

        private void UpdateFacingDirection(float moveInputX) {
            if (moveInputX != 0f) {
                isFacingRight = moveInputX > 0f;
            }
            spriteRenderer.flipX = !isFacingRight;
        }

        private float ChooseAcceleration(float moveInput, float desiredVelocityX) {
            // Case 1) 이동을 멈추는 경우
            bool isStopping = moveInput == 0f;
            if (isStopping) {
                return playerStat.moveDecceleration;
            }

            // Case 2) 반대 방향으로 이동하려는 경우
            bool isTurningDirection = rb.velocity.x * desiredVelocityX < 0f;
            if (isTurningDirection) {
                return playerStat.turnAcceleration;
            }
            // Case 3) 기존 방향으로 계속 이동하는 경우
            return playerStat.moveAcceleration;
        }

        private void UpdateCameraFollowTarget() {
            Vector2 newPosition = transform.position;

            // 바라보는 방향으로 look ahead
            newPosition.x += isFacingRight ? cameraLookAheadDistance : -cameraLookAheadDistance;
            cameraFollowTarget.transform.position = newPosition;
        }

        private void HandleBoost(Vector2 moveInput) { //쓰읍 살짝 짜치네요
            if (playerStat.playerCurrentEnergy <= 0) {
                return;
            }
            if (isBoosting(moveInput)) {
                playerStat.playerCurrentEnergy -= playerStat.energyDecreaseRate * Time.deltaTime;
                virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, defaultOrthographicSize * 2f, zoomSpeed * Time.deltaTime); //Zoom Out
            }
            else {
                virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, defaultOrthographicSize, zoomSpeed * Time.deltaTime);; //Zoom In
            }
        }

        private bool isBoosting(Vector2 moveInput) { // Boost Condition 확인하는 함수
            return playerControl.Player.Boost.IsPressed() && moveInput.magnitude > 0 && playerStat.playerCurrentEnergy > 0;
        }
    }
}

