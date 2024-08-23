using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SunkenRuins {
    
    public class MapClickUI : MonoBehaviour {

        [SerializeField] int selectedStageNum = 0; //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½È£
        [SerializeField] int maxStageNum;
        [SerializeField] Sprite[] selectedStageImage;
        [SerializeField] Sprite[] selectedStageImage_blur;

        [SerializeField] Image targetImage;
        [SerializeField] Image blurImage;


        private void Update()
        {
            targetImage.sprite = selectedStageImage[selectedStageNum];
            blurImage.sprite = selectedStageImage_blur[selectedStageNum];
        }
        public void stageStart() {
            Debug.Log("game start: " + selectedStageNum + " stage");

            switch (selectedStageNum)
            {
                case 0:
                    SceneManager.LoadScene(2); //Æ©Åä¸®¾ó
                    break;
                case 1:
                    SceneManager.LoadScene(3); //½ºÅ×ÀÌÁö1
                    break;
            }
        }

        public void returnToTitle()
        {
            //ï¿½ï¿½ï¿½ï¿½ È­ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½Æ°ï¿½ï¿½ï¿½
            SceneManager.LoadScene("Title Screen");
        }

        public void moveLeft()
        {
            if(selectedStageNum > 0)
            {
                selectedStageNum--;
            }
        }

        public void moveRight()
        {
            if(selectedStageNum < maxStageNum)
            {
                selectedStageNum++;
            }
        }
    }
}