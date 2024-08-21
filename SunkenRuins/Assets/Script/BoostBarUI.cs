using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace SunkenRuins
{
    public class BoostBarUI : MonoBehaviour
    {
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private RectTransform scrollbarTransform;
        [SerializeField] private RectTransform highlightImageTransform;
        [SerializeField] private float progressChangeSpeed = 1.5f;

        private float scrollbarWidth;
        private float scrollbarHalfWidth;
        private float highLightImageRange;
        public bool IsPerfectBoost // 스크롤이 빨간 area에 있을 때 부스트를 실행하면 perfectBoostSpeed를 가진다.
        {
            get
            {
                // 80f로 더하고 160f로 나누는 이유는 localPosition의 시작과 끝이 각각 -80f, 80f이기 때문이다
                // scrollbar value는 0부터 1이므로 바꿔줄 필요가 있다.
                // 0f부터 160f까지의 범위를 160f로 나누면 0부터 1이 나온다.
                return scrollbar.value < (scrollbarHalfWidth + highlightImageTransform.localPosition.x) / scrollbarWidth + highLightImageRange
                && scrollbar.value > (scrollbarHalfWidth + highlightImageTransform.localPosition.x) / scrollbarWidth - highLightImageRange; 
            }
        }

        private void Start()
        {
            // PlayerManager.Instance.OnPlayerBoost += OnPlayerBoost_SetNewScrollandImageValue;

            scrollbarWidth = scrollbarTransform.rect.width;
            scrollbarHalfWidth = scrollbarWidth / 2f;

            // 320으로 나누는 이유는 2로 나누어 width의 절반을 구한 후에
            float highLightImageHalfWidth = highlightImageTransform.rect.width / 2f;
            // scroll이 이동하는 UI 바의 전체 width로 나눈다
            // 그래야 scrollbar Value가 0부터 1인 것에 맞출 수 있다
            highLightImageRange = highLightImageHalfWidth / scrollbarWidth;
        }

        // 플레이어가 부스트 시도할 때마다 UI 바는 새로운 위치에서 시작함
        // private void OnPlayerBoost_SetNewScrollandImageValue(object sender, EventArgs e)
        // {
        //     // Scroll Reposition
        //     scrollbar.value = UnityEngine.Random.Range(0f, 1f);

        //     // Highlighted Image Reposition
        //     highlightImageTransform.localPosition = Vector3.right * UnityEngine.Random.Range(-80f, 80f);
        // }

        public void SetNewScrollandImageValue()
        {
            // Scroll Reposition
            scrollbar.value = UnityEngine.Random.Range(0f, 1f);

            // Highlighted Image Reposition
            highlightImageTransform.localPosition = Vector3.right * UnityEngine.Random.Range(-60f, 60f);
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

        public void SetUIActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}