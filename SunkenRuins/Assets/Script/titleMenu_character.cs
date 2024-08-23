using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class titleMenu_character : MonoBehaviour
{
    float initPos;
    RectTransform rect;
    [SerializeField] float waveSize;
    [SerializeField] float period; //한 번 위아래로 움직이는 데 걸리는 시간

    float timer = 0f;
    void Start()
    {
        rect = GetComponent<RectTransform>();
        initPos = rect.position.y;
    }

    void Update()
    {
        timer += Time.deltaTime;
        rect.position = new Vector3(rect.position.x, initPos + waveSize * Mathf.Sin(timer * 2 * Mathf.PI / period));
    }
}
