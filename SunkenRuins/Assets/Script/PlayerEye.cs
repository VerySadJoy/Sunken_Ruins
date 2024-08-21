using System.Collections;
using System.Collections.Generic;
using System.Text;
using SunkenRuins;
using UnityEngine;
using UnityEngine.UIElements;

namespace SunkenRuin
{
    public class PlayerEye : MonoBehaviour
    {
        enum PlayerState
        {
            Idle,
            NormalBoost,
            GameOver,
            Effect,
            PerfectBoost,
        }

        // 0~2nd - Idle
        // 3rd - Normal Boost
        // 4th - Dead || Game Over
        // 5th - Paralyze || Hypnotize
        // 6th - Perfect Boost || Game Clear
        [SerializeField] private Sprite[] playerEyes;
        private SpriteRenderer spriteRenderer;
        private PlayerState playerState;
        private float timer = 0f; private int idleEyeNumber = 0; private int eyeNumberIncrement = 1;

        private void OnEnable()
        {
            EventManager.StartListening(SunkenRuins.EventType.NormalBoost, OnNormalBoost);
            EventManager.StartListening(SunkenRuins.EventType.PerfectBoost, OnPerfectBoost);
            EventManager.StartListening(SunkenRuins.EventType.PlayerDamaged, OnEffect);
        }

        private void OnDisable()
        {
            EventManager.StopListening(SunkenRuins.EventType.NormalBoost, OnNormalBoost);
            EventManager.StopListening(SunkenRuins.EventType.PerfectBoost, OnPerfectBoost);
            EventManager.StopListening(SunkenRuins.EventType.PlayerDamaged, OnEffect);
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            playerState = PlayerState.Idle;
        }

        private void Update()
        {
            switch (playerState)
            {
                case PlayerState.Idle:
                    IdleAnimation();
                    break;

                case PlayerState.NormalBoost:
                    StartCoroutine(ChangeEyeCoroutine(playerEyes[3]));
                    break;

                case PlayerState.GameOver:
                    StartCoroutine(ChangeEyeCoroutine(playerEyes[4]));
                    break;

                case PlayerState.Effect:
                    StartCoroutine(ChangeEyeCoroutine(playerEyes[5]));
                    break;

                case PlayerState.PerfectBoost:
                    StartCoroutine(ChangeEyeCoroutine(playerEyes[6]));
                    break;
            }

            timer += Time.deltaTime;
        }

        private void OnNormalBoost(Dictionary<string ,object> message)
        {
            playerState = PlayerState.NormalBoost;
        }

        private void OnPerfectBoost(Dictionary<string ,object> message)
        {
            playerState = PlayerState.PerfectBoost;
        }

        private void OnEffect(Dictionary<string ,object> message)
        {
            playerState = PlayerState.Effect;
        }

        private void IdleAnimation()
        {
            if (timer >= 0.15f)
            {
                spriteRenderer.sprite = playerEyes[idleEyeNumber];
                idleEyeNumber += eyeNumberIncrement;
                timer = 0f; // reset timer

                if (idleEyeNumber >= 2 || idleEyeNumber <= 0) eyeNumberIncrement *= -1;
            }
        }

        private IEnumerator ChangeEyeCoroutine(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
            yield return new WaitForSeconds(1f);

            playerState = PlayerState.Idle; // change animation back to idle
            idleEyeNumber = 1; timer = 0.15f;
        }

        public void FlipEyeSprite(bool isFlip)
        {
            spriteRenderer.flipX = isFlip;
        }
    }
}