using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour 
{
	[SerializeField] private GameObject blood;

	[SerializeField] private float speed					= 45.0f;
	[SerializeField] private float lifeTime					= 2.0f;
	[SerializeField] private float waitTime					= 0.3f;
	[SerializeField] private int maxDamage					= 5;
	[SerializeField] private int minDamage					= 1;
	private bool canMove									= true;
	private int direction									= 1;

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
		if (canMove)
		{
			if (waitTime > 0.0f)
				waitTime -= Time.deltaTime;
			else
				transform.Translate (Vector3.right * (direction * (speed * Time.deltaTime)));

			lifeTime -= Time.deltaTime;
			if (lifeTime <= 0.0f)
				Destroy (gameObject);
		}
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
