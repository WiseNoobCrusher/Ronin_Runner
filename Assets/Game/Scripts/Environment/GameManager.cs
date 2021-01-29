﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Public Variables
    // Game Object variables
    public GameObject player;
    public GameObject wall;
    public GameObject pauseMenu;

    // Private Variables
    // Component variables
    private PlayerMove playerMove;
    private WallHazard wallHazard;

    // Paused variables
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
        // Checks if the 'Escape' key was pressed
        if (Input.GetKeyUp(KeyCode.Escape))
            isPaused = !isPaused;

        // Checks if the game is paused
        if (isPaused)
        {
            // Stops the player, the wall and opens the pause menu
            playerMove.SetCanMove(false);
            wallHazard.SetIsPaused(true);
            pauseMenu.SetActive(true);
        }
        else
        {
            // Let the player and wall move and closes the pause menu
            playerMove.SetCanMove(true);
            wallHazard.SetIsPaused(false);
            pauseMenu.SetActive(false);
        }
    }

    // This sets the 'isPaused' variable
    public void setIsPaused(bool pause)
    {
        isPaused = pause;
    }

    // This exits the game
    public void ExitGame()
    {
        Application.Quit();
    }
}