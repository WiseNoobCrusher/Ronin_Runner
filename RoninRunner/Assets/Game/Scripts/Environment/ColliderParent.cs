using UnityEngine;
using System.Collections;

//this is a utility class. It holds collision information about this collider, so another script can access that.
public class ColliderParent : MonoBehaviour
{
	public string[] tagsToCheck;            //if left empty, collider will check collisions from everything. Othewise, it will only check these tags

	[HideInInspector]
	public bool collided, colliding;
	[HideInInspector]
	public GameObject hitObject;

	void Awake()
	{
		if (!GetComponent<Collider>() || (GetComponent<Collider>() && GetComponent<Collider>().isTrigger))
			Debug.LogError("'ColliderParent' script attached to object which does not have a trigger collider", transform);
	}

	//see if anything entered collider, filer by tag, store the object
	private void OnCollisionEnter(Collision other)
	{
		if (tagsToCheck.Length > 0 && !collided)
		{
			foreach (string tag in tagsToCheck)
			{
				if (other.transform.gameObject.tag == tag)
				{
					collided = true;
					hitObject = other.gameObject;

					break;
				}

			}
		}
		else
			collided = true;
		hitObject = other.gameObject;
	}

	//see if anything is constantly colliding with this collider, filter by tag, store the object
	void OnCollisionStay(Collision other)
	{
		if (tagsToCheck.Length > 0)
		{
			foreach (string tag in tagsToCheck)
			{
				if (other.transform.gameObject.tag == tag)
				{
					colliding = true;
					hitObject = other.gameObject;

					break;
				}
			}
		}
		else
		{
			hitObject = other.gameObject;
			colliding = true;
		}
	}

    void OnCollisionExit(Collision other)
	{
		if (tagsToCheck.Length > 0)
		{
			foreach (string tag in tagsToCheck)
			{
				if (other.transform.gameObject.tag == tag)
				{
					colliding = false;
					hitObject = null;
					break;
				}
			}
		}

		else
			return;
	}

	//this runs after the main code, and resets the info to false
	//so we know when something is no longer collider with this trigger
	void LateUpdate()
	{
		if (collided)
		{
			collided = false;
			hitObject = null;
		}
	}
}
