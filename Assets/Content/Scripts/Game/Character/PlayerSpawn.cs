using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour 
{
	private GameObject player;
	private GameObject levelCont;

	void SetPlayer(GameObject other)
	{
		player = other;
		SpawnPlayer ();
	}

	void SetLevelCont (GameObject other)
	{
		levelCont = other;
	}

	void SpawnPlayer ()
	{
		GameObject clonePlayer = (GameObject)Instantiate (player, transform.position, transform.rotation);
		clonePlayer.name = player.name;
		clonePlayer.transform.parent = levelCont.transform;

		clonePlayer.SendMessage ("SetSpawn", gameObject, SendMessageOptions.DontRequireReceiver);
	}
}
