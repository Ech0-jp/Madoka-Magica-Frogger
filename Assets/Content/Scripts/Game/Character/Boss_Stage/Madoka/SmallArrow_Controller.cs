using UnityEngine;
using System.Collections;

public class SmallArrow_Controller : MonoBehaviour 
{
	[SerializeField] private GameObject blood;

	[SerializeField] private float lifeTime				= 2.0f;		// The amount of time the arrow will last for.
	[SerializeField] private float speed				= 45.0f;	// How for the arrow will travel.

	[SerializeField] private int maxDamage				= 25;		// The max damage the arrow can do.
	[SerializeField] private int minDamage				= 10;		// The min damage the arrow can do.
	private int direction								= 1;		// The direction the arrow is facing.

	// Use this for initialization
	void Start () 
	{
		float offset = Random.Range (-0.05f, 0.05f);
		Quaternion rotate = transform.rotation;
		rotate.z += offset;
		transform.rotation = rotate;
		
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
			Quaternion bloodRotation = new Quaternion();
			if (direction == 1)
				bloodRotation = new Quaternion (0, 180, 90, 0);
			else if (direction == -1)
				bloodRotation = new Quaternion (0, -90, 90, 0);
			GameObject cloneBlood = (GameObject) Instantiate (blood, transform.position, bloodRotation);
			other.SendMessageUpwards ("Damage", Random.Range (minDamage, maxDamage + 1), SendMessageOptions.DontRequireReceiver);
			Destroy (gameObject);
		}
	}
}
