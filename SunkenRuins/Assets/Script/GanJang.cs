using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GanJang : MonoBehaviour
{
    private float fadeDuration = 2.5f;
    private string playerLayerString = "Player";
    [SerializeField] private Image clearImage;
    [SerializeField] private GameObject text;

    private void Start()
    {
        text.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerString))
        {
            GetComponentInChildren<SpriteRenderer>().sprite = null;
            text.SetActive(true);
            StartCoroutine(clearFadeCoroutine());
        }
    }

    private IEnumerator clearFadeCoroutine()
    {
        float elapsedTime = 0f;
            
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            clearImage.color = new Color(255f, 255f, 255f, alpha);
            yield return null;
        }

        text.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1); // load to BaseScene
    }
}
