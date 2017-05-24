using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	private Camera bossCamera;

	private bool cutSceneMove						= false;
	[SerializeField] private float cutScenetarget	= 65.0f;

	[SerializeField] private float cameraSpeed		= 15.0f;		// The speed the camera moves.

	[SerializeField] private float trigger			= 10.0f;		// The point at which the camera will start moving.
	[SerializeField] private float maxDistance		= 82.0f;		// The maximum distance that the camera can move to.
	[SerializeField] private float minDistance		= 0.0f;			// The minimum distance that the camera can move to.

	private GameObject player;										// The player's GameObject.

	// Use this for initialization
	void Start () 
	{
		bossCamera = gameObject.GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (player != null)
		{
			Vector3 posMax = bossCamera.ViewportToWorldPoint(new Vector3 (1, 1, bossCamera.nearClipPlane));
			Vector3 posMin = bossCamera.ViewportToWorldPoint (new Vector3 (0, 0, bossCamera.nearClipPlane));

			if (player.transform.position.x > posMax.x - trigger)
			{
				if (transform.position.x < maxDistance)
				{
					transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x + 1, transform.position.y, transform.position.z), cameraSpeed * Time.deltaTime);
				}
			}

			if (player.transform.position.x < posMin.x + trigger)
			{
				if (transform.position.x > minDistance)
				{
					transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x - 1, transform.position.y, transform.position.z), cameraSpeed * Time.deltaTime);
				}
			}

			if (cutSceneMove)
			{
				transform.position = Vector3.MoveTowards (transform.position, new Vector3 (cutScenetarget, transform.position.y, transform.position.z), cameraSpeed * Time.deltaTime);
				if (transform.position.x == cutScenetarget)
					cutSceneMove = false;
			}
		}
	}

	void SetPlayer ()
	{
		player = GameObject.FindGameObjectWithTag ("BossPlayer");
	}

	void CutScene()
	{
		cutSceneMove = true;
	}
}
