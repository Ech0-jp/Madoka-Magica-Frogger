using UnityEngine;
using System.Collections;

public class Life_BossStage : MonoBehaviour 
{
	private Camera bossCamera;

	private enum Character
	{
		Homura,
		Kyouko,
		Madoka,
		Mami,
		Sayaka
	}
	[SerializeField] private Character character;

	[SerializeField] private Texture2D greenHP;
	[SerializeField] private Texture2D redHP;
	[SerializeField] private Texture2D blueHP;
	[SerializeField] private Texture2D whiteHP;
	[SerializeField] private Texture2D blackHP;

	private Texture2D currentHP_Foreground;
	private Texture2D currentHP_Background;

	private Animator animController;				// The anim controller for the soul gem.
	private CharacterController_BossStage player;	// The player's charactercontroller.
	private float lifeTimer;						// The player's life timer.
	private float originalLifeTimer;				// The original value of the player's life timer.
	private float health;							// The player's health.
	private float maxHealth;						// The max health the player can have.
	private bool dead;

	// Use this for initialization
	void Start () 
	{
		bossCamera = GameObject.FindGameObjectWithTag ("BossCamera").GetComponent<Camera> ();
		animController = GetComponentInChildren<Animator> ();
		player = GameObject.FindGameObjectWithTag ("BossPlayer").GetComponent<CharacterController_BossStage> ();

		CharacterStats stats = GameObject.FindGameObjectWithTag ("BossStage").GetComponent<CharacterStats> ();
		switch (character)
		{
		case Character.Homura:
			originalLifeTimer = stats.characterStats.homura.lifeTimer;
			maxHealth = stats.characterStats.homura.health;
			break;
		case Character.Kyouko:
			originalLifeTimer = stats.characterStats.kyouko.lifeTimer;
			maxHealth = stats.characterStats.kyouko.health;
			break;
		case Character.Madoka:
			originalLifeTimer = stats.characterStats.madoka.lifeTimer;
			maxHealth = stats.characterStats.madoka.health;
			break;
		case Character.Mami:
			originalLifeTimer = stats.characterStats.mami.lifeTimer;
			maxHealth = stats.characterStats.mami.health;
			break;
		case Character.Sayaka:
			originalLifeTimer = stats.characterStats.sayaka.lifeTimer;
			maxHealth = stats.characterStats.sayaka.health;
			break;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (player != null)
		{
			lifeTimer = player.lifeTimer;
			health = player.health;
			dead = player.dead;
			if (dead)
			{
				currentHP_Foreground = whiteHP;
				currentHP_Background = blackHP;
			}
			else 
			{
				currentHP_Foreground = greenHP;
				currentHP_Background = redHP;
			}

			UpdateSoulGem ();
		}
		else 
			player = GameObject.FindGameObjectWithTag ("BossPlayer").GetComponent<CharacterController_BossStage> ();
	}

	void UpdateSoulGem ()
	{
		Vector3 pos = bossCamera.ScreenToWorldPoint (new Vector3 (Screen.width / 3, 0, bossCamera.nearClipPlane));
		transform.position = new Vector3 (pos.x, -17.2f, 0);

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

	void OnGUI ()
	{
		DrawHealth ();
	}

	void DrawHealth ()
	{
		Rect backPos = new Rect ();
		backPos.x = Screen.width / 3 + 20;
		backPos.y = Screen.height - 25;
		backPos.width = 400;
		backPos.height = 20;
		GUI.DrawTexture (backPos, currentHP_Background);

		Rect healthPos = new Rect ();
		healthPos.x = Screen.width / 3 + 20;
		healthPos.y = Screen.height - 25;
		healthPos.width = 400 * (health / maxHealth);
		healthPos.height = 20;
		GUI.DrawTexture (healthPos, currentHP_Foreground);
	}
}















