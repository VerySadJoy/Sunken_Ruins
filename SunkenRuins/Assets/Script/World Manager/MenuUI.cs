using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SunkenRuins {
    public class MenuUI : MonoBehaviour {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

        static void FirstLoad()
        {
            //게임 시작하면 무조건 타이틀 씬에서부터 시작
            if (SceneManager.GetActiveScene().name.CompareTo("Title Screen") != 0)
            {
                SceneManager.LoadScene("Demo Scene (HW)");
            }
        }

        //추후 변수 타입 변경
        [SerializeField] CanvasGroup SettingUI;

        public void OnStartButton() {
            SceneManager.LoadScene(1);
        }

        public void OnSettingButton() {
            SettingUI.blocksRaycasts = true;
            SettingUI.GetComponentInChildren<Canvas>().enabled = true;

        }
        public void gameQuit() //게임 종료
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
        }
    }
}