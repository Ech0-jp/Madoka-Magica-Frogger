using UnityEngine;
using System.Collections;

public class SafeZone : MonoBehaviour 
{
	private bool inSafeZone;
	private bool takenSafeZone;
	[SerializeField] private int score;

	private float timer;

	void Start ()
	{
		GameObject.FindGameObjectWithTag ("Player").SendMessage ("SetSafeZoneCount", SendMessageOptions.DontRequireReceiver);
	}

	void FixedUpdate ()
	{
		if (inSafeZone)
		{
			timer -= Time.deltaTime;
			if (timer <= 0.0f)
			{
				TakeSafeZone ();
			}
		}
	}

	void TakeSafeZone ()
	{
		inSafeZone = false;

		if (!takenSafeZone)
		{
			takenSafeZone = true;
			GameObject.FindGameObjectWithTag ("LevelController").BroadcastMessage ("ResetScoringArea", SendMessageOptions.DontRequireReceiver);
		}
	}

	void SafeZoneReached (float other)
	{
		GetComponentInChildren<Animator> ().SetBool ("CloseDoor", true);
		inSafeZone = true;
		timer = other;
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (takenSafeZone)
		{
			if (other.tag == "Player")
			{
				other.SendMessage ("TakenSafeZone", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
