using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;
using Unity.Mathematics;

namespace SunkenRuins
{
    public class PlayerManager : MonoBehaviour
    {
        // Singleton 구조
        public static PlayerManager Instance { get; private set; }

        // UnityEvent
        // public event EventHandler OnPlayerBoost;

        [Header("Follow Camera Target")]
        [SerializeField] private GameObject cameraFollowTarget;
        // 바라보는 방향으로 얼마나 앞에 있는 지점을 카메라가 추적할 것인지
        [SerializeField, Range(0f, 2f)] private float cameraLookAheadDistance = 1f;
        [SerializeField] private float zoomSpeed = 2f;

        //Component
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        public PlayerStat playerStat;
        private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float defaultOrthographicSize = 8f;
        [SerializeField] private float zoomOrthographicSize = 5f;
        private PlayerControl playerControl; // Input System
        private bool isFacingRight = true;

        // Layermask String
        private const string itemLayerString = "Item";
        private const string enemyLayerString = "Enemy";

        //Boost
        [SerializeField] private DottedLineUI boostTrajectoryLineUI;
        [SerializeField] private BoostBarUI boostBarUI;

        private bool isBoosting = false;
        private bool isBoostPreparing = false;
        private float lastBoostTime = 0f;
        private float boostCooldown = 1f;
        private float boostDuration = 0.5f;
        private bool temp = false;
        private bool hasBoostEventBeenInvoked = false;

        private void Awake()
        {
            if (Instance != null) Debug.LogError("There is more than one player instance");
            else Instance = this;

            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            playerStat = GetComponent<PlayerStat>();
            virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            virtualCamera.m_Lens.OrthographicSize = 8f;
        }

        private void OnTriggerEnter2D(Collider2D other)
        { 
            if (other.gameObject.layer == LayerMask.NameToLayer(itemLayerString))
            { //Item 획득
                ItemSO itemSO = other.gameObject.GetComponent<Item>().GetItemSO();
                if (itemSO != null)
                {
                    switch (itemSO.itemType)
                    {
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
            // else if (other.gameObject.layer == LayerMask.NameToLayer(enemyLayerString))
            // {
            //     EnemyStat monsterStat = other.gameObject.GetComponent<EnemyStat>();
            //     playerStat.Damage(monsterStat.teamType);
            // }
        }

        private void OnEnable()
        {
            playerControl = new PlayerControl();
            playerControl.Player.Enable();

            EventManager.StartListening(EventType.PlayerDamaged, Damage);
            EventManager.StartListening(EventType.HypnoCuttleFishHypnotize, Hypnotize);
            EventManager.StartListening(EventType.ShellAbsorb, GetAbsorbed);
            EventManager.StartListening(EventType.ShellEscape, EscapeFromEnemy);
            EventManager.StartListening(EventType.ShellSwallow, ShellSwallow);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.PlayerDamaged, Damage);
            EventManager.StopListening(EventType.HypnoCuttleFishHypnotize, Hypnotize);
            EventManager.StopListening(EventType.ShellAbsorb, GetAbsorbed);
            EventManager.StopListening(EventType.ShellEscape, EscapeFromEnemy);
            EventManager.StopListening(EventType.ShellSwallow, ShellSwallow);
        }

        private Vector3 dirFromShellNormalized;
        private bool isAbsorbed = false; private bool isSwallowed = false;
        private void Update()
        {
            HandleMoveInput();
            BoostInput();
            UpdateCameraFollowTarget();

            if (isAbsorbed)
            {
                transform.Translate(playerStat.absorbSpeed * dirFromShellNormalized * Time.deltaTime, Space.World); // Move dirToOtherNormalized per second
            }
        }

        public void ShellSwallow(Dictionary<string, object> message)
        {
            // 조개 중간 위치로 순간이동
            transform.position = (Vector3)message["shellPos"];

            // 버튼 연타 확인
            isSwallowed = true;
        }

        public void GetAbsorbed(Dictionary<string, object> message)
        {
            // rb.constraints = RigidbodyConstraints2D.FreezeAll;
            SetInputEnabled(false);
            isAbsorbed = true;
            dirFromShellNormalized = (Vector3)message["dirToPlayerNormalized"];
        }
        public void EscapeFromEnemy(Dictionary<string, object> message)
        {
            isAbsorbed = false;
            SetInputEnabled(true);
            rb.constraints = RigidbodyConstraints2D.None;
        }

        private void Damage(Dictionary<string ,object> message)
        {
            // TODO:
            // 1. 데미지 입는 효과
            // 2. 데미지 SFX
        }

        private void BoostInput()
        {
            float boostInput = playerControl.Player.Mouse.ReadValue<float>();
        
            // 부스트 방향 버그 수정
            UpdateFacingDirection(boostInput);
            if (temp && boostInput > 0)
            {
                boostInput = 0;
            }
            else if (temp && boostInput == 0)
            {
                temp = false;
            }
            else if (!temp && boostInput > 0)
            {
                //temp = true;
            } //언젠간 Refactoring하길... ㅎㅎ


            if (boostInput > 0 && playerStat.playerCurrentEnergy > 0 && !isBoosting 
                && Time.time > lastBoostTime + boostCooldown && playerStat.CanUseEnergy)
            {
                PrepareBoost();
            }

            if (boostInput == 0 && isBoostPreparing)
            {
                ExecuteBoost();
            }
            
            if (Input.GetKeyDown(KeyCode.E) && isBoostPreparing)
            { 
                //이거 나중에 Input System으로 수정해야함
                CancelBoost();
            }

        }

        private void PrepareBoost()
        {
            isBoostPreparing = true;
            Time.timeScale = 0.5f;
            if (!hasBoostEventBeenInvoked)
            {
                boostBarUI.SetNewScrollandImageValue();
                hasBoostEventBeenInvoked = true;
            } // 플레이어가 부스트를 시도하는 것을 UI에 알림
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, zoomOrthographicSize, zoomSpeed * Time.deltaTime); ; //Zoom In
            // TODO:
            // UI 보이기
            boostBarUI.SetUIActive(true);

            Debug.Log("부스트 준비");

            // 부스트 준비 중에도 Sprite 방향 신경쓰기
            Vector2 finalMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Input System으로 변경해야한다면 변경
            Vector2 boostDirection = (finalMousePosition - ((Vector2)transform.position)).normalized;
            UpdateFacingDirection(boostDirection.x);

            // 점선 잇기 (Player Position to Mouse Position)
            boostTrajectoryLineUI.LineEnable();
        }

        private void ExecuteBoost()
        {
            Time.timeScale = 1f;
            isBoostPreparing = false;
            isBoosting = true;
            lastBoostTime = Time.time;
            playerStat.playerCurrentEnergy--;
            hasBoostEventBeenInvoked = false;
            boostBarUI.SetUIActive(false);
            boostTrajectoryLineUI.LineDisable();

            Vector2 finalMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Input System으로 변경해야한다면 변경
            Vector2 boostDirection = ((finalMousePosition) - ((Vector2)transform.position)).normalized;
            StartCoroutine(ZoomOutCoroutine(defaultOrthographicSize, zoomSpeed)); // Zoom Out
            if (boostBarUI.IsPerfectBoost)
            {
                StartCoroutine(BoostMovement(boostDirection, playerStat.perfectBoostSpeed));
                Debug.Log("완벽 부스트");
            }
            else
            {
                StartCoroutine(BoostMovement(boostDirection, playerStat.normalBoostSpeed));
                Debug.Log("노말 부스트");
            }
        }

        private IEnumerator BoostMovement(Vector2 direction, float speed)
        {
            // 부스트 방향 버그 수정
            UpdateFacingDirection(direction.x);

            float elapsed = 0f;
            while (elapsed < boostDuration)
            {
                float moveStep = speed * Time.deltaTime;
                transform.Translate(direction * moveStep, Space.World);
                elapsed += Time.deltaTime;
                yield return null;
            }
            elapsed = 0f;
            Vector2 initialVelocity = direction * speed;

            while (elapsed < boostDuration)
            {
                float t = elapsed / boostDuration;
                Vector2 velocity = Vector2.Lerp(initialVelocity, Vector2.zero, t);
                transform.Translate(velocity * Time.deltaTime, Space.World);
                elapsed += Time.deltaTime;
                yield return null;
            }

            isBoosting = false;
        }

        private void CancelBoost()
        {
            isBoostPreparing = false;
            Time.timeScale = 1f;
            boostBarUI.SetUIActive(false);
            boostTrajectoryLineUI.LineDisable();
            hasBoostEventBeenInvoked = false;
            temp = true;

            StartCoroutine(ZoomOutCoroutine(defaultOrthographicSize, zoomSpeed));
            //TODO:
            //UI 가리기
            Debug.Log("취소");
        }

        private IEnumerator ZoomOutCoroutine(float targetOrthographicSize, float zoomSpeed)
        {
            float initialOrthographicSize = virtualCamera.m_Lens.OrthographicSize;

            while (virtualCamera.m_Lens.OrthographicSize > targetOrthographicSize)
            {
                float newOrthographicSize = Mathf.MoveTowards(virtualCamera.m_Lens.OrthographicSize, targetOrthographicSize, zoomSpeed * Time.deltaTime);
                virtualCamera.m_Lens.OrthographicSize = newOrthographicSize;
                yield return null;
            }
            virtualCamera.m_Lens.OrthographicSize = targetOrthographicSize; //최종실행
        }

        private void Hypnotize(Dictionary<string, object> message)
        {
            StartCoroutine(HypnotizeInputCoroutine());
        }

        private IEnumerator HypnotizeInputCoroutine()
        {
            SetInputEnabled(false);
            yield return new WaitForSeconds(playerStat.HypnotizeTime);
            SetInputEnabled(true);
        }

        public void SetInputEnabled(bool enable)
        { 
            // 컷신이나 뭐할때 Input 죽이는 용
            if (enable)
            {
                playerControl.Player.Enable();
            }
            else
            {
                playerControl.Player.Disable();
            }
        }

        private void HandleMoveInput()
        {
            Vector2 moveInput = playerControl.Player.Move.ReadValue<Vector2>();

            // 원하는 속도를 계산
            float desiredVelocityX = playerStat.moveSpeed * moveInput.x;
            float desiredVelocityY = playerStat.moveSpeed * moveInput.y;

            // 방향 전환 여부에 따라 다른 가속도 사용
            float accelerationX = ChooseAcceleration(moveInput.x, desiredVelocityX);
            float accelerationY = ChooseAcceleration(moveInput.y, desiredVelocityY);

            // x축 속도가 원하는 속도에 부드럽게 도달하도록 보간
            float updatedVelocityX = Mathf.MoveTowards(rb.velocity.x, desiredVelocityX, accelerationX * Time.deltaTime);
            float updatedVelocityY = Mathf.MoveTowards(rb.velocity.y, desiredVelocityY, accelerationY * Time.deltaTime);
            rb.velocity = new Vector2(updatedVelocityX, updatedVelocityY);
            //HandleBoost(moveInput);

            if (!isBoosting && !isBoostPreparing) UpdateFacingDirection(moveInput.x); // 부스트할 때는 마우스 방향만이 Sprite flip을 결정함
        }

        private void UpdateFacingDirection(float moveInputX)
        {
            if (moveInputX != 0f)
            {
                isFacingRight = moveInputX > 0f;
            }
            spriteRenderer.flipX = !isFacingRight;
        }

        private float ChooseAcceleration(float moveInput, float desiredVelocityX)
        {
            // Case 1) 이동을 멈추는 경우
            bool isStopping = moveInput == 0f;
            if (isStopping)
            {
                return playerStat.moveDecceleration;
            }

            // Case 2) 반대 방향으로 이동하려는 경우
            bool isTurningDirection = rb.velocity.x * desiredVelocityX < 0f;
            if (isTurningDirection)
            {
                return playerStat.turnAcceleration;
            }

            // Case 3) 기존 방향으로 계속 이동하는 경우
            return playerStat.moveAcceleration;
        }

        private void UpdateCameraFollowTarget()
        {
            Vector2 newPosition = transform.position;

            // 바라보는 방향으로 look ahead
            newPosition.x += isFacingRight ? cameraLookAheadDistance : -cameraLookAheadDistance;
            cameraFollowTarget.transform.position = newPosition;
        }

        // private void HandleBoost(Vector2 moveInput) { //쓰읍 살짝 짜치네요
        //     if (playerStat.playerCurrentEnergy <= 0) {
        //         return;
        //     }
        //     if (isBoosting(moveInput)) {
        //         playerStat.playerCurrentEnergy -= playerStat.energyDecreaseRate * Time.deltaTime;
        //         virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, defaultOrthographicSize * 2f, zoomSpeed * Time.deltaTime); //Zoom Out
        //     }
        //     else {
        //         virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, defaultOrthographicSize, zoomSpeed * Time.deltaTime);; //Zoom In
        //     }
        // }

        // private bool isBoosting(Vector2 moveInput) { // Boost Condition 확인하는 함수
        //     return playerControl.Player.Boost.IsPressed() && moveInput.magnitude > 0 && playerStat.playerCurrentEnergy > 0;
        // }
    }
}