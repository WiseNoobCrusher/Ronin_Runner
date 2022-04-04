using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameFadeBlack : MonoBehaviour
{
    // Public Variables
    // Inspector Variables
    public Image fadeBlack;
    public float fadeSpeed = 0.5f;

    // Private Variables
    private float fadeAmount;

    private void Start()
    {
        fadeBlack.gameObject.SetActive(true);
        StartCoroutine(FadeToBlack());
    }

    public IEnumerator FadeToBlack(bool fade = false)
    {
        if (fadeBlack != null)
        {
            Color fadeColor = fadeBlack.color;

            if (fade)
            {
                fadeBlack.enabled = true;
                while (fadeBlack.color.a < 1)
                {
                    fadeAmount = fadeColor.a + (fadeSpeed * Time.deltaTime);
                    fadeColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, fadeAmount);
                    fadeBlack.color = fadeColor;

                    yield return null;
                }
            }
            else
            {
                while (fadeBlack.color.a > 0)
                {
                    fadeAmount = fadeColor.a - (fadeSpeed * Time.deltaTime);
                    fadeColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, fadeAmount);
                    fadeBlack.color = fadeColor;

                    yield return null;
                }
                fadeBlack.enabled = false;
            }
        }
        else
        {
            Debug.LogError("'fadeBlack' is missing.");
            yield break;
        }
    }
}
