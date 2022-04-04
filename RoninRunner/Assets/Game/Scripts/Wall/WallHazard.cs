using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DealDamage))]
[RequireComponent(typeof(AudioSource))]

public class WallHazard : MonoBehaviour
{
    // Public Variables
    // Inspector Variables
    // Wall Variables
    public float graceDistance = 146f;
    public float initialSpeed = 3f;
    public float damage = 2f;
    public float damageInterval = 3f;
    public float pushHeight = 2f;
    public float pushForce = 2f;
    public string[] effectedTags;

    // Non-Inspector Variables
    // Wall Variables
    [HideInInspector]
    public ParticleSystem _particleSystem;
    [HideInInspector]
    public float speed = 0f;
    [HideInInspector]
    public float disBetweenTargets = 0f;

    // StopTheWall Variables
    [HideInInspector]
    public bool isStopped = false;

    // SlowDownTheWall Variables
    [HideInInspector]
    public bool isSlowed = false;

    // Private Variables
    // GameManager Variables
    private GameObject gameManagerObj;
    private GameManager gameManager;
    private bool isPaused;
    private bool isEnding;

    // Time Variables
    private float startTime = 0f;
    private float endTime = 0f;
    private float currTime = 0f;
    private float elapsedTime = 0f;

    // Wall Variables
    private bool canStart = false;
    private GameObject target;
    private bool isAlreadyHit = false;
    private float tempSpeed = 0f;
    private float nonEditedSpeed = 0f;

    // StopTheWall Variables
    private float stopTime = 0f;
    private float stopStartTime = 0f;
    private float stopElapsedTime = 0f;

    // SlowDownTheWall Variables
    private float slowTime = 0f;
    private float slowAmount = 0f;
    private float slowStartTime = 0f;
    private float slowElapsedTime = 0f;

    // Component Variables
    private WallSpeedHandler speedHandler;
    private DealDamage dealDamage;


    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("GameManager") == true)
        {
            gameManagerObj = GameObject.FindGameObjectWithTag("GameManager");
            if (gameManagerObj.GetComponent<GameManager>() == true)
            {
                gameManager = gameManagerObj.GetComponent<GameManager>();
                isPaused = gameManager.GetIsPaused();
                isEnding = gameManager.IsEnding;
            }
            else
                Debug.LogError("The component 'GameManager' does not exist.");
        }
        else
            Debug.LogError("No object with the tag 'GameManager' exists.");

        _particleSystem = GetComponentInChildren<ParticleSystem>();
        speedHandler = (speedHandler != null) ? GetComponent<WallSpeedHandler>() : null;
        dealDamage = GetComponent<DealDamage>();

        speed = initialSpeed;
        nonEditedSpeed = initialSpeed;

        endTime = Time.deltaTime;
        startTime = endTime;
    }

    private void Update()
    {
        isPaused = gameManager.GetIsPaused();
        isEnding = gameManager.IsEnding;

        var temp = GameObject.FindGameObjectWithTag("Player").transform;
        disBetweenTargets = (float)Math.Round(temp.position.x - (transform.position.x + (transform.localScale.x / 2)) - 1, 0);

        if (!canStart)
            if (disBetweenTargets >= graceDistance)
                canStart = true;

        if (isStopped)
        {
            speed = 0;

            stopElapsedTime = (float)Math.Round(endTime - stopStartTime, 0);
            if (stopElapsedTime != 0 && stopElapsedTime % stopTime == 0)
            {
                isStopped = false;

                if (speedHandler != null && speedHandler.withinTrigger)
                    speed = nonEditedSpeed;
                else
                    speed = tempSpeed;

                var _ps = _particleSystem.main;
                _ps.startColor = Color.black;
            }
        }

        if (isSlowed)
        {
            speed = tempSpeed / slowAmount * (Time.deltaTime * gameManager.timeManager.GetGlobalTimeScale());

            slowElapsedTime = (float)Math.Round(endTime - slowStartTime, 0);
            if (slowElapsedTime != 0 && slowElapsedTime % slowTime == 0)
            {
                isSlowed = false;
                if (speedHandler != null && speedHandler.withinTrigger)
                {
                    if (speedHandler != null && speedHandler.withinTrigger)
                        speed = nonEditedSpeed;
                    else
                        speed = tempSpeed;
                }
                else
                    speed = tempSpeed;

                var _ps = _particleSystem.main;
                _ps.startColor = Color.black;
            }
        }

        if (!isPaused && !isEnding && canStart)
        {
            if (isStopped)
                speed = 0;

            if (isSlowed)
                speed = tempSpeed / slowAmount * (Time.deltaTime * gameManager.timeManager.GetGlobalTimeScale());

            var temp2 = GameObject.FindGameObjectWithTag("Player").transform;
            disBetweenTargets = (float)Math.Round(temp2.position.x - (transform.position.x + (transform.localScale.x / 2)), 0);

            Vector3 wallPos = transform.position;
            wallPos.x += speed * (Time.deltaTime * gameManager.timeManager.GetGlobalTimeScale());
            transform.position = wallPos;

            if (speedHandler != null && !speedHandler.withinTrigger && !isStopped && !isSlowed)
                nonEditedSpeed = speed;
        }

        endTime += Time.deltaTime;
    }

    public void StopTheWall(float time)
    {
        isStopped = true;
        speed = 0;

        stopTime = time;
        stopStartTime = endTime;

        if (speedHandler != null && speedHandler.withinTrigger)
            tempSpeed = nonEditedSpeed;
        else if (speed == 0)
            tempSpeed = initialSpeed;
        else
            tempSpeed = speed;

        var _ps = _particleSystem.main;
        _ps.startColor = Color.yellow;
    }

    public void SlowDownTheWall(float amount, float time)
    {
        isSlowed = true;

        slowTime = time;
        slowAmount = amount;
        slowStartTime = endTime;

        if ((speedHandler != null && speedHandler.withinTrigger) || speed == 0)
            tempSpeed = nonEditedSpeed;
        else
            tempSpeed = speed;

        speed = tempSpeed / amount * (Time.deltaTime * gameManager.timeManager.GetGlobalTimeScale());

        var _ps = _particleSystem.main;
        _ps.startColor = Color.blue;
    }

    public void MoveTheWall(float newX)
    {
        foreach (var tag in effectedTags)
        {
            GameObject temp = GameObject.FindGameObjectWithTag(tag);
            if (temp.tag == "Player")
            {
                if (newX < temp.transform.position.x)
                {
                    var pos = transform.position;
                    pos.x = newX;

                    transform.position = pos;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!isPaused && !gameManager.IsTutorial)
        {
            foreach (var tag in effectedTags)
            {
                if (other.tag == tag)
                {
                    target = other.gameObject;

                    if (tag == "Player")
                    {
                        if (!isAlreadyHit)
                            dealDamage.Attack(target, (int)damage, 0, 0);

                        isAlreadyHit = true;

                        startTime = endTime;
                    }
                    else if (tag == "Enemy")
                        dealDamage.Attack(target, target.GetComponent<Health>().currentHealth, 0, 0);
                    else
                        target = null;

                    break;
                }
                else
                    target = null;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isPaused && !gameManager.IsTutorial)
        {
            foreach (var tag in effectedTags)
            {
                if (other.tag == tag)
                {
                    if (tag == "Player")
                    {
                        elapsedTime = (float)Math.Round(endTime - startTime, 0);
                        if (elapsedTime != currTime && elapsedTime % damageInterval == 0)
                        {
                            dealDamage.Attack(target, (int)damage, 0, 0);
                            currTime = elapsedTime;
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        target = null;
        isAlreadyHit = false;
        startTime = 0;
    }
}