using UnityEngine;
using System.Collections;

public class SoulGem : MonoBehaviour 
{
	Animator animController;
	private PlayerController player;
	private float lifeTimer;
	private float originalLifeTimer;

	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		animController = GetComponentInChildren<Animator> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		lifeTimer = player.lifeTimer;
		if (lifeTimer > originalLifeTimer - (originalLifeTimer / 5))
		{
			animController.SetInteger ("GemState", 0);
		}
		else if (lifeTimer > originalLifeTimer - (originalLifeTimer / 5) * 2)
		{
			animController.SetInteger ("GemState", 1);
		}
		else if (lifeTimer > originalLifeTimer - (originalLifeTimer / 5) * 3)
		{
			animController.SetInteger ("GemState", 2);
		}
		else if (lifeTimer > originalLifeTimer - (originalLifeTimer / 5) * 4)
		{
			animController.SetInteger ("GemState", 3);
		}
		else
		{
			animController.SetInteger ("GemState", 4);
		}
	}

	void MaxLife (float life)
	{
		originalLifeTimer = life;
	}
}
