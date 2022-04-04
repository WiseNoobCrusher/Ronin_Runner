using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(DealDamage))]
public class EnemyController : MonoBehaviour
{
    // Public Variables
    // Inspector Variables
    // Float Variables
    [Header("Main Settings")]
    public float speed = 0.2f;
    public float damage = 2f;
    public float turnSmoothTime = 0.1f;
    public float groundDistance = 0.4f;
    public float pushHeight = 4f;
    public float pushForce = 4f;

    // LayerMask Variables
    public LayerMask groundMask;

    // GameObject Variables
    public TriggerParent sightBounds;
    public TriggerParent attackBounds;
    public TriggerParent headCheck;
    public GameObject[] bounds;

    // Transform Variables
    public Transform ground;

    // String Variables
    public string[] effectedTags;

    // Private Variables
    // GameManager Variables
    private GameObject gameManagerObj;
    private GameManager gameManager;
    private bool isPaused;
    private bool isEnding;

    // Float Variables
    private float distance;
    private float disOffset;
    private float gravity = -9.81f;
    private float tempSpeed = -1f;
    private float turnSmoothVelocity;

    // Enum Variables
    private enum EnemyStates { PATROL, CHASE };

    // EnemyStates Variables
    private EnemyStates currState;

    // Bool Variables
    private bool isValid = true;
    private bool offsetFound = false;
    private bool isGrounded = true;
    private bool isOnRotatePlatform = false;

    // Vector3 Variables
    private Vector3 move;
    private Vector3 velocity;
    private Vector3 direction;
    private Vector3 prevPos;

    // GameObject Variables
    private GameObject currBound;
    private GameObject platObject;
    private GameObject target = null;

    // Component Variables
    private CharacterController controller;
    private DealDamage dealDamage;
    private Animator animator;

    // Time Variables
    private float endTime = 0f;
    private float currTime = 0f;
    private float elapsedTime = 0f;

    // Start is called before the first frame update
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

        if (bounds.Length < 1)
        {
            isValid = false;
            Debug.LogError("'bounds' is not valid.");
        }
        else
            currBound = bounds[0];

        for (int i = 0; i < bounds.Length; i++)
            bounds[i].transform.SetParent(null);
        sightBounds.transform.SetParent(null);

        controller = GetComponent<CharacterController>();
        dealDamage = GetComponent<DealDamage>();
        animator = GetComponentInChildren<Animator>();

        currState = EnemyStates.PATROL;

        tempSpeed = animator.speed;

        endTime = Time.deltaTime;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        isPaused = gameManager.GetIsPaused();
        isEnding = gameManager.IsEnding;

        if (!isPaused && !isEnding && !gameManager.IsTutorial)
        {
            if (isValid)
            {
                if (animator)
                    animator.speed = 1f;

                isGrounded = Physics.CheckSphere(ground.position, groundDistance, groundMask);

                if (isGrounded && velocity.y < 0)
                    velocity.y = -2f;

                if (transform.parent != null)
                {
                    if (transform.parent.name == "Plat 1" || transform.parent.name == "Plat 2" || transform.parent.name == "Plat 3" || transform.parent.name == "Plat 4")
                    {
                        platObject = transform.parent.gameObject;
                        isOnRotatePlatform = true;
                    }
                    else
                        isOnRotatePlatform = false;
                }

                if (isOnRotatePlatform)
                {
                    for (int i = 0; i < bounds.Length; i++)
                        bounds[i].transform.SetParent(platObject.transform);
                    sightBounds.transform.SetParent(platObject.transform);
                }
                else
                {
                    for (int i = 0; i < bounds.Length; i++)
                        bounds[i].transform.SetParent(null);
                    sightBounds.transform.SetParent(null);
                }

                // Check Direction
                if (prevPos != transform.position)
                {
                    direction = (prevPos - transform.position).normalized;
                    prevPos = transform.position;
                }

                // Checks sight bounds
                if (sightBounds.colliding && sightBounds.hitObject != null)
                    currState = EnemyStates.CHASE;
                else
                    currState = EnemyStates.PATROL;

                // Checks which state the enemy is in
                if (currState == EnemyStates.PATROL)
                {
                    distance = Vector3.Distance(transform.position, currBound.transform.position);

                    if (distance > 0.9601873f)
                    {
                        Vector3 relativePos = new Vector3(currBound.transform.position.x, transform.position.y, 0) - new Vector3(transform.position.x, transform.position.y, 0);
                        Quaternion rotation = Quaternion.LookRotation(relativePos);

                        transform.position = Vector3.MoveTowards(transform.position, currBound.transform.position, speed * (Time.fixedDeltaTime * gameManager.timeManager.GetGlobalTimeScale()));
                        GetComponentInChildren<Animator>().transform.rotation = Quaternion.Lerp(GetComponentInChildren<Animator>().transform.rotation, rotation, speed * (Time.fixedDeltaTime * gameManager.timeManager.GetGlobalTimeScale()));
                    }
                    else
                    {
                        if (currBound == bounds[0])
                            currBound = bounds[1];
                        else
                            currBound = bounds[0];
                    }

                    if (animator)
                    {
                        animator.SetFloat("DistanceToTarget", distance);
                        animator.SetBool("Grounded", true);
                        animator.SetFloat("YVelocity", (float)Math.Round(velocity.y, 0));

                        if (gameManager.isSlowMode)
                            animator.speed = tempSpeed / gameManager.slowMultiplier;
                        else
                            animator.speed = tempSpeed;
                    }
                }
                else
                {
                    distance = Vector3.Distance(transform.position, sightBounds.hitObject.transform.position);

                    if (attackBounds.colliding && attackBounds.hitObject != null)
                    {
                        distance = Vector3.Distance(transform.position, attackBounds.hitObject.transform.position);

                        elapsedTime = (float)Math.Round(endTime - 0, 0);
                        if (elapsedTime != currTime && elapsedTime % 3 == 0)
                        {
                            dealDamage.Attack(attackBounds.hitObject, (int)damage, 0, 0);
                            animator.SetTrigger("Punch");
                            currTime = elapsedTime;
                        }
                    }

                    if (distance > 0.01f)
                    {
                        Vector3 relativePos = new Vector3(sightBounds.hitObject.transform.position.x, 0, 0) - new Vector3(transform.position.x, 0, 0);
                        Quaternion rotation = Quaternion.LookRotation(relativePos);

                        transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(sightBounds.hitObject.transform.position.x, transform.position.y, 0), speed * (Time.fixedDeltaTime * gameManager.timeManager.GetGlobalTimeScale()));
                        GetComponentInChildren<Animator>().transform.rotation = Quaternion.Lerp(GetComponentInChildren<Animator>().transform.rotation, rotation, speed * (Time.fixedDeltaTime * gameManager.timeManager.GetGlobalTimeScale()));
                    }

                    if (animator)
                    {
                        animator.SetFloat("DistanceToTarget", distance);
                        animator.SetBool("Grounded", true);
                        animator.SetFloat("YVelocity", (float)Math.Round(velocity.y, 0));
                    }
                }

                velocity.y += gravity * (Time.deltaTime * gameManager.timeManager.GetGlobalTimeScale());
                controller.Move(velocity * (Time.deltaTime * gameManager.timeManager.GetGlobalTimeScale()));

                endTime += Time.deltaTime;
            }
        }
        else
        {
            if (animator)
                animator.speed = 0f;
        }
    }
}
