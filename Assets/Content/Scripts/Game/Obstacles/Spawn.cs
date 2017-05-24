using UnityEngine;
using System.Collections;

/// <summary>
/// Spawn.
/// 
/// This spawns the vehicles and logs based on the spawners state.
/// Each object spawns at a different rate based on the devs input in the inspector.
/// 
/// </summary>

public class Spawn : MonoBehaviour 
{
	private enum SpawnState
	{
		Vehicle		= 1,
		Log			= 2
	}
	private SpawnState spawnState; // Changes the spawn rate based off of what it is spawning.

	[SerializeField] private float minSpawnTimeVehicle;		// The minimum amount of time that can elapse for a vehicle to spawn.
	[SerializeField] private float maxSpawnTimeVehicle;		// The maximum amount of time that can elapse for a vehicle to spawn.

	[SerializeField] private float maxVehicleSpeed;			// The maximum speed that the vehicle can move;
	[SerializeField] private float minVehicleSpeed;			// The minimum speed that the vehicle can move;

	[SerializeField] private float minSpawnTimeLog;			// The minimum amount of time that can elapse for a log to spawn.
	[SerializeField] private float maxSpawnTimeLog;			// The maximum amount of time that can elapse for a log to spawn.

	[SerializeField] private float maxLogSpeed;				// The maximum speed that the log can move;
	[SerializeField] private float minLogSpeed;				// The minimum speed that the log can move;

	private float spawnTimer;								// The timer for spawning.

	private GameObject spawningObj;							// The GameObject that is to be spawned. (based off of RandomLevelGenerater)
	private GameObject waypoint;							// The waypoint that the spawningObj moves to.
	private float speed;									// The speed that the spawningObj moves at.

	private Transform spawnedCont;							// The transform.parent of the spawningObj.

	private bool canSpawn;									// If the obect can spawn or not.
	private bool pauseTime;									// If time is stopped.
	private bool bossStage;

	// Use this for initialization
	void Start () 
	{
		spawnTimer = 0.5f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!pauseTime && !bossStage)
		{
			spawnTimer -= Time.deltaTime;
			if (spawnTimer <= 0.0f)
			{
				if (canSpawn) // If it can spawn set the timer to the minSpawnTime and spawn an object.
				{
					SpawnObj ();
					switch(spawnState)
					{
					case SpawnState.Vehicle:
						spawnTimer = minSpawnTimeVehicle;
						break;
					case SpawnState.Log:
						spawnTimer = minSpawnTimeLog;
						break;
					}
					canSpawn = false;
				}
				else if (!canSpawn) // If it cant spawn set the timer to the maxSpawnTime.
				{
					switch (spawnState)
					{
					case SpawnState.Vehicle:
						spawnTimer = Random.Range (0.0f, maxSpawnTimeVehicle);
						break;
					case SpawnState.Log:
						spawnTimer = Random.Range (0.0f, maxSpawnTimeLog);
						break;
					}
					canSpawn = true;
				}
			}
		}
	}

// Spawns the GameObject.
	void SpawnObj ()
	{
		GameObject cloneObj = (GameObject)Instantiate (spawningObj, transform.position, transform.rotation);
		cloneObj.transform.parent = spawnedCont;
		cloneObj.SendMessage ("SetWaypoint", waypoint);
		cloneObj.SendMessage ("SetSpeed", speed);
	}

// Gets what object will spawn from the RandomLevelGenerater.
	void GetSpawningObj (GameObject other)
	{
		spawningObj = other;
	}

// Gets the state of the spawner from the RandomLevelGenerater.
	void SetSpawnState (int other)
	{
		switch (other)
		{
		case 1:
			spawnState = SpawnState.Vehicle;
			speed = Random.Range (minVehicleSpeed, maxVehicleSpeed);
			break;
		case 2:
			spawnState = SpawnState.Log;
			speed = Random.Range (minLogSpeed, maxLogSpeed);
			break;
		}
	}

	void BossStage (bool other)
	{
		bossStage = other;
	}

	void SetPauseTime (bool other)
	{
		pauseTime = other;
	}

// Gets the waypoint the spawningObj will travel to from the RandomLevelGenerater.
	void GetWaypoint (GameObject other)
	{
		waypoint = other;
	}

// Gets the transform.parent of the spawningObj from the RandomLevelGenerater.
	void GetParentObj (Transform other)
	{
		spawnedCont = other;
	}
}














