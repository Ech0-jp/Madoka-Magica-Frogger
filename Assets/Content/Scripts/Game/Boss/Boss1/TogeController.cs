using UnityEngine;
using System.Collections;

public class TogeController : MonoBehaviour 
{
	[SerializeField] private float waitTime			= 2.0f; 		// The amount of time to elapse before the toge will appear.
	[SerializeField] private float attackTime		= 3.0f;			// The amount of time that the toge will attack for.
	[SerializeField] private float waitForTrigger	= 0.5f;			// The amount of time to elapse before the BoxCollider will enable allowing the player to get hit.

	[SerializeField] private GameObject togeColliderLeft;			// The toges collider when the boss is facing the left.
	[SerializeField] private GameObject togeColliderRight;			// The toges collider when the boss is facing the right.

	private float lifeTime							= 5.0f;			// The life time of the toge.
	private Animator animController;								// The animator controller for the toge.
	private int direction;											// The direction the toge is facing when the boss spawns it.


	// Use this for initialization
	void Start () 
	{
		animController = GetComponentInChildren<Animator> ();
		lifeTime = waitTime + attackTime + 1.2f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (waitTime > 0.0f)
		{
			waitTime -= Time.deltaTime;
			if (waitTime <= 0.0f)
				animController.SetBool ("Attack", true);
		}
		else if (attackTime > 0.0f)
		{
			attackTime -= Time.deltaTime;

			waitForTrigger -= Time.deltaTime;
			if (waitForTrigger <= 0.0f)
			{
				if (direction == 1)
					togeColliderRight.GetComponent<BoxCollider2D> ().enabled = true;
				else if (direction == -1)
					togeColliderLeft.GetComponent<BoxCollider2D> ().enabled = true;
			}

			if (attackTime <= 0.0f)
				animController.SetBool ("Attack", false);
		}

		lifeTime -= Time.deltaTime;
		if (lifeTime <= 0.0f)
			DestroyToge ();
	}

	void DestroyToge ()
	{
		Destroy (gameObject);
	}

	void GetDirection (int other)
	{
		direction = other;
	}
}
