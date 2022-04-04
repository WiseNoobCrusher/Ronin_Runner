using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    // Public Variables
    [Header("Attack Settings")]
    public GameObject attackBounds;
    public float pushForce = 5f;
    public float pushHeight = 3f;
    public int attackDmg = 2;
    public string[] effectedTags;

    // Private Variables
    // GameManager Variables
    private GameObject gameManagerObj;
    private GameManager gameManager;
    private bool isPaused;

    // Misc. Variables
    private DealDamage dealDamage;
    private TriggerParent attackTrigger;

    // Bool Variables
    private bool isValid = true;

    // Start is called before the first frame update
    void Start()
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

        //rigid = GetComponent<Rigidbody>();
        dealDamage = GetComponent<DealDamage>();

        if (attackBounds.GetComponent<TriggerParent>() == true)
            attackTrigger = attackBounds.GetComponent<TriggerParent>();
        else
        {
            isValid = false;
            Debug.LogError("'TriggerParent' has not been added to the Attack Bounds game object.");
        }

        if (effectedTags.Length == 0)
        {
            isValid = false;
            Debug.LogError("'effectedTags' is blank, please add some tags in here.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        isPaused = gameManager.GetIsPaused();

        if (!isPaused)
        {
            if (isValid)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    GetComponentInChildren<Animator>().SetTrigger("Punch");
                    if (attackTrigger.colliding && attackTrigger.hitObject != null)
                    {
                        foreach (var tag in effectedTags)
                        {
                            if (attackTrigger.hitObject.tag == tag)
                                dealDamage.Attack(attackTrigger.hitObject, attackDmg, pushHeight, pushForce);
                        }    
                    }
                }
            }
        }
    }
}
