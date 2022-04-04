using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    // Public Variables
    // Inspector Variables
    public Image fadeBlack;
    public RawImage sceneImage;
    public Text sceneText;
    public float fadeSpeed = 0.5f;
    public float transitionSpeed = 3f;
    public List<Texture> scenes;
    public List<string> text;

    // Private Variables
    // Cutscene Variables
    private bool isValid = true;
    private bool isText = false;
    private string sceneName;
    private float fadeAmount;
    private int index = 0;
    private AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        if (scenes.Count > 0)
        {
            if (fadeBlack != null)
                fadeBlack.GetComponent<AspectRatioFitter>().aspectRatio = scenes[0].width / scenes[0].height;
            else
            {
                isValid = false;
                Debug.LogError("'fadeBlack' is missing.");
            }

            if (sceneImage != null)
                sceneImage.GetComponent<AspectRatioFitter>().aspectRatio = scenes[0].width / scenes[0].height;
            else
            {
                isValid = false;
                Debug.LogError("'sceneImage' is missing.");
            }

            if (sceneImage == null)
            {
                isValid = false;
                Debug.LogError("'sceneImage' is missing.");
            }
        }
        else
        {
            isValid = false;
            Debug.LogError("'scenes' is empty.");
        }

        if (text.Count > 0)
        {
            int count = 0;
            for (int i = 0; i < text.Count; i++)
            {
                if (text[i] == "")
                    count++;
            }

            if (count != text.Count)
            {
                if (isValid && (text.Count == scenes.Count))
                    isText = true;
                else
                    isText = false;
            }
            else
                isText = false;
        }

        sceneName = SceneManager.GetActiveScene().name;

        if (isValid)
        {
            audio = GetComponent<AudioSource>();
            if (audio)
                audio.Play();

            StartCoroutine(StartCutscene());
        }
    }

    private IEnumerator StartCutscene()
    {
        while (index < scenes.Count)
        {
            sceneImage.texture = scenes[index];
            
            if (isText)
                sceneText.text = text[index];
            
            yield return new WaitForSeconds(transitionSpeed);
            yield return StartCoroutine(FadeToBlack(false));
            yield return new WaitForSeconds(transitionSpeed);
            yield return StartCoroutine(FadeToBlack());
            
            yield return null;
            index++;
        }

        yield return new WaitForSeconds(transitionSpeed);

        if (sceneName == "StartCutscene")
            SceneManager.LoadScene("Ronin_Runner");
        else if (sceneName == "EndCutscene")
            SceneManager.LoadScene("CreditsMenu", LoadSceneMode.Single);
    }
    
    private IEnumerator FadeToBlack(bool fade = true)
    {
        Color fadeColor = fadeBlack.color;

        if (fade)
        {
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
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopAllCoroutines();
            if (sceneName == "StartCutscene")
                SceneManager.LoadScene("Ronin_Runner");
            else if (sceneName == "EndCutscene")
                SceneManager.LoadScene("CreditsMenu", LoadSceneMode.Single);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            index++;
            StopAllCoroutines();
            StartCoroutine(StartCutscene());
        }
    }
}
