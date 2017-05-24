using UnityEngine;
using System.Collections;

public class TogeCollider : MonoBehaviour 
{
	[SerializeField] private float maxTogeDamage		= 40.0f;	// The maximum amount of damage that a toge will do.
	[SerializeField] private float minTogeDamage		= 20.0f;	// The minimum amount of damage that a toge will do.

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "BossPlayer")
		{
			other.SendMessage ("Damage", Random.Range (minTogeDamage, maxTogeDamage), SendMessageOptions.DontRequireReceiver);
		}
	}
}
