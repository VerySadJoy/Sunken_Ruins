using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SunkenRuins {
    public class MapSelectionUI : MonoBehaviour {

        public MapUI[] MapUIList;
        public MapAsset[] MapAssetList;

        private void Awake() {
            
        }
        
        private void Start() {
            MapUIList[0].mapNameText.text = MapAssetList[0].mapName;
            MapUIList[1].mapNameText.text = MapAssetList[1].mapName;
            MapUIList[2].mapNameText.text = MapAssetList[2].mapName;
        }
    }
}
