using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

//ATTACH TO MAIN CAMERA, shows your health and coins
public class GUIManager : MonoBehaviour 
{	
	[Header("Main Settings")]
	public WallHazard wallHazard;           //assign the wallhazard script for distance ui
	[Header("Health Settings")]
	public GameObject healthUI;
	[Header("Distance UI Settings")]
	public GameObject distanceUI;
	[Header("Soul UI Settings")]
	public GameObject soulUI;

	private GameObject gameManagerObj;
	private GameManager gameManager;
	private bool isPaused;
	private Health health;
	private Text textUI;
	private double disBetweenTargets;
	private GameObject soulBar;
	private GameObject healthBar;

	//setup, get how many coins are in this level
	private void Start()
	{
		if (GameObject.FindGameObjectWithTag("GameManager") == true)
		{
			gameManagerObj = GameObject.FindGameObjectWithTag("GameManager");
			if (gameManagerObj.GetComponent<GameManager>() == true)
			{
				gameManager = gameManagerObj.GetComponent<GameManager>();
				isPaused = gameManager.GetIsPaused();
			}
			else
				Debug.LogError("The component 'GameManager' does not exist.");
		}
		else
			Debug.LogError("No object with the tag 'GameManager' exists.");
	
		health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
		healthBar = healthUI.GetComponentInChildren<RawImage>().gameObject;
		textUI = distanceUI.GetComponentInChildren<Text>();

		disBetweenTargets = Math.Round(wallHazard.disBetweenTargets, 0);
		textUI.text = disBetweenTargets.ToString() + " m";

		soulBar = soulUI.GetComponentInChildren<RawImage>().gameObject;
	}

    private void Update()
    {
		disBetweenTargets = Math.Round(wallHazard.disBetweenTargets, 0);
		textUI.text = disBetweenTargets.ToString() + " m";

		//If theres a soul bar, change its width based on how many souls you have out of MAXSOULS
		if(soulBar != null)
        {
			float scale = (float)gameManager.soulCount / (float)gameManager.MAXSOULS;
			soulBar.transform.localScale = new Vector3(scale, soulBar.transform.localScale.y);
		}

		if (healthBar != null)
        {
			float scale = (float)health.currentHealth / (float)health.maxHealth;
			healthBar.transform.localScale = new Vector3(scale, healthBar.transform.localScale.y);
		}
	}
}