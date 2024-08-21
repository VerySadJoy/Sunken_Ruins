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
            FaceUp,
            FaceDown,
        }

        // 0~2nd - Idle
        // 3rd - Normal Boost
        // 4th - Dead || Game Over
        // 5th - Paralyze || Hypnotize
        // 6th - Perfect Boost || Game Clear
        [SerializeField] private Sprite[] playerEyes;
        private SpriteRenderer spriteRenderer;
        private PlayerState playerState;

        private void OnEnable()
        {
            EventManager.StartListening(SunkenRuins.EventType.NormalBoost, OnNormalBoost);
            EventManager.StartListening(SunkenRuins.EventType.PerfectBoost, OnPerfectBoost);
            EventManager.StartListening(SunkenRuins.EventType.PlayerDamaged, OnEffect);
            EventManager.StartListening(SunkenRuins.EventType.MoveUp, OnMoveUp);
            EventManager.StartListening(SunkenRuins.EventType.MoveDown, OnMoveDown);
            EventManager.StartListening(SunkenRuins.EventType.MoveIdle, OnMoveIdle);
        }

        private void OnDisable()
        {
            EventManager.StopListening(SunkenRuins.EventType.NormalBoost, OnNormalBoost);
            EventManager.StopListening(SunkenRuins.EventType.PerfectBoost, OnPerfectBoost);
            EventManager.StopListening(SunkenRuins.EventType.PlayerDamaged, OnEffect);
            EventManager.StopListening(SunkenRuins.EventType.MoveUp, OnMoveUp);
            EventManager.StopListening(SunkenRuins.EventType.MoveDown, OnMoveDown);
            EventManager.StopListening(SunkenRuins.EventType.MoveIdle, OnMoveIdle);
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
                    spriteRenderer.sprite = playerEyes[1];
                    break;

                case PlayerState.FaceUp:
                    spriteRenderer.sprite = playerEyes[0];
                    break;

                case PlayerState.FaceDown:
                    spriteRenderer.sprite = playerEyes[2];
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

        private void OnMoveUp(Dictionary<string, object> message)
        {
            if (playerState == PlayerState.NormalBoost || playerState == PlayerState.PerfectBoost 
                || playerState == PlayerState.Effect) return;
            playerState = PlayerState.FaceUp;
        }

        private void OnMoveDown(Dictionary<string, object> message)
        {
            if (playerState == PlayerState.NormalBoost || playerState == PlayerState.PerfectBoost 
                || playerState == PlayerState.Effect) return;
            playerState = PlayerState.FaceDown;
        }

        private void OnMoveIdle(Dictionary<string ,object> message)
        {
            if (playerState == PlayerState.NormalBoost || playerState == PlayerState.PerfectBoost 
                || playerState == PlayerState.Effect) return;
            playerState = PlayerState.Idle;
        }

        private IEnumerator ChangeEyeCoroutine(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
            yield return new WaitForSeconds(1f);

            playerState = PlayerState.Idle; // change animation back to idle
        }

        public void FlipEyeSprite(bool isFlip)
        {
            spriteRenderer.flipX = isFlip;
        }
    }
}