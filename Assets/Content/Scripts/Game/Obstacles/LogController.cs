using UnityEngine;
using System.Collections;

/// <summary>
/// Log controller.
/// 
/// This moves the log to its waypoint and destroys it once it reaches its target.
/// 
/// </summary>

public class LogController : MonoBehaviour 
{
	private float speed;						// The speed of the log.
	private int logDirection;					// The direction the log is moving.
	private Vector3 targetLocation;				// The waypoints location.
	private bool pauseTime;						// If time is stopped.
	
	// Update is called once per frame
	void FixedUpdate () 
	{
	// Moves the log to the waypoint. Once it reaches its waypoint, destroy it.
		if (!pauseTime)
		{
			transform.position = Vector3.MoveTowards (transform.position, targetLocation, speed * Time.deltaTime);
			if (transform.position == targetLocation)
			{
				Destroy (gameObject);
			}
		}
	}

// Sets the waypoint that the log will travel to.
	void SetWaypoint (GameObject waypoint)
	{
		targetLocation = waypoint.transform.position;
		if (targetLocation.x > transform.position.x)
			logDirection = 1;
		else if (targetLocation.x < transform.position.x)
			logDirection = -1;
	}

// Sets the speed that the log moves at.
	void SetSpeed(float other)
	{
		speed = other;
	}

	void SetPauseTime (bool other)
	{
		pauseTime = other;
	}

// Moves the player with the log.
	void OnTriggerStay2D (Collider2D other)
	{
		if (other.tag == "Player")
		{
			if (other.GetComponent<PlayerController>().isMoving != true && !pauseTime)
			{
				other.transform.position = Vector3.MoveTowards (other.transform.position, new Vector3(targetLocation.x, other.transform.position.y, other.transform.position.z), speed * Time.deltaTime);
			}
			other.SendMessage ("GetLogSpeed", speed, SendMessageOptions.DontRequireReceiver);
			other.SendMessage ("GetLogDirection", logDirection, SendMessageOptions.DontRequireReceiver);
		}
	}
}
