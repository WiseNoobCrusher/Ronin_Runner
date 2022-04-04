using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class HighJumpEnabler : MonoBehaviour
{
    // Public variables
    public float amountOfOrbs = 1f;
    public RawImage imageUI;

    // Private variables
    private PlayerMovement playerMovement;
    private Text textUI;
    private bool isGrounded;
    private float orbs = 0f;

    // Awake is called before the first frame update
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        if (imageUI.GetComponentInChildren<Text>() == true)
            textUI = imageUI.GetComponentInChildren<Text>();
        else
            Debug.Log("The 'Text' component is missing.");

        CheckJumpIcon();
    }

    // Update is called once per frame
    private void Update()
    {
        CheckJumpIcon();
    }

    private void CheckJumpIcon()
    {
        textUI.text = "Vials: " + orbs;
    }

    // Adds orb to the orb count
    public void AddOrb() { orbs++; CheckJumpIcon(); }

    public void ResetOrbCount()
    {
        orbs -= amountOfOrbs;
        CheckJumpIcon();
    }

    public bool CheckHighJump()
    {
        isGrounded = playerMovement.isGrounded;

        if (orbs >= amountOfOrbs)
        {
            if (!playerMovement.hasDoubleJumped && !isGrounded)
                return false;
            else
                return true;
        }
        else
            return false;
    }
}