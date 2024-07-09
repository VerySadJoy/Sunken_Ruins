using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SunkenRuins {
    public class MenuUI : MonoBehaviour {

        //추후 변수 타입 변경
        [SerializeField] GameObject SettingUI;

        public void OnStartButton() {
            //SceneManager.LoadScene(1);
        }

        public void OnSettingButton() {
            SettingUI.SetActive(true);
        }
    }
}