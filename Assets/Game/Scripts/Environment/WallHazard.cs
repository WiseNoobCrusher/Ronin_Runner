using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DealDamage))]
[RequireComponent(typeof(AudioSource))]

public class WallHazard : MonoBehaviour
{
    ParticleSystem _particleSystem;

    [SerializeField]
    private float gracePeriod;
    [SerializeField]
    private float damagePeriod;
    [SerializeField]
    private float initialSpeed;
    [SerializeField]
    private int damage;
    [SerializeField]
    private AudioClip hitSound;
    [SerializeField]
    private string[] effectedTags;

    // Misc. variables  
    private DealDamage dealDamage;
    private AudioSource aSource;
    private GameObject target = null;

    // Move-Related variables
    private bool canStart = true;
    private bool startMove = false;
    private bool isTriggerOn = false;
    private float moveSpeed;

    // Time variables
    private float startTime;
    private float endTime;
    private float tempSec = 0;

    // Elapsed Times variables
    private float elapsedTime;
    private float stopElapsedTime;
    private float slowElapsedTime;

    // StopTheWall variables
    private bool isStopped = false;
    private float stopStartTime;
    private float stopTime;

    // SlowDownTheWall variables
    private bool isSlowed = false;
    private float tempSpeed;
    private float slowTime;
    private float slowStartTime;

    // Start is called before the first frame update
    private void Start()
    {
        dealDamage = GetComponent<DealDamage>();
        aSource = GetComponent<AudioSource>();
        startTime = Time.time;
        moveSpeed = initialSpeed * Time.deltaTime;

        target = GameObject.FindGameObjectWithTag("Player");

        // Checks if the component 'ParticleSystem' exists within the child object
        if (GetComponentInChildren<ParticleSystem>() == false)
        {
            canStart = false;
            Debug.LogError("Particle System for the wall is missing.");
        }
        else
            _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    private void Update()
    {
        // If the component 'ParticleSystem' wasn't found, this would be false
        if (canStart)
        {
            // Checks if the wall can start moving or not
            if (!startMove)
            {
                // Checks if the amount of time has passed
                endTime += Time.deltaTime;
                elapsedTime = endTime - startTime;
                if (elapsedTime >= gracePeriod)
                {
                    // Allows the wall to start moving
                    startMove = true;
                    startTime = endTime;
                }
            }
            else
            {
                // Checks if the wall can move (This is for the StopTheWall function)
                if (!isStopped)
                {
                    // Checks if the x value of the wall and player match
                    bool isValid = true;
                    if (target != null)
                        if (transform.localPosition.x >= target.transform.localPosition.x)
                            isValid = false;

                    // If the x value of the wall and player match, this would be false
                    if (isValid)
                    {
                        // Moves the wall
                        var sh = gameObject.transform;
                        Vector3 objectPos = gameObject.transform.localPosition;

                        objectPos.x += moveSpeed;
                        sh.localPosition = objectPos;
                    }

                    // Checks if the wall has been slowed down (This is for the SlowDownTheWall function)
                    if (isSlowed)
                    {
                        // Checks if the allocated amount of time has passed
                        slowElapsedTime = float.Parse(Math.Round(slowStartTime - endTime).ToString());
                        if (slowElapsedTime != 0)
                        {
                            if (slowElapsedTime % slowTime == 0)
                            {
                                // Sets the wall back to regular speed
                                moveSpeed = tempSpeed;
                                isSlowed = false;
                            }
                        }
                    }
                }
                else
                {
                    // Checks if the allocated amount of time has passed
                    stopElapsedTime = float.Parse(Math.Round(stopStartTime - endTime).ToString());
                    if (stopElapsedTime != 0)
                    {
                        if (stopElapsedTime % stopTime == 0)
                        {
                            // Allows the wall to move again
                            var ps = _particleSystem.main;
                            ps.startColor = Color.black;

                            isStopped = false;
                        }
                    }
                }
            }

            /* Checks if the boolean that determines if the player has 
               collided with the wall is true or false */
            if (isTriggerOn)
            {
                // Checks if target is null
                elapsedTime = float.Parse(Math.Round(endTime - startTime, 0).ToString());
                Debug.Log(elapsedTime.ToString());
                if (target != null)
                {
                    // Checks if 'tempSec' is 0 or not
                    // 'tempSec' is used to try to stop the player from being damaged twice
                    if (tempSec == 0)
                    {
                        // Checks if the allocated time has passed
                        if (elapsedTime % damagePeriod == 0)
                        {
                            // Sets tempSec and damage the player
                            tempSec = elapsedTime;
                            dealDamage.Attack(target, damage, 0, 0);
                        }
                    }
                    else
                    {
                        /* Checks if the allocated time has passed and if
                           'elapsedTime' does not equal 'tempSec' */
                        if (elapsedTime != tempSec)
                        {
                            if (elapsedTime % damagePeriod == 0)
                            {
                                tempSec = elapsedTime;
                                dealDamage.Attack(target, damage, 0, 0);
                            }
                        }
                    }
                }
                else
                    Debug.LogError("Target is missing.");
            }
        }

        // Sets the target to the player object and updates 'endTime'
        target = GameObject.FindGameObjectWithTag("Player");
        endTime += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        /* Goes through 'effectedTags' to check if the tag matches
           the object that collided */
        foreach (string tag in effectedTags)
        {
            if (other.gameObject.tag == tag)
            {
                /* Sets target to the object that collided, sets 'startTime'
                   and sets 'isTriggerOn' */
                target = other.gameObject;
                startTime = endTime;
                isTriggerOn = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        /* Goes through 'effectedTags' to check if the tag matches
           the object that collided */
        foreach (string tag in effectedTags)
        {
            if (other.gameObject.tag == tag)
                // Sets target to the object that collided
                target = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Sets 'target' to null, 'isTriggerOn' to false and 'tempSec' to 0
        target = null;
        isTriggerOn = false;
        tempSec = 0;
    }

    public void MoveTheWall(float newX)
    {
        // Moves the wall by the amount given if the given value of x is less or equal to the target's
        if (newX <= target.transform.localPosition.x)
        {
            var sh = gameObject.transform;
            Vector3 objectPos = gameObject.transform.localPosition;

            objectPos.x = newX;
            sh.localPosition = objectPos;
        }
        else
            Debug.LogError("The value of X given is higher the the value of X of the intended target. Please enter a value of X that is less than the target's.");
    }

    public void StopTheWall(float time)
    {
        // Stops the wall for a given amount of time
        var ps = _particleSystem.main;
        ps.startColor = Color.yellow;

        isStopped = true;
        stopTime = time;
        stopStartTime = endTime;
    }

    public void SlowDownTheWall(float newSpeed, float time)
    {
        // Checks if the given speed is slower than the current speed
        if ((newSpeed * Time.deltaTime) < moveSpeed)
        {
            // Slows down the wall by a given amount for a given amount of time
            tempSpeed = moveSpeed;
            moveSpeed = newSpeed * Time.deltaTime;
            isSlowed = true;
            slowTime = time;
            slowStartTime = endTime;
        }
        else
            Debug.LogError("The speed you provided in 'SlowDownTheWall' is faster than the speed provided in the component 'WallHazard'. Please choose a smaller number.");
    }

    public float GetStopElapsedTime()
    {
        // Gets the 'stopElapsedTime'
        float output = float.Parse(Math.Round(stopStartTime - endTime).ToString());
        return output;
    }

    public float GetSlowElapsedTime()
    {
        // Gets the 'slowElapsedTime'
        float output = float.Parse(Math.Round(slowStartTime - endTime).ToString());
        return output;
    }

    public void SetSpeed(float newSpeed)
    {
        // Sets the new speed
        moveSpeed = newSpeed;
    }
}
