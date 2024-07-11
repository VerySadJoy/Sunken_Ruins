using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using TMPro.Examples;

namespace SunkenRuins {
    public class MapSelectionUI : MonoBehaviour {
        private const int SCROLL_SPEED = 1;
        public MapUI[] MapUIList;
        public MapAsset[] MapAssetList;
        
        private void Start() { //추후 맵 값 정확히 넣어주는 함수 필요
            MapUIList[1].mapNameText.text = MapAssetList[0].mapName;
            MapUIList[2].mapNameText.text = MapAssetList[1].mapName;
            MapUIList[3].mapNameText.text = MapAssetList[2].mapName;
        }

        public void LeftBoxOnClick(){
            Debug.Log("left");
            MapUIList[0].transform.DOMoveX(1050, SCROLL_SPEED);

            MapUIList[1].transform.DOMoveX(-150, SCROLL_SPEED);
            MapUIList[1].transform.DOScale(0f, SCROLL_SPEED);
            
            MapUIList[2].transform.DOMoveX(150, SCROLL_SPEED);
            MapUIList[2].transform.DOScale(1f, SCROLL_SPEED);

            MapUIList[3].transform.DOMoveX(450, SCROLL_SPEED);
            MapUIList[3].transform.DOScale(2f, SCROLL_SPEED);

            MapUIList[4].transform.DOMoveX(750, SCROLL_SPEED);
            MapUIList[4].transform.DOScale(1f, SCROLL_SPEED);
            MapUIList = ConvertArrayLeft(MapUIList);
        }

        public void RightBoxOnClick(){
            Debug.Log("right");
            MapUIList[4].transform.DOMoveX(-150, SCROLL_SPEED);

            MapUIList[3].transform.DOMoveX(1050, SCROLL_SPEED);
            MapUIList[3].transform.DOScale(0f, SCROLL_SPEED);
            
            MapUIList[2].transform.DOMoveX(750, SCROLL_SPEED);
            MapUIList[2].transform.DOScale(1f, SCROLL_SPEED);

            MapUIList[1].transform.DOMoveX(450, SCROLL_SPEED);
            MapUIList[1].transform.DOScale(2f, SCROLL_SPEED);

            MapUIList[0].transform.DOMoveX(150, SCROLL_SPEED);
            MapUIList[0].transform.DOScale(1f, SCROLL_SPEED);
            MapUIList = ConvertArrayRight(MapUIList);
        }

        public void CenterBoxOnClick(){
            Debug.Log("center");
        }

        public static MapUI[] ConvertArrayLeft(MapUI[] inputArray) {
            int length = inputArray.Length;
            if (length == 0) {
                return inputArray;
            }

            MapUI[] outputArray = new MapUI[length];

            for (int i = 0; i < length; i++) {
                int newIndex = (i + length - 1) % length;
                outputArray[newIndex] = inputArray[i];
            }

            return outputArray;
        }

        public static MapUI[] ConvertArrayRight(MapUI[] inputArray) {
            int length = inputArray.Length;
            if (length == 0) {
                return inputArray;
            }

            MapUI[] outputArray = new MapUI[length];

            for (int i = 0; i < length; i++) {
                int newIndex = (i + 1) % length;
                outputArray[newIndex] = inputArray[i];
            }

            return outputArray;
        }
    }
}
