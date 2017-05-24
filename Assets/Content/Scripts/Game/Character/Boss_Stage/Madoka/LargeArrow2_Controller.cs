using UnityEngine;
using System.Collections;

public class LargeArrow2_Controller : MonoBehaviour 
{
	[SerializeField] private GameObject blood;

	[SerializeField] private float lifeTime				= 2.0f;		// The amount of time the arrow will last for.
	[SerializeField] private float speed				= 45.0f;	// How for the arrow will travel.
	
	[SerializeField] private int maxDamage				= 85;		// The max damage the arrow can do.
	[SerializeField] private int minDamage				= 60;		// The min damage the arrow can do.
	
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

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Boss")
		{
			other.SendMessageUpwards ("Damage", Random.Range (minDamage, maxDamage + 1), SendMessageOptions.DontRequireReceiver);
			Quaternion bloodRotation = new Quaternion();
			if (direction == 1)
				bloodRotation = new Quaternion (0, 180, 90, 0);
			else if (direction == -1)
				bloodRotation = new Quaternion (0, -90, 90, 0);
			GameObject cloneBlood = (GameObject) Instantiate (blood, transform.position, bloodRotation);
		}
	}
}
