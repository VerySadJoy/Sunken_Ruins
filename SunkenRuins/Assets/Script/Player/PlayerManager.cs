using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;
using Unity.Mathematics;
using SunkenRuin;

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
        // 바라보는 방향?�로 ?�마???�에 ?�는 지?�을 카메?��? 추적??것인지
        [SerializeField, Range(0f, 2f)] private float cameraLookAheadDistance = 1f;
        [SerializeField] private float zoomSpeed = 2f;

        //Component
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        public PlayerStat playerStat;
        private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float defaultOrthographicSize = 8f;
        [SerializeField] private float zoomOrthographicSize = 5f;
        private PlayerControl playerControl { get; set; } // Input System
        private bool isFacingRight = true;

        // Layermask String
        private const string itemLayerString = "Item";
        private const string enemyLayerString = "Enemy";

        //Boost
        [SerializeField] private DottedLineUI boostTrajectoryLineUI;
        [SerializeField] private BoostBarUI boostBarUI;
        [SerializeField] private EnergyBarUI energyBarUI;
        [SerializeField] private PlayerEye playerEye;

        private bool isBoosting = false;
        private bool isBoostPreparing = false;
        private float lastBoostTime = 0f;
        private float boostCooldown = 1f;
        private float boostDuration = 0.5f;
        private bool temp = false;
        private bool hasBoostEventBeenInvoked = false;

        //BoostAnimation
        [SerializeField] GameObject boostEffectRing;
        [SerializeField] float ringAppearDelay_default; //부스트 링이 생기는 사이 시간간격
        [SerializeField] float ringAppearDelay_power; //파워 부스트 링 시간간격 

        [SerializeField] AnimationCurve ringSizeCurve;
        [SerializeField] AnimationCurve ringColorCurve;

        [SerializeField] Color ringColor_default;
        [SerializeField] Color ringColor_power;

        //bubble particle
        [SerializeField] ParticleSystem bubble;


        private void Awake()
        {
            if (Instance != null) Debug.LogError("There is more than one player instance");
            else Instance = this;

            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            playerStat = GetComponent<PlayerStat>();
            virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            virtualCamera.m_Lens.OrthographicSize = defaultOrthographicSize;
        }

        private void OnTriggerEnter2D(Collider2D other)
        { 
            if (other.gameObject.layer == LayerMask.NameToLayer(itemLayerString))
            { //Item ?�득
                ItemSO itemSO = other.gameObject.GetComponent<Item>().GetItemSO();
                if (itemSO != null)
                {
                    switch (itemSO.itemType)
                    {
                        case ItemType.HealthPotion:
                            playerStat.RestoreHealth(100);
                            break;
                        case ItemType.PowerBattery:
                            playerStat.RestoreEnergy(3f);
                            break;
                        case ItemType.BubbleShield:
                            playerStat.BeInvincible(2); // ?�단?� ?�드코딩?�로 invincibleTime ?�자�?받음
                            break;
                        default:
                            Debug.LogError("?�거 ?�오�??�됨");
                            break;
                    }
                }
                Destroy(other.gameObject); //?�이????��
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer(enemyLayerString)) {
                playerStat.playerCurrentHealth -= 20;
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

            EventManager.StartListening(EventType.PlayerToStartPosition, MoveToStartPosition);
            EventManager.StartListening(EventType.PlayerDamaged, Damage);
            EventManager.StartListening(EventType.HypnoCuttleFishHypnotize, Hypnotize);
            EventManager.StartListening(EventType.ShellAbsorb, GetAbsorbed);
            EventManager.StartListening(EventType.ShellEscape, EscapeFromEnemy);
            EventManager.StartListening(EventType.ShellSwallow, ShellSwallow);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.PlayerToStartPosition, MoveToStartPosition);
            EventManager.StopListening(EventType.PlayerDamaged, Damage);
            EventManager.StopListening(EventType.HypnoCuttleFishHypnotize, Hypnotize);
            EventManager.StopListening(EventType.ShellAbsorb, GetAbsorbed);
            EventManager.StopListening(EventType.ShellEscape, EscapeFromEnemy);
            EventManager.StopListening(EventType.ShellSwallow, ShellSwallow);
        }

        private Vector3 dirFromShellNormalized;
        private Vector3 shellPosition;
        private bool isAbsorbed = false;
        private bool isSwallowed = false;
        private void Update()
        {
            if (Time.timeScale == 0) {
                //Debug.Log("Stopped");
                return;
            }
            HandleMoveInput();
            BoostInput();
            UpdateCameraFollowTarget();


            if (playerStat.playerCurrentEnergy > playerStat.playerMaxEnergy) { 
                playerStat.playerCurrentEnergy = playerStat.playerMaxEnergy;
            }
            if (playerStat.playerCurrentHealth > playerStat.playerMaxHealth) { 
                playerStat.playerCurrentHealth = playerStat.playerMaxHealth;
            }
        }
        private void FixedUpdate (){
            if (isAbsorbed)
            {
                dirFromShellNormalized = (shellPosition - this.transform.position).normalized;
                rb.velocity += 1.15f * new Vector2(dirFromShellNormalized.x, dirFromShellNormalized.y);
            }
        }

        private void MoveToStartPosition(Dictionary<string, object> message)
        {
            transform.position = (Vector3)message["StartPosition"];
        }

        public void ShellSwallow(Dictionary<string, object> message)
        {
            // 조개 중간 ?�치�??�간?�동
            transform.position = (Vector3)message["shellPos"];

            // 버튼 ?��? ?�인
            isSwallowed = true;
        }

        public void GetAbsorbed(Dictionary<string, object> message)
        {
            SetBoostInputEnable(false);
            isAbsorbed = true;
            shellPosition = (Vector3)message["position"];
        }
        public void EscapeFromEnemy(Dictionary<string, object> message)
        {
            isAbsorbed = false;
            SetBoostInputEnable(true);
            rb.constraints = RigidbodyConstraints2D.None;
        }

        private void Damage(Dictionary<string ,object> message)
        {
            // TODO:
            // 1. ?��?지 ?�는 ?�과
            // 2. ?��?지 SFX
        }

        private void BoostInput()
        {
            float boostInput = playerControl.Player.Mouse.ReadValue<float>();
        
            // 부?�트 방향 버그 ?�정
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
            } //?�젠�?Refactoring?�길... ?�ㅎ


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
                //?�거 ?�중??Input System?�로 ?�정?�야??
                CancelBoost();
            }
        }

        private void PrepareBoost()
        {
            isBoostPreparing = true;
            Time.timeScale = 0.5f;
            if (!hasBoostEventBeenInvoked)
            {
                //boostBarUI.SetNewScrollandImageValue();
                hasBoostEventBeenInvoked = true;
            }
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, zoomOrthographicSize, zoomSpeed * Time.deltaTime); ; //Zoom In

            boostBarUI.SetUIActive(true);

            // 부?�트 준�?중에??Sprite 방향 ?�경?�기
            Vector2 finalMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Input System?�로 변경해?�한?�면 변�?
            Vector2 boostDirection = (finalMousePosition - ((Vector2)transform.position)).normalized;
            UpdateFacingDirection(boostDirection.x);

            // ?�선 ?�기 (Player Position to Mouse Position)
            boostTrajectoryLineUI.LineEnable();
        }

        private void ExecuteBoost()
        {
            Time.timeScale = 1f;
            isBoostPreparing = false;
            isBoosting = true;
            lastBoostTime = Time.time;
            playerStat.playerCurrentEnergy -= 1;
            hasBoostEventBeenInvoked = false;
            boostBarUI.SetUIActive(false);
            boostTrajectoryLineUI.LineDisable();

            Vector2 finalMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Input System?�로 변경해?�한?�면 변�?
            Vector2 boostDirection = ((finalMousePosition) - ((Vector2)transform.position)).normalized;
            StartCoroutine(ZoomOutCoroutine(defaultOrthographicSize, zoomSpeed)); // Zoom Out
            if (boostBarUI.IsPerfectBoost)
            {
                StartCoroutine(BoostMovement(boostDirection, playerStat.perfectBoostSpeed, true));
                EventManager.TriggerEvent(EventType.PerfectBoost, null);
                // Debug.Log("Perfect Boost");
            }
            else
            {
                StartCoroutine(BoostMovement(boostDirection, playerStat.normalBoostSpeed, false));
                EventManager.TriggerEvent(EventType.NormalBoost, null);
                // Debug.Log("Normal Boost");
            }
        }

        private IEnumerator BoostMovement(Vector2 direction, float speed, bool isPerfect)
        {
            // 부?�트 방향 버그 ?�정
            UpdateFacingDirection(direction.x);

            IEnumerator boostEffectCor = makeBoostRing(direction, isPerfect);

            var ps = bubble.emission;
            ps.enabled = true;
            ps.rateOverTime = 15f; // 버블 발생량 늘림 

            boostEffectOffset = 0f;
            StartCoroutine(boostEffectCor);

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
            isPlayerDecellerating = true;

            while (elapsed < boostDuration)
            {
                float t = elapsed / boostDuration;
                Vector2 velocity = Vector2.Lerp(initialVelocity, Vector2.zero, t);
                transform.Translate(velocity * Time.deltaTime, Space.World);
                elapsed += Time.deltaTime;

                boostEffectOffset = t; //오프셋 줄어듦
                yield return null;
            }

            isBoosting = false;
            StopCoroutine(boostEffectCor);

            ps.rateOverTime = 5f;
            isPlayerDecellerating = false;
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

        float boostEffectOffset;
        bool isPlayerDecellerating = false;

        IEnumerator makeBoostRing(Vector2 boostDir, bool isPower)
        {
            float ringDelay;
            Color ringColor;
           
            if (isPower)
            {
                ringDelay = ringAppearDelay_power;
                ringColor = ringColor_power;
            }
            else
            {
                ringDelay = ringAppearDelay_default;
                ringColor = ringColor_default;
            }

            float boostTimer = ringDelay;

            while (true)
            {
                boostTimer += Time.deltaTime;

                //일정 주기로 실행
                if(boostTimer >= ringDelay)
                {
                    if (isPlayerDecellerating)
                    {
                        ringDelay = ringDelay * 1.5f;
                    }

                    boostTimer = 0f;
                    GameObject targetEffectRing = Instantiate(boostEffectRing);
                    boostEffectRing targetScript = targetEffectRing.GetComponent<boostEffectRing>();

                    //커브 애니메이션 할당 
                    targetScript.scaleCurve = ringSizeCurve;
                    targetScript.colorCurve = ringColorCurve;
                    targetScript.emitColor = ringColor;

                    //부스트 방향 기반으로 각도 구하기 
                    float boostAngle;
                    boostAngle = Mathf.Atan2(boostDir.y, boostDir.x) * Mathf.Rad2Deg;

                    /*
                    if(boostAngle < 0f)
                    {
                        //부스트 방향이 2,3사분면에 있으면
                        boostAngle += 180f; //반대쪽 사분면으로 넘기기
                        targetEffectRing.GetComponent<SpriteRenderer>().flipX = true;
                    }
                    */

                    targetEffectRing.transform.eulerAngles = new Vector3(0, 0, boostAngle); //각도 조절
                    targetEffectRing.transform.position = transform.position - (Vector3)boostDir.normalized * boostEffectOffset - new Vector3(0, 0.25f); //위치 조절 
                }
                yield return null;
            }
        }

        private IEnumerator ZoomOutCoroutine(float targetOrthographicSize, float zoomSpeed) {
            float initialOrthographicSize = virtualCamera.m_Lens.OrthographicSize;

            while (Mathf.Abs(virtualCamera.m_Lens.OrthographicSize - targetOrthographicSize) > 0.01f)
            {
                float newOrthographicSize = Mathf.MoveTowards(virtualCamera.m_Lens.OrthographicSize, targetOrthographicSize, zoomSpeed * Time.deltaTime);
                virtualCamera.m_Lens.OrthographicSize = newOrthographicSize;
                yield return null;
            }
            virtualCamera.m_Lens.OrthographicSize = targetOrthographicSize;
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
            // 컷신?�나 뭐할??Input 죽이????
            if (enable)
            {
                playerControl.Player.Mouse.Enable();
            }
            else
            {
                playerControl.Player.Mouse.Disable();
            }
        }
        public void SetBoostInputEnable(bool enable) {
            if (enable)
            {
                playerControl.Player.Mouse.Enable();
            }
            else
            {
                playerControl.Player.Mouse.Disable();
            }
        }

        private void HandleMoveInput()
        {
            Vector2 moveInput = playerControl.Player.Move.ReadValue<Vector2>();

            // ?�하???�도�?계산
            float desiredVelocityX = playerStat.moveSpeed * moveInput.x;
            float desiredVelocityY = playerStat.moveSpeed * moveInput.y;

            // 방향 ?�환 ?��????�라 ?�른 가?�도 ?�용
            float accelerationX = ChooseAcceleration(moveInput.x, desiredVelocityX);
            float accelerationY = ChooseAcceleration(moveInput.y, desiredVelocityY);

            // x�??�도가 ?�하???�도??부?�럽�??�달?�도�?보간
            float updatedVelocityX = Mathf.MoveTowards(rb.velocity.x, desiredVelocityX, accelerationX * Time.deltaTime);
            float updatedVelocityY = Mathf.MoveTowards(rb.velocity.y, desiredVelocityY, accelerationY * Time.deltaTime);
            rb.velocity = new Vector2(updatedVelocityX, updatedVelocityY);

            if (rb.velocity.y > 0)
            {
                EventManager.TriggerEvent(EventType.MoveUp, null);
            }
            else if (rb.velocity.y < 0)
            {
                EventManager.TriggerEvent(EventType.MoveDown, null);
            }
            else
            {
                EventManager.TriggerEvent(EventType.MoveIdle, null);
            }

            //HandleBoost(moveInput);

            if (!isBoosting && !isBoostPreparing) UpdateFacingDirection(moveInput.x); // 부?�트???�는 마우??방향만이 Sprite flip??결정??
        }

        private void UpdateFacingDirection(float moveInputX)
        {
            if (moveInputX != 0f)
            {
                isFacingRight = moveInputX > 0f;
                playerEye.FlipEyeSprite(!isFacingRight);
            }
            spriteRenderer.flipX = !isFacingRight;
        }

        private float ChooseAcceleration(float moveInput, float desiredVelocityX)
        {
            // Case 1) ?�동??멈추??경우
            bool isStopping = moveInput == 0f;
            if (isStopping)
            {
                return playerStat.moveDecceleration;
            }

            // Case 2) 반�? 방향?�로 ?�동?�려??경우
            bool isTurningDirection = rb.velocity.x * desiredVelocityX < 0f;
            if (isTurningDirection)
            {
                return playerStat.turnAcceleration;
            }

            // Case 3) 기존 방향?�로 계속 ?�동?�는 경우
            return playerStat.moveAcceleration;
        }

        private void UpdateCameraFollowTarget()
        {
            Vector2 newPosition = transform.position;

            // 바라보는 방향?�로 look ahead
            newPosition.x += isFacingRight ? cameraLookAheadDistance : -cameraLookAheadDistance;
            cameraFollowTarget.transform.position = newPosition;
        }

        // private void HandleBoost(Vector2 moveInput) { //?�읍 ?�짝 짜치?�요
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

        // private bool isBoosting(Vector2 moveInput) { // Boost Condition ?�인?�는 ?�수
        //     return playerControl.Player.Boost.IsPressed() && moveInput.magnitude > 0 && playerStat.playerCurrentEnergy > 0;
        // }
    }
}