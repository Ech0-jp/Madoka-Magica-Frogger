using UnityEngine;
using System.Collections;

public class LargeArrow1_Controller : MonoBehaviour 
{
	[SerializeField] private GameObject blood;

	[SerializeField] private Vector3 charge1Size;					// The size of the arrow for charge 1.
	[SerializeField] private Vector3 charge2Size;					// The size of the arrow for charge 2.

	[SerializeField] private float lifeTime				= 2.0f;		// The amount of time the arrow will last for.
	[SerializeField] private float speed				= 45.0f;	// How for the arrow will travel.
	
	[SerializeField] private int charge1MaxDamage		= 35;		// The max damage the arrow can do.
	[SerializeField] private int charge1MinDamage		= 20;		// The min damage the arrow can do.

	[SerializeField] private int charge2MaxDamage		= 50;		// The max damage the arrow can do.
	[SerializeField] private int charge2MinDamage		= 25;		// The min damage the arrow can do.

	private int arrowState								= 1;		// The charge of the arrow (1 or 2).
	private int direction								= 1;		// The direction the arrow is facing.

	// Use this for initialization
	void Start () 
	{
		if (transform.rotation.y > 90.0f)
			direction = -1;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Translate (Vector3.right * (direction * (speed * Time.deltaTime)));
		
		lifeTime -= Time.deltaTime;
		if (lifeTime <= 0.0f)
			Destroy (gameObject);
	}

	void ArrowState (int i)
	{
		switch (i)
		{
		case 1:
			transform.localScale = charge1Size;
			break;
		case 2:
			transform.localScale = charge2Size;
			break;
		}
		arrowState = i;
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Boss")
		{
			switch (arrowState)
			{
			case 1:
				other.SendMessageUpwards ("Damage", Random.Range (charge1MinDamage, charge1MaxDamage + 1), SendMessageOptions.DontRequireReceiver);
				break;
			case 2:
				other.SendMessageUpwards ("Damage", Random.Range (charge2MinDamage, charge2MaxDamage + 1), SendMessageOptions.DontRequireReceiver);
				break;
			}
			Quaternion bloodRotation = new Quaternion();
			if (direction == 1)
				bloodRotation = new Quaternion (0, 180, 90, 0);
			else if (direction == -1)
				bloodRotation = new Quaternion (0, -90, 90, 0);
			GameObject cloneBlood = (GameObject) Instantiate (blood, transform.position, bloodRotation);
		}
	}
}












