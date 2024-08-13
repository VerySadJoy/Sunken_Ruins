using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SunkenRuins {
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private PlayerManager player;
        private Slider healthBar;
        private void Awake () {
            healthBar = GetComponentInChildren<Slider>();
        }
        private void Start() {
            healthBar.maxValue = player.playerStat.playerMaxHealth;
        }
        private void Update() {
            healthBar.value = player.playerStat.playerCurrentHealth;
        }
    }
}
