﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Public Variables
    public GameObject player;
    public GameObject wall;
    public GameObject pauseMenu;

    // Private Variables
    private PlayerMove playerMove;
    private WallHazard wallHazard;
    private bool isPaused = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (player.GetComponent<PlayerMove>() == true)
            playerMove = player.GetComponent<PlayerMove>();
        else
            Debug.LogError("The component, 'PlayerMove', is missing.");

        if (wall.GetComponent<WallHazard>() == true)
            wallHazard = wall.GetComponent<WallHazard>();
        else
            Debug.LogError("The component, 'WallHazard', is missing.");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            isPaused = !isPaused;

        if (isPaused)
        {
            playerMove.SetCanMove(false);
            wallHazard.SetIsPaused(true);
            pauseMenu.SetActive(true);
        }
        else
        {
            playerMove.SetCanMove(true);
            wallHazard.SetIsPaused(false);
            pauseMenu.SetActive(false);
        }
    }

    public void setIsPaused(bool pause)
    {
        isPaused = pause;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}