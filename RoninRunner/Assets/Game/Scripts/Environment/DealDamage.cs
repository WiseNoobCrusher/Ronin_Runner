using System;
using UnityEngine;

//attach this to any object which needs to deal damage to another object
public class DealDamage : MonoBehaviour 
{
	// Damage Variables
	private Health health;

    //remove health from object and push it
    public void Attack(GameObject victim, int dmg, float pushHeight, float pushForce)
	{
		health = victim.GetComponent<Health>();
		//push
		//Vector3 pushDir = (victim.transform.position - transform.position);
		//pushDir.y = 0f;
		//pushDir.y = pushHeight * 0.1f;

		//deal dmg
		if (health && !health.flashing)
		{
			health.currentHealth -= dmg;
			//Debug.Log(this.name + " attacked " + victim.name);
		}
	}
}

/* NOTE: if you just want to push objects you could use this script but set damage to 0. (ie: a bouncepad)
 * if you want to restore an objects health, set the damage to a negative number (ie: a healing bouncepad!) */