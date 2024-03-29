﻿using UnityEngine;
using System;
using System.Collections;

//attach to any object in the game which takes damage (player, enemies, breakable crates, smashable windows..)
[RequireComponent(typeof(AudioSource))]
public class Health : MonoBehaviour 
{
	public AudioClip impactSound;					//play when object imacts with something else
	public AudioClip hurtSound;						//play when this object recieves damage
	public AudioClip deadSound;						//play when this object dies
	public int currentHealth = 1;					//health of the object
	public bool takeImpactDmg;						//does this object take damage from impacts?
	public bool onlyRigidbodyImpact;				//if yes to the above, does it only take impact damage from other rigidbodies?
	public bool respawn;                            //should this object respawn when killed?
	public string[] impactFilterTag;				//if we take impact damage, don't take impact damage from these objects (tags)
	public float hitFlashDelay = 0.1f;				//how long each flash lasts (smaller number = more rapid flashing)
	public float flashDuration = 0.9f;				//how long flash lasts (object is invulnerable to damage during this time)
	public Color hitFlashColor = Color.red;			//color object should flash when it takes damage
	public Transform flashObject;					//object to flash upon receiving damage (ie: a child mesh). If left blank it defaults to this object.
	public GameObject[] spawnOnDeath;				//objects to spawn upon death of this object (ie: a particle effect or a coin)
	
	[HideInInspector]
	public bool dead, flashing;
	[HideInInspector]
	public Vector3 respawnPos;
	[HideInInspector]
	public int maxHealth;

	private Color originalColor;
	private int defHealth, h, hitForce;
	private bool hitColor = false;
	private float nextFlash, stopFlashTime;
	private Renderer flashRender;
	private AudioSource aSource;
	private GameObject deathWall;
	private WallHazard wallHazard;
	private GameManager gameManager;

    //setup
    void Awake()
	{
		maxHealth = currentHealth;
		aSource = GetComponent<AudioSource>();
		if(currentHealth <= 0)
			Debug.LogWarning(transform.name + " has 'currentHealth' set to 0 or less in 'Health' script: it has died upon scene start");
		aSource.playOnAwake = false;
		if(flashObject == null)
			flashObject = transform;
		flashRender = flashObject.GetComponent<Renderer>();
		originalColor = flashRender.material.color;
		defHealth = currentHealth;
		respawnPos = transform.position;

		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		if (gameManager == null)
			Debug.LogError("gameManager not found");

		if ((deathWall == null) && (GameObject.FindGameObjectWithTag("Hazard") != null))
		{
			deathWall = GameObject.FindGameObjectWithTag("Hazard");
			if ((wallHazard == null) && (deathWall.GetComponent<WallHazard>() != null))
				wallHazard = deathWall.GetComponent<WallHazard>();
			else
				Debug.LogError("Wall Hazard script is missing");
		}
		else
			Debug.LogError("No object was found.");
	}
	
	//detecting damage and dying
	void Update()
	{		
		//flash if we took damage
		if (currentHealth < h)
		{
			flashing = true;
			stopFlashTime = Time.time + flashDuration;
			if (hurtSound)
				AudioSource.PlayClipAtPoint(hurtSound, transform.position);
		}
		h = currentHealth;
		
		//flashing
		if (flashing)
		{
			Flash ();
			if (Time.time > stopFlashTime)
			{
				flashRender.material.color = originalColor;
				flashing = false;
			}
		}
		
		//are we dead?
		dead = (currentHealth <= 0) ? true : false;
		if (dead)
			Death();
	}
	
	//toggle the flashObject material tint color
	void Flash()
	{
		flashRender.material.color = (hitColor) ? hitFlashColor : originalColor;
		if(Time.time > nextFlash)
		{
			hitColor = !hitColor;
			nextFlash = Time.time + hitFlashDelay;
		}
	}
	
	//respawn object, or destroy it and create the SpawnOnDeath objects
	void Death()
	{
		if (deadSound)
			AudioSource.PlayClipAtPoint(deadSound, transform.position);
		flashing = false;
		flashObject.GetComponent<Renderer>().material.color = originalColor;
		WallSpeedHandler speedHandler = GameObject.FindGameObjectWithTag("Hazard").GetComponent<WallSpeedHandler>();
		if (tag == "Player" && speedHandler.withinTrigger)
			speedHandler.withinTrigger = false;
		if(respawn)
		{
			Rigidbody rigid = GetComponent<Rigidbody>();
			if(rigid)
				rigid.velocity *= 0;
			transform.position = respawnPos;
			dead = false;
			currentHealth = defHealth;
			if (wallHazard != null && tag == "Player")
            {
				if (respawnPos.x - deathWall.transform.localPosition.x <= 55)
					wallHazard.MoveTheWall(respawnPos.x - 55);
				gameManager.SlowMode(false);
            }
			else
				Debug.LogError("Wall Hazard is missing. But the real question is... how did you even get here with this error?");
			StartCoroutine(SpawnPickups());
			gameManager.RespawnEnemies();
		}
		else
			Destroy (gameObject);

		if (tag == "Enemy")
        {
			for (int i = 0; i < gameManager.enemies.Count; i++)
            {
				if (gameManager.enemies[i] == gameObject.GetComponent<EnemyController>())
					gameManager.enemiesDestroyed[i] = true;
            }
        }
		
		if (spawnOnDeath.Length != 0)
			foreach(GameObject obj in spawnOnDeath)
            {
				Instantiate(obj, transform.position + new Vector3(0, 2, 0), Quaternion.Euler(Vector3.zero));
			}
	}
	
	//calculate impact damage on collision
	void OnCollisionEnter(Collision col)
	{
		if(!aSource.isPlaying && impactSound)
		{
			aSource.clip = impactSound;
			aSource.volume = col.relativeVelocity.magnitude/30;
			aSource.Play();
		}
			
		//make sure we take impact damage from this object
		if (!takeImpactDmg)
			return;
		foreach(string tag in impactFilterTag)			
			if(col.transform.tag == tag)
				return;
		if(onlyRigidbodyImpact && !col.rigidbody)
			return;
		
		//calculate damage
		if(col.rigidbody)
			hitForce = (int)(col.rigidbody.velocity.magnitude/4 * col.rigidbody.mass);
		else
			hitForce = (int)col.relativeVelocity.magnitude/6;
		currentHealth -= hitForce;
		//print (transform.name + " took: " + hitForce + " dmg in collision with " + col.transform.name);
	}

	public void AddHealth(int health)
    {
		if (health < maxHealth)
        {
			if (currentHealth < maxHealth)
			{
				if (currentHealth + health < maxHealth)
					currentHealth += health;
				else
					currentHealth = maxHealth;
			}
		}
	}

	private IEnumerator SpawnPickups()
    {
		yield return new WaitForSeconds(0.01f);
		if (tag == "Player")
			gameManager.RespawnPickups();
		yield break;
	}

	private IEnumerator SpawnEnemies()
    {
		yield return new WaitForSeconds(0.01f);
		// Insert Respawn Enemy function call here
		yield break;
    }
}

// NOTE: if you just want an object to play impact sounds, give it this script, but uncheck for impact damage