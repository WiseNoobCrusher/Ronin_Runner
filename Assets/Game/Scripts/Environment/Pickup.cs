using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Public variables
    public Vector3 rotation = new Vector3(0, 80, 0);
    public Vector3 rotationGain = new Vector3(10, 20, 10);  //added rotation when player gets near coin 
    public float startSpeed = 3f;                           //how fast the pickup moves toward player when they get near
    public float speedGain = 0.2f;                          //how fast the pickup accelerates toward player when they're near
    [Header("Pickup Settings")]
    public float[] values;

    // Private variables
    private GameObject[] pickups;
    private GameObject currentPickup;
    private TriggerParent triggerParent;
    private bool collected;
    private Transform player;
    private GameObject deathWall;
    private WallHazard wallHazard;


    void Awake()
    {
        // Gets the current pickup from this GameObject
        pickups = GameObject.FindGameObjectsWithTag("Pickup");
        for (int i = 0; i < pickups.Length; i++)
        {
            if (pickups[i].name == name)
                currentPickup = pickups[i];
        }

        GetComponent<Collider>().isTrigger = true;
        triggerParent = GetComponentInChildren<TriggerParent>();

        //if no trigger bounds are attached to the pickup, set them up
        if (!triggerParent)
        {
            GameObject bounds = new GameObject();
            bounds.name = "Bounds";
            bounds.AddComponent<SphereCollider>();
            bounds.GetComponent<SphereCollider>().radius = 7f;
            bounds.GetComponent<SphereCollider>().isTrigger = true;
            bounds.transform.parent = transform;
            bounds.transform.position = transform.position;
            bounds.AddComponent<TriggerParent>();
            triggerParent = GetComponentInChildren<TriggerParent>();
            triggerParent.tagsToCheck = new string[1];
            triggerParent.tagsToCheck[0] = "Player";
            Debug.LogWarning("No pickup radius 'bounds' trigger attached to the pickup: " + transform.name + ", one has been added automatically", bounds);
        }

        int nullCount = 0;
        int nameCount = 0;
        GameObject[] hazards = GameObject.FindGameObjectsWithTag("Hazard");
        for (int i = 0; i < hazards.Length; i++)
        {
            // Checks if any objects with the tag 'Hazard' exist
            if (hazards[i] == null)
                nullCount++;
            else
            {
                // Checks if any objects with the tag 'Hazard', has the name 'DangerWall'
                if (hazards[i].name == "DangerWall")
                    deathWall = hazards[i];
                else
                    nameCount++;
            }
        }

        // Checks if 'hazards' is null
        if (nullCount == hazards.Length)
            Debug.LogError("No object with the tag 'Hazard' exists.");

        // Checks if there was any object with the tag 'Hazard', had the name 'DangerWall'
        if ((nameCount - nullCount) == (hazards.Length - nullCount))
            Debug.LogError("No object with the tag 'Hazard', has the name 'DangerWall'");

        // Checks if 'wallHazard' is null and if the component 'WallHazard' is not null
        if ((wallHazard == null) && (deathWall.GetComponent<WallHazard>() == true))
            wallHazard = deathWall.GetComponent<WallHazard>();
        else
            Debug.LogError("The 'WallHazard' component is missing.");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") == true)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        else
            Debug.LogError("Player does not exist.");
    }

    // Update is called once per frame
    void Update()
    {
        // Rotates the player
        transform.Rotate(rotation * Time.deltaTime, Space.World);

        if (triggerParent.collided)
            collected = true;

        // If true, the object starts going towards the player
        if (collected)
        {
            startSpeed += speedGain;
            rotation += rotationGain;
            transform.position = Vector3.Lerp(transform.position, player.position, startSpeed * Time.deltaTime);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        // Checks if the object that collided is the player
        if (other.tag == "Player")
            PickupGet();
    }

    private void PickupGet()
    {
        /* Checks if the pickup name has '(1)', '(2)', etc. If it does, it splits the 
           name by the space and puts the name in a seperate string */
        string theName = "";
        if (currentPickup.name[currentPickup.name.Length - 1] == ')')
            theName = currentPickup.name.Split(' ')[0].ToString();
        else
            theName = currentPickup.name;

        // Checks which pickup it is and does the appropriate action
        // Requires values[0] & values[1]; values[0] = newSpeed & values[1] = time
        if (theName == "SlowDownWall_PowerUp")
        {
            /* Checks if the two required values aren't 0 (if need to be higher than 0), 
               and gives an error if it is 0 */
            if (values[0] != 0)
            {
                if (values[1] != 0)
                    // Runs the function that stops the wall from moving
                    wallHazard.SlowDownTheWall(values[0], values[1]);
                else
                    Debug.LogError("value[1] is null.");
            }
            else
                Debug.LogError("value[0] is null.");
        }
        // Requires values[0]; values[0] = time
        else if (theName == "StopWall_PowerUp")
        {
            /* Checks if the one required value isn't 0 (if need to be higher than 0), 
               and gives an error if it is */
            if (values[0] != 0)
                wallHazard.StopTheWall(values[0]);
            else
                Debug.LogError("values[0] is null");
        }

        // Destroys the object after it has been picked up
        Destroy(gameObject);
    }
}
