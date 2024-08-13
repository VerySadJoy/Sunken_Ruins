using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SunkenRuins {
    public class EnergyBarUI : MonoBehaviour
    {
        private PlayerManager player;
        private Image[] energyLevel;

        private void Awake () {
            player = GetComponentInParent<PlayerManager>();
            energyLevel = GetComponentsInChildren<Image>();
        }
        private void Update() {
            if (player.playerStat.playerCurrentEnergy == 3) {
                energyLevel[0].color = Color.green;
                energyLevel[1].color = Color.green;
                energyLevel[2].color = Color.green;
            }
            else if (player.playerStat.playerCurrentEnergy == 2) {
                energyLevel[0].color = Color.green;
                energyLevel[1].color = Color.white;
                energyLevel[2].color = Color.green;
            }
            else if (player.playerStat.playerCurrentEnergy == 1) {
                energyLevel[0].color = Color.white;
                energyLevel[1].color = Color.white;
                energyLevel[2].color = Color.green;
            }
            else if (player.playerStat.playerCurrentEnergy == 0) {
                energyLevel[0].color = Color.white;
                energyLevel[1].color = Color.white;
                energyLevel[2].color = Color.white;
            }
        }
        public void SetUIActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
