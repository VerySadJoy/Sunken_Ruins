using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace SunkenRuins {
    public class MapUI : MonoBehaviour {

        public TextMeshProUGUI mapNameText;        
        private void Awake() {
            mapNameText = GetComponentInChildren<TextMeshProUGUI>();
            if (mapNameText == null) {
                Debug.Log("hi");
            }
        }
    
    }
}
