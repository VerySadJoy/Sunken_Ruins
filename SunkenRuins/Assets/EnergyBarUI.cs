using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SunkenRuins {
    public class EnergyBarUI : MonoBehaviour
    {
        private PlayerManager player;
        private Image[] energyLevel;
        [SerializeField] private Sprite energyFullSprite;
        [SerializeField] private Sprite energyBlankSprite;

        private void Awake () {
            player = GetComponentInParent<PlayerManager>();
            // energyLevel = GetComponentsInChildren<Image>();
        }
        private void Update() {
            // if (player.playerStat.playerCurrentEnergy >= 3) {
            //     energyLevel[0].image = energyFullSprite;
            //     energyLevel[1].sprite = energyFullSprite;
            //     energyLevel[2].sprite = energyFullSprite;
            // }
            // else if (player.playerStat.playerCurrentEnergy == 2) {
            //     energyLevel[0].sprite = energyFullSprite;
            //     energyLevel[1].sprite = energyFullSprite;
            //     energyLevel[2].sprite = energyBlankSprite;
            // }
            // else if (player.playerStat.playerCurrentEnergy == 1) {
            //     energyLevel[0].sprite = energyFullSprite;
            //     energyLevel[1].sprite = energyBlankSprite;
            //     energyLevel[2].sprite = energyBlankSprite;
            // }
            // else if (player.playerStat.playerCurrentEnergy == 0) {
            //     energyLevel[0].sprite = energyBlankSprite;
            //     energyLevel[1].sprite = energyBlankSprite;
            //     energyLevel[2].sprite = energyBlankSprite;
            // }
        }
        public void SetUIActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
