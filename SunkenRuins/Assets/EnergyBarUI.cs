using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SunkenRuins {
    public class EnergyBarUI : MonoBehaviour
    {
        [SerializeField] private PlayerManager player;
        private Slider energyBar;
        private void Awake () {
            energyBar = GetComponentInChildren<Slider>();
        }
        private void Start() {
            energyBar.maxValue = player.playerStat.playerMaxEnergy;
        }
        private void Update() {
            energyBar.value = player.playerStat.playerCurrentEnergy;
        }
    }
}
