using UnityEngine;
using System.Collections;

/// <summary>
/// Vehicle controller.
/// 
/// This moves the vehicle to its waypoint and destroys it once it reaches its target.
/// 
/// </summary>

public class VehicleController : MonoBehaviour 
{
	private float speed;					// The speed of the vehicle.
	private Vector3 targetLocation;			// The location of the waypoint.
	private bool pauseTime;					// If time is stopped.s
	
	// Update is called once per frame
	void FixedUpdate () 
	{
	// Moves the vehicle to its waypoint, once it reaches it, destroy it.
		if (!pauseTime)
		{
			transform.position = Vector3.MoveTowards (transform.position, targetLocation, speed * Time.deltaTime);
			if (transform.position == targetLocation)
			{
				Destroy (gameObject);
			}
		}
	}

	void KillEnemy ()
	{
		Destroy (gameObject);
	}

	void SetPauseTime (bool other)
	{
		pauseTime = other;
	}

// Sets the speed that the log moves at.
	void SetSpeed(float other)
	{
		speed = other;
	}
// Set the vehicles waypoint.
	void SetWaypoint (GameObject waypoint)
	{
		targetLocation = waypoint.transform.position;
	}
}
