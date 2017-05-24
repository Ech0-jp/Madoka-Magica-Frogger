using UnityEngine;
using System.Collections;

public class Boss_GUI : MonoBehaviour 
{
	[SerializeField] private Texture2D bossHP_Green;

	public Boss1_Controller boss;
	private float health;
	private float maxHealth;

	// Use this for initialization
	void Start () 
	{
		boss = GameObject.FindGameObjectWithTag ("BossController").GetComponent<Boss1_Controller> ();
		maxHealth = (float) boss.health;
	}
	
	// Update is called once per frame
	void Update () 
	{
		health = (float) boss.health;
	}

	void OnGUI ()
	{	
		Rect hPos = new Rect();
		hPos.x = Screen.width;
		hPos.y = -10;
		hPos.width = -800 * (health / maxHealth);
		hPos.height = 20;
		GUI.DrawTexture (hPos, bossHP_Green);
	}
}
