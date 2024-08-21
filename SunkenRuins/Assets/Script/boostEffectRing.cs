using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boostEffectRing : MonoBehaviour
{
    [SerializeField] float lifeTime;
    float lifeTimer;

    [HideInInspector] public AnimationCurve scaleCurve;
    [HideInInspector] public AnimationCurve colorCurve;

    SpriteRenderer spr;
    Color initColor;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);

        initColor = spr.color; //초기 색상, 알파값 가져옴
        //spr.color = new Color(initColor.r, initColor.g, initColor.b, 0); //맨 처음에는 투명화

        lifeTimer = 0f; //타이머 초기화 
    }

    void Update()
    {
        scaleEdit();
    }

    void scaleEdit()
    {
        if(lifeTimer > lifeTime)
        {
            Destroy(gameObject); //시간 지나면 파괴
        }

        lifeTimer += Time.deltaTime;
        transform.localScale = new Vector3(1, 1, 1) * scaleCurve.Evaluate(lifeTimer / lifeTime); //크기 조절
        spr.color = new Color(initColor.r, initColor.g, initColor.b, colorCurve.Evaluate(lifeTimer / lifeTime)); //색상 조절 
    }
}
