using UnityEngine;
using System.Collections;

public class FireWorkController : MonoBehaviour 
{
	[SerializeField] private int maxDamage					= 100;		// The max damage the firework can output.
	[SerializeField] private int minDamage					= 50;		// The min damage the firework can output.
	private bool touchingBoss								= false;	// If the firework can hit the boss.
	private float canHitTimer								= 0.15f;	// The amount of time it takes for the firework to start it's explosion.
	private bool hit										= false;	// If the firework has already hit the boss.

	// Use this for initialization
	void Start () 
	{
		StartCoroutine ("DestroyFireWork", 0.44f);
	}

	void Update ()
	{
		canHitTimer -= Time.deltaTime;
		if (canHitTimer <= 0.0f)
		{
			if (touchingBoss)
			{
				if (!hit)
				{
					GameObject.FindGameObjectWithTag ("BossController").SendMessage ("Damage", Random.Range (minDamage, maxDamage + 1), SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	IEnumerator DestroyFireWork (float time)
	{
		yield return new WaitForSeconds (time);

		Destroy (gameObject);
	}

	void OnTriggerStay2D (Collider2D other)
	{
		if (other.tag == "Boss")
		{
			touchingBoss = true;
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{
		if (other.tag == "Boss")
		{
			touchingBoss = false;
		}
	}
}












