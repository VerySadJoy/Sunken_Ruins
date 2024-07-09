using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUI : MonoBehaviour {

    private Canvas settingCanvas;

    private void Awake() {
        settingCanvas = GetComponent<Canvas>();
    }

    private void Update() {
        //UI Off
        if (Input.GetKeyDown(KeyCode.Escape)) {
            settingCanvas.enabled = false;
        }
    }
}
