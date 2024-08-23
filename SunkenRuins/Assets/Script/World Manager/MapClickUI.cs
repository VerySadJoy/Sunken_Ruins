using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SunkenRuins {
    
    public class MapClickUI : MonoBehaviour {

        [SerializeField] int selectedStageNum = 0; //시작할 스테이지 번호
        [SerializeField] int maxStageNum;
        [SerializeField] Sprite[] selectedStageImage;
        [SerializeField] Image targetImage;

        public void stageStart() {
            Debug.Log("game start: " + selectedStageNum + " stage");
        }

        public void returnToTitle()
        {
            //원래 화면으로 돌아가기
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
            if(selectedStageNum < maxStageNum - 1)
            {
                selectedStageNum++;
            }
        }
    }
}