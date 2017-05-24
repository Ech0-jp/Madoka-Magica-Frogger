using UnityEngine;
using System.Collections;

public class ScoringArea : MonoBehaviour 
{
	private bool hasTriggered;		// If the player already entered the area.
	private int score;				// The score the player reveices when entering the area.

	void GetScore (int other)
	{
		score = other;
	}

	void ResetScoringArea ()
	{
		hasTriggered = false;
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (!hasTriggered)
		{
			if (other.tag == "Player")
			{
				hasTriggered = true;
				other.SendMessage ("AddScore", score, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
