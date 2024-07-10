using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace SunkenRuins {
    public class MapSelectionUI : MonoBehaviour {

        public MapUI[] MapUIList;
        public MapAsset[] MapAssetList;
        
        private void Start() {
            MapUIList[0].mapNameText.text = MapAssetList[0].mapName;
            MapUIList[1].mapNameText.text = MapAssetList[1].mapName;
            MapUIList[2].mapNameText.text = MapAssetList[2].mapName;
        }

        public void LeftBoxOnClick(){
            MapUIList[0].transform.DOMoveX(150, 1);
            MapUIList[0].transform.DOScale(0.5f, 1);
            MapUIList[1].transform.DOMoveX(450, 1);
            MapUIList[1].transform.DOScale(2f, 1);
        }
    }
}
