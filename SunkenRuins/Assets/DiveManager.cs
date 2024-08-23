using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveManager : MonoBehaviour
{
    public void LoadNewScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(4); // load DemoScene
    }
}
