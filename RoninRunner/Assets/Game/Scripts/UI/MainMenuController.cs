using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Public Variables
    // Inspector Variables
    // Audio Variables
    public AudioSource theAudio;

    // Menu Variables
    public GameObject titleScreen;
    public GameObject helpScreen;
    public GameObject powerUpScreen;
    public GameObject platformScreen;
    public GameObject hostileScreen;

    private void Start()
    {
        if (!titleScreen.activeSelf)
        {
            titleScreen.SetActive(true);
            helpScreen.SetActive(false);
            powerUpScreen.SetActive(false);
            platformScreen.SetActive(false);
            hostileScreen.SetActive(false);
        }

        if (theAudio.clip != null)
            theAudio.Play();
    }

    // Title Screen
    public void StartGame()
    {
        SceneManager.LoadScene("StartCutscene", LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // Help Menu
    public void HelpMenu_Open()
    {
        titleScreen.SetActive(false);
        helpScreen.SetActive(true);
    }

    public void HelpMenu_Close()
    {
        titleScreen.SetActive(true);
        helpScreen.SetActive(false);
    }

    // PowerUp Menu
    public void PowerUpMenu_Open()
    {
        powerUpScreen.SetActive(true);
        helpScreen.SetActive(false);
    }

    public void PowerUpMenu_Close()
    {
        powerUpScreen.SetActive(false);
        helpScreen.SetActive(true);
    }

    // Platform Menu
    public void PlatformMenu_Open()
    {
        platformScreen.SetActive(true);
        helpScreen.SetActive(false);
    }

    public void PlatformMenu_Close()
    {
        platformScreen.SetActive(false);
        helpScreen.SetActive(true);
    }

    // Hostile Menu
    public void HostileMenu_Open()
    {
        hostileScreen.SetActive(true);
        helpScreen.SetActive(false);
    }

    public void HostileMenu_Close()
    {
        hostileScreen.SetActive(false);
        helpScreen.SetActive(true);
    }
}
