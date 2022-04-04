using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DealDamage))]
public class SpikeTrap : MonoBehaviour
{
    // Public Variables
    // Inspector Variables
    [Header("Main Settings")]
    public TriggerParent hitbox;
    public float damage = 2f;
    public float pushHeight = 1f;
    public float pushForce = 1f;
    public string[] effectedTags;

    // Private Variables
    // Bool Variables
    private bool isValid = true;

    // Component Variables
    private DealDamage dealDamage;

    private void Start()
    {
        if (!hitbox)
        {
            isValid = false;
            Debug.LogError("'hitbox' is missing.");
        }

        dealDamage = GetComponent<DealDamage>();
    }

    private void Update()
    {
        if (isValid)
        {
            if (hitbox.collided && hitbox.hitObject != null)
            {
                foreach (var tag in effectedTags)
                {
                    if (hitbox.hitObject.tag == tag)
                        dealDamage.Attack(hitbox.hitObject, (int)damage, pushHeight, pushForce);
                }
            }
        }
    }
}
