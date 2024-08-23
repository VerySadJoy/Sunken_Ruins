using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SunkenRuins {
    public class EnergyBarUI : MonoBehaviour
    {
        [SerializeField] private PlayerManager player;
        private RectTransform[] energyBarImages;

        private void Awake () {
            energyBarImages = GetComponentsInChildren<RectTransform>(); // 3(right), 4(left), 5(center)가 꽉 찬 친구들
        }

        private void Start() {
            for (int i = 0; i < energyBarImages.Length; ++i)
            {
                energyBarImages[i].gameObject.SetActive(true);
            }
        }

        private void Update() {
            if (player.playerStat.playerCurrentEnergy >= 3)
            {
                energyBarImages[4].gameObject.SetActive(true);
                energyBarImages[5].gameObject.SetActive(true);
                energyBarImages[6].gameObject.SetActive(true);
            }
            else if (player.playerStat.playerCurrentEnergy == 2)
            {
                energyBarImages[4].gameObject.SetActive(false);
                energyBarImages[5].gameObject.SetActive(true);
                energyBarImages[6].gameObject.SetActive(true);
            }
            else if (player.playerStat.playerCurrentEnergy == 1)
            {
                energyBarImages[4].gameObject.SetActive(false);
                energyBarImages[5].gameObject.SetActive(false);
                energyBarImages[6].gameObject.SetActive(true);
            }
            else if (player.playerStat.playerCurrentEnergy <= 0)
            {
                energyBarImages[4].gameObject.SetActive(false);
                energyBarImages[5].gameObject.SetActive(false);
                energyBarImages[6].gameObject.SetActive(false);
            }
        }
    }
}
