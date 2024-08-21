using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUI : MonoBehaviour {

    private Canvas settingCanvas;
    private CanvasGroup settingCanvasGroup;

    private void Awake() {
        settingCanvas = GetComponentInChildren<Canvas>();
        settingCanvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update() {
        //UI Off
        if (Input.GetKeyDown(KeyCode.Escape)) {
            settingCanvas.enabled = false;
            settingCanvasGroup.blocksRaycasts = false;
        }
    }
}
