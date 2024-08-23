using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins {
    public class WorldManager : MonoBehaviour
    {
        [SerializeField] private GameObject pauseUI;
        private void Update(){
            if (Input.GetKeyDown(KeyCode.Escape)) {
                pauseUI.SetActive(true);
                
                pauseUI.GetComponent<PauseUI>().OnPause();
            }
        }
    }
}