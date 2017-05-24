using UnityEngine;
using System.Collections;

public class RocketController : MonoBehaviour 
{
	[SerializeField] private GameObject explosion;

	[SerializeField] private float speed					= 45.0f;
	[SerializeField] private float lifeTime					= 2.0f;
	[SerializeField] private int maxDamage					= 30;
	[SerializeField] private int minDamage					= 15;
	private bool canMove									= true;
	private int direction									= 1;
	
	// Use this for initialization
	void Start () 
	{
		if (transform.rotation.y > 90.0f)
			direction = -1;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (canMove)
		{
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
			GameObject cloneExplosion = (GameObject) Instantiate (explosion, transform.position, transform.rotation);
			other.SendMessageUpwards ("Damage", Random.Range (minDamage, maxDamage + 1), SendMessageOptions.DontRequireReceiver);
			Destroy (gameObject);
		}
	}
}
