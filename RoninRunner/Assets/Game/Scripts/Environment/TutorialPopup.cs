using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour
{
    private GameManager gameManager;
    private WallHazard wallHazard;
    //public GameObject tutorialUI;
    //private Text text;
    public string message;  //Tutorial text
    public bool hasBeenActivated = false;  //ONLY USED FOR PICKUPS

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (gameManager == null)
            Debug.LogError("gameManager not found");

        wallHazard = GameObject.FindGameObjectWithTag("Hazard").GetComponent<WallHazard>();

        //if (!(text = gameObject.GetComponent<Text>()))
            //Debug.LogError("No text component");

        gameManager.tutorialUI.SetActive(false);
        //gameManager.IsTutorial = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        if (other.gameObject.tag == "Player" && !gameManager.IsTutorial && !hasBeenActivated)
        {
            gameManager.tutorial = this;
            gameManager.currentTutorial = gameObject;
            var p1 = wallHazard._particleSystem;

            p1.Pause();
            gameManager.tutorialUI.GetComponentInChildren<Text>().text = message + "\n\rPRESS LEFT MOUSE TO RESUME";
            gameManager.tutorialUI.SetActive(true);
            gameManager.IsTutorial = true;
            gameManager.timeManager.SetGlobalTimeScale(0);
            gameManager.timeManager.SetPlayerTimeScale(0);

            //Debug.Log(gameManager.IsTutorial);

            for (int i = 0; i < gameManager.pickups.Length; i++)
            {
                gameManager.pickups[i].tempRotation = gameManager.pickups[i].rotation;
                gameManager.pickups[i].tempStartSpeed = gameManager.pickups[i].startSpeed;

                gameManager.pickups[i].rotation = new Vector3(0, 0, 0);
                gameManager.pickups[i].startSpeed = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {  
        //Debug.Log(gameManager.IsTutorial);
        /*
        if(gameManager.IsTutorial && (Input.GetKeyDown(KeyCode.Mouse0)))
        {
            if (gameObject.tag != "Pickup" && gameObject == gameManager.currentTutorial)
                gameObject.SetActive(false);
            else if (gameObject.tag == "Pickup" && gameObject == gameManager.currentTutorial)
                hasBeenActivated = true;

            var p1 = wallHazard._particleSystem;

            p1.Play();

            gameManager.tutorialUI.SetActive(false);
            gameManager.IsTutorial = false;
            gameManager.timeManager.SetPlayerTimeScale(1);
            gameManager.timeManager.SetGlobalTimeScale(1);

            for (int i = 0; i < gameManager.pickups.Length; i++)
            {
                gameManager.pickups[i].rotation = gameManager.pickups[i].tempRotation;
                gameManager.pickups[i].startSpeed = gameManager.pickups[i].tempStartSpeed;
            }
            gameManager.currentTutorial = null;
        }
        */
    }
}
