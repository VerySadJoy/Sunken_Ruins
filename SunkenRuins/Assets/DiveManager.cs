using System.Collections;
using System.Collections.Generic;
using SunkenRuins;
using UnityEngine;

public class DiveManager : MonoBehaviour
{
    public void LoadNewScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(4); // load DemoScene
    }

    public void turnDiveSFX()
    {
        SFXManager.instance.PlaySFX(10); // 풍덩 SFX
        
    }
}
