using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SunkenRuins {
    
    public class MapClickUI : MonoBehaviour {

        [SerializeField] private Canvas mapSelectionCanvas;

        public void buttonOnClick() {
            mapSelectionCanvas.enabled = true;
        }

        private void Update() {
            //UI Off
            if (Input.GetKeyDown(KeyCode.Escape)) {
                mapSelectionCanvas.enabled = false;
            }
        }
    }
}