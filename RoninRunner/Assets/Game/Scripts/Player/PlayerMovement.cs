using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // ---------------------------------------------------------
    // Public Variables
    // ---------------------------------------------------------
    // Inspector Variables
    // AudioClip Variables
    [Header("Main Settings")]
    public AudioClip jumpSound;
    public AudioClip landSound;

    // Move Variables
    public float initialSpeed = 6f;
    public float speedInterval = 3f;
    public float speedIncrease = 2f;
    public float maxSpeed = 25f;
    public float initialGravity = -9.81f;
    public float turnSmoothTime = 0.1f;
    public float waterForce = 2f;
    public string[] effectedPlatformTags;
    
    // Jump Variables
    public float initialJumpHeight = 4f;
    public float jumpMultiplier = 2f;

    // Wall Jump Variables
    public float gravitySubstracter = 2f;

    // Ground Variables
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Transform ground;

    // Non Inspector Variables
    // Move Variables
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public float gravity = -9.81f;
    [HideInInspector]
    public float faceDirection = 0f;
    [HideInInspector]
    public bool isMoving = false;
    [HideInInspector]
    public Vector3 velocity;

    // Jump Variables
    [HideInInspector]
    public bool hasDoubleJumped = false;
    [HideInInspector]
    private bool canHighJump = false;

    // Wall Jump Variables
    [HideInInspector]
    public GameObject currWall = null;

    // Ground Variables
    [HideInInspector]
    public bool isGrounded = false;

    // Platform Variables
    [HideInInspector]
    public bool isOnWallPlat = false;
    [HideInInspector]
    public bool onMovingPlatform = false;

    // ---------------------------------------------------------
    // Private Variables
    // ---------------------------------------------------------
    // GameManager Variables
    private GameObject gameManagerObj;
    private GameManager gameManager;
    private bool isPaused;
    private bool isTutorial;

    // Time Variables
    private float startTime = 0f;
    private float endTime = 0f;
    private float currSecOne = 0f;
    private float currSecTwo = 0f;
    private float elapsedTime = 0f;

    // Move Variables
    private float turnSmoothVelocityOne;
    private float turnSmoothVelocityTwo;
    private float disBetweenTargets;
    private bool isInWater = false;
    private Transform target = null;

    // Jump Variables
    private float jumpHeight;
    private bool hasJumped = false;
    private bool hasHighJumped = false;
    private bool landSoundPlayed = true;
    
    // Component Variables
    private CharacterController controller;
    private Animator animator;
    private DealDamage dealDamage;
    private WallHazard wallHazard;
    private HighJumpEnabler highJumpEnabler;

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
                isTutorial = gameManager.IsTutorial;
            }
            else
                Debug.LogError("The component 'GameManager' does not exist.");
        }
        else
            Debug.LogError("No object with the tag 'GameManager' exists.");

        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        dealDamage = GetComponent<DealDamage>();

        if (GameObject.FindGameObjectWithTag("Hazard").GetComponent<WallHazard>() == true)
            wallHazard = GameObject.FindGameObjectWithTag("Hazard").GetComponent<WallHazard>();
        else
            Debug.LogError("The 'WallHazard' component is non existant, or the object is non existant.");

        if (GetComponent<HighJumpEnabler>() == true)
            highJumpEnabler = GetComponent<HighJumpEnabler>();
        else
            Debug.LogError("'HighJumpEnabler' component is missing.");

        speed = initialSpeed;
        jumpHeight = initialJumpHeight;

        endTime = Time.deltaTime;
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, 0), 999 * Time.deltaTime);

        var temp = transform.position.y;
        if (isInWater)
        {
            temp = Mathf.MoveTowards(transform.position.y, target.position.y, waterForce * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, temp, transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        isPaused = gameManager.GetIsPaused();
        isTutorial = gameManager.IsTutorial;

        if (!isPaused && !isTutorial)
        {
            float posOne = transform.position.x;
            float posTwo = 0f;
            float distance = 0f;

            if (animator)
                animator.speed = 1f;

            // Get movement variable
            float h = Input.GetAxis("Horizontal");
            faceDirection = h;
            // Checks if the player is on the ground
            isGrounded = Physics.CheckSphere(ground.position, groundDistance, groundMask);

            // Checks if the player has moved at all
            if (h > 0 || h < 0)
            {
                // If so, checks if it is false
                if (!isMoving)
                {
                    // If it is, set it to true and set 'startTime'
                    isMoving = true;
                    startTime = endTime;
                }
            }
            else
            {
                // If not, sets it to false, sets 'speed' back to 'initial speed' and 'tempSec' to 0
                isMoving = false;
                speed = initialSpeed;
                currSecOne = 0;
            }

            // Checks if 'hasMoved' is true
            if (isMoving)
            {
                /* If so calculates 'elapsedTime' and checks if it is not zero, if it isn't equal to 'tempSec'
                 * and if it has been a certain amount of time given by 'speedInterval' */
                elapsedTime = float.Parse(Math.Round(endTime - startTime, 0).ToString());
                if (elapsedTime != 0 && elapsedTime != currSecOne && elapsedTime % speedInterval == 0)
                {
                    // If so, checks if the player's speed is less than 'maxSpeed' and checks if the player is grounded
                    if (speed < maxSpeed && isGrounded)
                    {
                        // If so, 'speed' increases by the amount given by 'speedIncrease' and 'tempSec' is set to 'elapsedTime'
                        speed += speedIncrease;
                        currSecOne = elapsedTime;
                    }
                }
            }

            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;

            // Sets the move variable
            Vector3 move = transform.right * h;
            move = new Vector3(move.x, move.y, 0).normalized;

            // Moves the player
            if (move.magnitude >= 0.1f)
            {
                var body = GetComponentInChildren<Animator>();
                float targetAngleOne = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
                float angleOne = Mathf.SmoothDampAngle(body.transform.eulerAngles.y, targetAngleOne, ref turnSmoothVelocityOne, turnSmoothTime);
                body.transform.rotation = Quaternion.Euler(0f, angleOne, 0f);

                var bound = GetComponentInChildren<TriggerParent>();
                float targetAngleTwo = Mathf.Atan2(move.z, move.x) * Mathf.Rad2Deg;
                float angleTwo = Mathf.SmoothDampAngle(bound.transform.eulerAngles.y, targetAngleTwo, ref turnSmoothVelocityTwo, turnSmoothTime);
                bound.transform.rotation = Quaternion.Euler(0f, angleTwo, 0f);

                controller.Move(move * speed * (Time.deltaTime * gameManager.timeManager.GetPlayerTimeScale()));
            }

            elapsedTime = float.Parse(Math.Round(endTime - Time.deltaTime, 0).ToString());
            // Checks if space bar is hit
            if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || isInWater))
            {
                // If so, and the player is grounded, the player jumps
                if (Input.GetKey(KeyCode.LeftShift) && highJumpEnabler.CheckHighJump())
                {
                    jumpHeight = jumpHeight * jumpMultiplier;
                    hasHighJumped = true;
                }

                velocity.y = (float)Math.Sqrt(jumpHeight * -2f * gravity);

                hasJumped = true;

                currSecTwo = elapsedTime;

                landSoundPlayed = false;
                if (jumpSound)
                    AudioSource.PlayClipAtPoint(jumpSound, transform.position);
            }
            else if (Input.GetKeyDown(KeyCode.Space) && hasJumped && !hasHighJumped)
            {
                // If so, and the player is not grounded, it checks if 'hasDoubleJumped' is false 
                if (isOnWallPlat)
                {
                    gravity = initialGravity;
                    velocity.y = (float)Math.Sqrt(jumpHeight * -2f * gravity);

                    if (jumpSound)
                        AudioSource.PlayClipAtPoint(jumpSound, transform.position);
                }
                else
                {
                    if (!hasDoubleJumped)
                    {
                        // If it is, the player jumps again and sets 'hasDoubleJumped' to true
                        velocity.y = (float)Math.Sqrt(jumpHeight * -2f * gravity);
                        hasDoubleJumped = true;

                        if (animator)
                            animator.SetTrigger("Jump");

                        if (jumpSound)
                            AudioSource.PlayClipAtPoint(jumpSound, transform.position);
                    }
                }
            }
            else if (isGrounded && hasJumped)
            {
                if (elapsedTime != currSecTwo)
                {
                    // If none of these apply, 'hasDoubleJumped' gets set to false and 'jumpHeight' is restored
                    hasDoubleJumped = false;

                    if (jumpHeight != initialJumpHeight)
                    {
                        highJumpEnabler.ResetOrbCount();
                        jumpHeight = initialJumpHeight;
                        hasHighJumped = false;
                    }

                    currSecTwo = 0f;

                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 2f))
                    {
                        if (hit.collider.isTrigger)
                        {
                            if (hit.collider.name == "HeadCheck")
                            {
                                if (hit.collider.transform.parent.tag == "Enemy")
                                {
                                    dealDamage.Attack(hit.collider.transform.parent.gameObject, hit.collider.transform.parent.GetComponent<Health>().currentHealth, 0, 0);
                                    velocity.y = (float)Math.Sqrt((jumpHeight / 2) * -2f * gravity);

                                    if (jumpSound)
                                        AudioSource.PlayClipAtPoint(jumpSound, transform.position);

                                    hasJumped = true;
                                    hasDoubleJumped = true;
                                }
                            }
                        }
                    }

                    if (landSound && !landSoundPlayed)
                    {
                        AudioSource.PlayClipAtPoint(landSound, transform.position);
                        landSoundPlayed = true;
                    }
                }
            }

            // Handles if the player is falling
            if (!onMovingPlatform)
            {
                if (isOnWallPlat)
                {
                    if (velocity.y < 0)
                        velocity.y += gravity * Time.deltaTime;
                    else
                        velocity.y += initialGravity * Time.deltaTime;
                }
                else
                {
                    velocity.y += initialGravity * Time.deltaTime;
                    gravity = initialGravity;
                }

                if (velocity.x < 0)
                    velocity.x += -(gravity * 3) * Time.deltaTime;
                else if (velocity.x > 0)
                    velocity.x += (gravity * 3) * Time.deltaTime;

                velocity = new Vector3(velocity.x, velocity.y, 0);
                controller.Move(velocity * Time.deltaTime);
            }

            posTwo = transform.position.x;
            distance = posTwo - posOne;

            if (distance < 0)
                distance = distance - (distance * 2);

            // Checks if an animator exists, if so, sets up the animations for the player
            if (animator)
            {
                animator.SetFloat("DistanceToTarget", distance);
                animator.SetBool("Grounded", isGrounded);
                animator.SetFloat("YVelocity", (float)Math.Round(velocity.y, 0));
            }

            // Checks the raycasts for different scenarios
            CheckRaycasts();

            disBetweenTargets = float.Parse(Math.Round((gameObject.transform.position.x - wallHazard.transform.position.x) - (wallHazard.transform.localScale.x / 2), 0).ToString());

            endTime += Time.deltaTime;
        }
        else
        {
            if (animator)
                animator.speed = 0f;
        }
    }

    private void CheckRaycasts()
    {
        // The 'Squish' feature
        RaycastHit h1;
        RaycastHit h2;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out h1, 2))
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out h2, 2))
            {
                if ((h1.collider.gameObject.tag == "Hazard" || h2.collider.gameObject.tag == "Hazard") && (h1.collider.gameObject.layer == 10 || h2.collider.gameObject.layer == 10))
                    dealDamage.Attack(gameObject, GetComponent<Health>().currentHealth, 0, 0);
            }
        }

        // Check for platforms
        RaycastHit h3;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out h3, 2))
        {
            // If the tag of the collider is in 'effectedPlatformTags', sets the Player parent to that object
            foreach (var tag in effectedPlatformTags)
            {
                if (h3.collider.gameObject.tag == tag)
                {
                    transform.SetParent(h3.collider.transform);
                    break;
                }
                else
                    transform.SetParent(null);
            }
        }
        else
            transform.SetParent(null);

        // Check for wall jump
        RaycastHit h4;
        RaycastHit h5;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out h4, 1))
        {
            if (h4.collider.gameObject.tag == "WallJump")
                currWall = h4.collider.gameObject;
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out h5, 1))
        {
            if (h5.collider.gameObject.tag == "WallJump")
                currWall = h5.collider.gameObject;
        }
        else
        {
            isOnWallPlat = false;
            gravity = initialGravity;
        }

        // Check for water
        RaycastHit h6;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out h6, 0.75f) || Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out h6, 0.75f) || Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out h6, 0.75f))
        {
            if (h6.collider.gameObject.tag == "Water")
                dealDamage.Attack(gameObject, GetComponent<Health>().currentHealth, 0, 0);
        }
    }
}

