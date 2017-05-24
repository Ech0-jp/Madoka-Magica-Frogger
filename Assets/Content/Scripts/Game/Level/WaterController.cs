using UnityEngine;
using System.Collections;

/// <summary>
/// Water controller.
/// 
/// This removes the waters tag so the player does not die while on the log.
/// 
/// </summary>

public class WaterController : MonoBehaviour 
{
	private bool playerOn;

// While the log is over the water, remove the tag.
	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Log")
		{
			gameObject.tag = "Untagged";
		}

		if (other.tag == "Player")
		{
			playerOn = true;
		}
	}
// If the log is not over the water, set the tag to "Water".
	void OnTriggerExit2D (Collider2D other)
	{
		if (other.tag == "Log")
		{
			gameObject.tag = "Water";
			if (playerOn)
			{
				GameObject.FindGameObjectWithTag ("Player").SendMessage ("KillPlayer", SendMessageOptions.DontRequireReceiver);
			}
		}

		if (other.tag == "Player")
		{
			playerOn = false;
		}
	}
}
