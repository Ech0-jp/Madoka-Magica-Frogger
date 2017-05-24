using UnityEngine;
using System.Collections;

/// <summary>
/// Random Level Generater.
/// 
/// This script will spawn all the level objects randomly based on the resolution of the screen.
/// 
/// Each and every level that the player plays will be different.
/// 
/// How many objects spawn are based on the X and Y scale of the object (water being can be scaled to its own size for functionality purposes) and will
/// spawn in accordance to the size of the screen that the player is playing on. 
/// 
/// The water can be scaled to any size less than the horizontalDistance value.
/// 
/// </summary>

public class RandomLevelGenerater : MonoBehaviour 
{
	private GameObject player;

	[SerializeField] private GameObject grass;			// GameObject for the grass.
	[SerializeField] private GameObject road;			// GameObject for the road.
	[SerializeField] private GameObject water;			// GameObject for the water.
	[SerializeField] private GameObject safeZone;		// GameObject for the safe zone.

	[SerializeField] private GameObject scoringArea;	// GameObject for the scoring area.
	[SerializeField] private int grassScore;			// Score the player gets for entering a grass strip.
	[SerializeField] private int roadScore;				// Score the player gets for entering a road strip.
	[SerializeField] private int waterScore;			// Score the player gets for entering a water strip.

	[SerializeField] private Transform grassCont;		// The transform.parent of grass GameObjects.
	[SerializeField] private Transform roadCont;		// The transform.parent of road GameObjects.
	[SerializeField] private Transform waterCont;		// The transform.parent of water GameObjects.
	[SerializeField] private Transform safeZoneCont;	// The transform.parent of safe zone GameObjects.

	[SerializeField] private GameObject vehicleObj;		// GameObject for the vehicle.
	[SerializeField] private GameObject logObj;			// GameObject for the log.

	[SerializeField] private Transform vehicleCont;		// The transform.parent of the vehicles.
	[SerializeField] private Transform logCont;			// The transform.parent of the logs.

	[SerializeField] private GameObject playerSpawn;	// The players spawn (used for progression purposes)
	[SerializeField] private GameObject spawn;			// The spawn for the logs and vehicles.
	[SerializeField] private GameObject waypoint;		// The waypoint logs and vehicles move towards.

	[SerializeField] private Transform spawnCont;		// The transform.parent of the spawns.
	[SerializeField] private Transform waypointCont;	// The transform.parent of the waypoints.

	[SerializeField] private float horizontalDistance;	// How far the level objects will spawn away from eachother (Scale on X).
	[SerializeField] private float verticleDistance;	// How far the level objects will spawn away from eachother (Scale on Y).

	private Vector3[,] levelPos;						// The array that holds all of the level positioning.

	private bool playerSet;								// Checks to see if the players spawn has already been set.

	// Use this for initialization
	void Awake () 
	{
		player = GameObject.FindGameObjectWithTag ("Menu").GetComponent<MainMenu> ().selectedCharacter;
		GenerateLevel ();
	}

// Generates the GameBoard on start.
	void GenerateLevel()
	{
	// Gets the size of the screen.
		Vector3 posMax = Camera.main.ViewportToWorldPoint(new Vector3 (1, 1, Camera.main.nearClipPlane));
		Vector3 posMin = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, Camera.main.nearClipPlane));

	// How many objects will spawn to fill the screen.
		int horizontalSize 	= Mathf.RoundToInt (posMax.x / 2);
		int verticleSize 	= Mathf.RoundToInt (posMax.y / 2);

	// Creates the array that holds all of the levels positioning.
		levelPos = new Vector3[horizontalSize, verticleSize];

	// The starting position on the Y-Axis.
		float yPos = posMin.y + 2.0f;

	// Fills the levelPos array with correct positions.
		for (int y = 0; y < verticleSize; y++)
		{
			float xPos = posMin.x + 2.0f; // Starting position on the X-Axis.
			for (int x = 0; x < horizontalSize; x++)
			{
				levelPos[x, y] = new Vector3(xPos, yPos, 0.5f);
				xPos += horizontalDistance;
			}
			yPos += verticleDistance;
		}

		// Spawns all the randomly generated level objects.
		for (int y = 0; y < verticleSize; y++)
		{
			int randObj = 0;
			if (y != 0 && y != verticleSize - 1) // Makes it so the first strip will always be grass.
				randObj = Random.Range (1, 4);

			if (y == 0) // Creates the players spawn.
			{
				if (!playerSet)
				{
					GameObject clonePlayerSpawn = (GameObject) Instantiate (playerSpawn, new Vector3 (posMax.x / 5, levelPos[0, y].y, -0.1f), transform.rotation);
					clonePlayerSpawn.transform.parent = spawnCont;

					clonePlayerSpawn.SendMessage ("SetLevelCont", gameObject, SendMessageOptions.DontRequireReceiver);
					clonePlayerSpawn.SendMessage ("SetPlayer", player, SendMessageOptions.DontRequireReceiver);
					playerSet = true;
				}
			}

			for (int x = 0; x < horizontalSize; x++)
			{
			// If it is the first strip, make it grass.
				if (y == 0)
				{
					GameObject cloneStartingArea = (GameObject) Instantiate (grass, levelPos[x, y], transform.rotation);
					cloneStartingArea.name = grass.name;
					cloneStartingArea.transform.parent = grassCont;
				}
			// If it is the final strip, make it the safe zone.
				else if (y == verticleSize - 1)
				{
					if (x % 4 == 0)
					{
						GameObject cloneSafeZone = (GameObject) Instantiate (safeZone, new Vector3(levelPos[x, y].x, levelPos[x, y].y, -1), transform.rotation);
						cloneSafeZone.name = safeZone.name;
						cloneSafeZone.transform.parent = safeZoneCont;
					}
					else 
					{
						GameObject cloneWater = (GameObject) Instantiate (water, levelPos[x, y], transform.rotation);
						cloneWater.name = water.name;
						cloneWater.transform.parent = waterCont;
					}
				}
			// If it is anything inbetween, spawn randomly.
				else
				{
					switch (randObj)
					{
					case 1: // Spawn grass.
						GameObject cloneGrass = (GameObject) Instantiate (grass, levelPos[x, y], transform.rotation);
						cloneGrass.name = grass.name;
						cloneGrass.transform.parent = grassCont;
						break;
					case 2: // Spawn road.
						GameObject cloneRoad = (GameObject) Instantiate (road, levelPos[x, y], transform.rotation);
						cloneRoad.name = road.name;
						cloneRoad.transform.parent = roadCont;
						break;

					case 3: // spawn water.
						GameObject cloneWater = (GameObject) Instantiate (water, levelPos[x, y], transform.rotation);
						cloneWater.name = water.name;
						cloneWater.transform.parent = waterCont;
						break;
					}
				}
			}
			if (randObj == 1) // Creates a player spawn if it is grass.
			{
				GameObject cloneScoringArea = (GameObject) Instantiate (scoringArea, new Vector3 (posMax.x / 5, levelPos[0, y].y, -0.1f), transform.rotation);
				cloneScoringArea.transform.parent = grassCont;
				cloneScoringArea.SendMessage ("GetScore", grassScore, SendMessageOptions.DontRequireReceiver);
			}
			if (randObj == 2) // Creates vehicles spawn if its road.
			{
				int randRotation = Random.Range (0 ,100);
				GameObject cloneSpawn = null;
				GameObject cloneWaypoint = null;
			
			// Randomly generates the rotation (Left or right)
				if (randRotation < 50)
				{
					cloneSpawn 		= (GameObject) Instantiate (spawn, new Vector3(posMax.x + 5, levelPos[0, y].y, 0), Quaternion.Euler (180, 180, 90));
					cloneWaypoint 	= (GameObject) Instantiate (waypoint, new Vector3(posMin.x - 5, levelPos[0, y].y, 0), transform.rotation);
				}
				else if (randRotation >= 50)
				{
					cloneSpawn 		= (GameObject) Instantiate (spawn, new Vector3(posMin.x - 5, levelPos[0, y].y, 0), Quaternion.Euler (180, 0, -90));
					cloneWaypoint 	= (GameObject) Instantiate (waypoint, new Vector3(posMax.x + 5, levelPos[0, y].y, 0), transform.rotation);
				}

				cloneSpawn.transform.parent = spawnCont;
				cloneWaypoint.transform.parent = waypointCont;

			// Sends the info to the spawn so it will spawn the correct object and the object will travel towards the correct waypoint.
				cloneSpawn.SendMessage ("GetSpawningObj", vehicleObj, SendMessageOptions.DontRequireReceiver);
				cloneSpawn.SendMessage ("SetSpawnState", 1, SendMessageOptions.DontRequireReceiver);
				cloneSpawn.SendMessage ("GetWaypoint", cloneWaypoint, SendMessageOptions.DontRequireReceiver);
				cloneSpawn.SendMessage ("GetParentObj", vehicleCont, SendMessageOptions.DontRequireReceiver);

				GameObject cloneScoringArea = (GameObject) Instantiate (scoringArea, new Vector3 (posMax.x / 5, levelPos[0, y].y, -0.1f), transform.rotation);
				cloneScoringArea.transform.parent = roadCont;
				cloneScoringArea.SendMessage ("GetScore", roadScore, SendMessageOptions.DontRequireReceiver);
			}
			else if (randObj == 3) // Creates a log spawn if its water.
			{
				int randRotation 			= Random.Range (0 ,100);
				GameObject cloneSpawn 		= null;
				GameObject cloneWaypoint 	= null;

			// Randomly generates a roation (left or right)
				if (randRotation < 50)
				{
					cloneSpawn 		= (GameObject) Instantiate (spawn, new Vector3(posMax.x + 5, levelPos[0, y].y, 0), Quaternion.Euler (0, 0, 90));
					cloneWaypoint 	= (GameObject) Instantiate (waypoint, new Vector3(posMin.x - 5, levelPos[0, y].y, 0), transform.rotation);
				}
				else if (randRotation >= 50)
				{
					cloneSpawn 		= (GameObject) Instantiate (spawn, new Vector3(posMin.x - 5, levelPos[0, y].y, 0), Quaternion.Euler (0, 0, -90));
					cloneWaypoint 	= (GameObject) Instantiate (waypoint, new Vector3(posMax.x + 5, levelPos[0, y].y, 0), transform.rotation);
				}
				
				cloneSpawn.transform.parent 	= spawnCont;
				cloneWaypoint.transform.parent 	= waypointCont;

			// Sends the info to the spawn so it will spawn the correct object and the object will travel towards the correct waypoint.
				cloneSpawn.SendMessage ("GetSpawningObj", logObj, SendMessageOptions.DontRequireReceiver);
				cloneSpawn.SendMessage ("SetSpawnState", 2, SendMessageOptions.DontRequireReceiver);
				cloneSpawn.SendMessage ("GetWaypoint", cloneWaypoint, SendMessageOptions.DontRequireReceiver);
				cloneSpawn.SendMessage ("GetParentObj", logCont, SendMessageOptions.DontRequireReceiver);

				GameObject cloneScoringArea = (GameObject) Instantiate (scoringArea, new Vector3 (posMax.x / 5, levelPos[0, y].y, -0.1f), transform.rotation);
				cloneScoringArea.transform.parent = waterCont;
				cloneScoringArea.SendMessage ("GetScore", waterScore, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	void GameStart ()
	{
		Camera.main.SendMessage ("SoulGem", levelPos[0, 0]);
	}
}













