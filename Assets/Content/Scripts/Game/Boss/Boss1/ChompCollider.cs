using UnityEngine;
using System.Collections;

public class ChompCollider : MonoBehaviour 
{
	[SerializeField] private float maxChompDamage		= 70.0f;	// The maximum amount of damage that a chomp will do.
	[SerializeField] private float minChompDamage		= 35.0f;	// The minimum amount of damage that a chomp will do.

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "BossPlayer")
		{
			other.SendMessage ("Damage", Random.Range (minChompDamage, maxChompDamage), SendMessageOptions.DontRequireReceiver);
		}
	}
}
