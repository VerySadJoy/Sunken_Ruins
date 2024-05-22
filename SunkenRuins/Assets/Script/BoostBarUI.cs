using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace SunkenRuins
{
    public class BoostBarUI : MonoBehaviour
    {
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private RectTransform highlightImageTransform;
        [SerializeField] private float progressChangeSpeed = 1.5f;

        private void Start()
        {
            PlayerManager.Instance.OnPlayerBoost += OnPlayerBoost_SetNewScrollandImageValue;
        }

        // 플레이어가 부스트 시도할 때마다 UI 바는 새로운 위치에서 시작함
        private void OnPlayerBoost_SetNewScrollandImageValue(object sender, EventArgs e)
        {
            // Scroll Reposition
            scrollbar.value = UnityEngine.Random.Range(0f, 1f);

            // Highlighted Image Reposition
            highlightImageTransform.localPosition = Vector3.right * UnityEngine.Random.Range(-80f, 80f);
        }        

        private void Update()
        {
            if (scrollbar.value >= 1)
            {
                DecrementBoostProgress();
            }
            else if (scrollbar.value <= 0)
            {
                IncrementBoostProgress();
            }

            scrollbar.value += progressChangeSpeed * Time.deltaTime;

        }

        public void IncrementBoostProgress()
        {
            progressChangeSpeed = Mathf.Abs(progressChangeSpeed);
        }

        public void DecrementBoostProgress()
        {
            progressChangeSpeed = -Mathf.Abs(progressChangeSpeed);
        }
    }
}