using UnityEngine;
using System.Collections;

public class CharacterController_BossStage : MonoBehaviour 
{
	public enum Character
	{
		Homura,
		Kyouko,
		Madoka,
		Mami,
		Sayaka
	}
	public Character character;						// The character that is being played.

	private bool injured									= false;	// If the player was recently hit.
	[SerializeField] private float iFrameLength				= 1.0f;		// How long the player will be invisible for after recieving damage.
	[SerializeField] private float iFrameBlinkTimer			= 0.25f;	// When the player gets hit, how long the player will be invisible for in the "blinking notifier"
	[SerializeField] private SpriteRenderer characterRenderer;			// The character's sprite renderer.
	
	[SerializeField] private float healthRegenTime			= 5.0f;		// The amount of time before the health regens.
	public float lifeTimer;												// The amount of time that the player can live for.
	private float lifePercent;
	public float health;												// The amount of health the player has.
	private float maxHealth;											// The maximum amount of health the player can have.
	private bool regenHealth								= false;	// If the player can regen health.

	[SerializeField] private GameObject deadTempParticles;				// The particle effect for being temporarily dead.
	[SerializeField] private GameObject deadParticles;					// The particle effect for dying.
	[SerializeField] private float deathTimer				= 5.0f;		// The amount of time that the player is temporarily dead for.
	[SerializeField] private float deathSpeed				= 20.0f;	// The speed that the player moves at when dead.
	private float deathPenalty;											// The amount of time that is taken off the lifeTimer Upon death;
	private bool deadTemp									= false;	// If the player is temporarily dead.
	public bool dead										= false;	// If the player is dead.

	private bool levelCleared								= false;	// If the level is cleared or not.

	// Use this for initialization
	void Start () 
	{
		SetCharacterStats ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!levelCleared)
			lifeTimer -= Time.deltaTime;
		if (lifeTimer <= 0.0f && !dead)
		{
			KillFromLifeTimer ();
		}

		if (regenHealth)
		{
			if (deadTemp)
			{
				health += Time.deltaTime * deathTimer;
				lifeTimer -= Time.deltaTime * deathPenalty;
			}
			else
			{
				health += Time.deltaTime * 5;
				lifeTimer -= Time.deltaTime * 10;
			}

			if (health >= maxHealth)
			{
				regenHealth = false;
				health = maxHealth;
			}
		}

		if (deadTemp)
			DeadMovement ();

//		// used to fix the bug where the character sometimes disapears after being injured.
//		if (!injured && !dead && !deadTemp)
//		{
//			if (!gameObject.GetComponentInChildren<SpriteRenderer> ().enabled)
//				gameObject.GetComponentInChildren<SpriteRenderer> ().enabled = true;
//		}
	}

	void SetCharacterStats ()
	{
		CharacterStats stats = GameObject.FindGameObjectWithTag ("BossStage").GetComponent<CharacterStats> ();
		switch(character)
		{
		case Character.Homura:
			lifeTimer 		= stats.characterStats.homura.lifeTimer;
			health 			= stats.characterStats.homura.health;
			deathPenalty	= stats.characterStats.homura.deathPenalty;
			GameObject.FindGameObjectWithTag ("GUI").SendMessage ("Character", 1, SendMessageOptions.DontRequireReceiver);
			break;
		case Character.Kyouko:
			lifeTimer 		= stats.characterStats.kyouko.lifeTimer;
			health 			= stats.characterStats.kyouko.health;
			deathPenalty	= stats.characterStats.kyouko.deathPenalty;
			GameObject.FindGameObjectWithTag ("GUI").SendMessage ("Character", 2, SendMessageOptions.DontRequireReceiver);
			break;
		case Character.Madoka:
			lifeTimer 		= stats.characterStats.madoka.lifeTimer;
			health 			= stats.characterStats.madoka.health;
			deathPenalty	= stats.characterStats.madoka.deathPenalty;
			GameObject.FindGameObjectWithTag ("GUI").SendMessage ("Character", 3, SendMessageOptions.DontRequireReceiver);
			break;
		case Character.Mami:
			lifeTimer 		= stats.characterStats.mami.lifeTimer;
			health 			= stats.characterStats.mami.health;
			deathPenalty	= stats.characterStats.mami.deathPenalty;
			GameObject.FindGameObjectWithTag ("GUI").SendMessage ("Character", 4, SendMessageOptions.DontRequireReceiver);
			break;
		case Character.Sayaka:
			lifeTimer 		= stats.characterStats.sayaka.lifeTimer;
			health 			= stats.characterStats.sayaka.health;
			deathPenalty	= stats.characterStats.sayaka.deathPenalty;
			GameObject.FindGameObjectWithTag ("GUI").SendMessage ("Character", 5, SendMessageOptions.DontRequireReceiver);
			break;
		default:
			Debug.LogError ("No character was selected. Please select a character in the Inspector to set the character's stats.");
			break;
		}
		lifeTimer = lifeTimer * lifePercent;
		maxHealth = health;
	}

	void SetLifePercent (float f)
	{
		lifePercent = f;
	}

	void Damage (int amount)
	{
		if (!injured && !dead && !deadTemp)
		{
			health -= amount;
			if (health <= 0)
			{
				if (health < 0)
					health= 0;
				KillFromHealth ();
			}
			else
			{
				regenHealth = false;

				gameObject.SendMessage ("Injured");

				StopCoroutine ("WaitToRegen");
				StartCoroutine ("WaitToRegen", healthRegenTime);
				StartCoroutine (IFrame (iFrameLength, iFrameBlinkTimer));
			}
		}
	}

	IEnumerator WaitToRegen (float time)
	{
		yield return new WaitForSeconds (time);
		regenHealth = true;
	}
	
	IEnumerator IFrame (float duration, float blinkTime)
	{
		while (duration > 0.0f)
		{
			injured = true;
			duration -= Time.deltaTime;
			characterRenderer.enabled = !characterRenderer.enabled;
			
			yield return new WaitForSeconds (blinkTime);
		}
		characterRenderer.enabled = true;
		injured = false;
	}

	void KillFromHealth ()
	{
		lifeTimer -= deathPenalty;
		if (lifeTimer > 0.0f)
		{
			if (character == Character.Madoka)
			{
				dead = true;
				characterRenderer.enabled = false;
				gameObject.SendMessage ("DeadTemp", true, SendMessageOptions.DontRequireReceiver);
				StartCoroutine ("AltTempDeath", 1.6f);
			}
			else
			{
				dead = true;
				gameObject.SendMessage ("DeadTemp", true, SendMessageOptions.DontRequireReceiver);
				StartCoroutine ("Respawn", 1);
			}
		}
		else
			KillFromLifeTimer ();
	}

	IEnumerator Respawn (float time)
	{
		yield return new WaitForSeconds (time);

		StartCoroutine (IFrame (0.25f, iFrameBlinkTimer * 2));
		StartCoroutine ("Respawn2", 2f);
	}

	IEnumerator Respawn2 (float time)
	{
		yield return new WaitForSeconds (time);

		GetComponent<Rigidbody2D> ().isKinematic = true;
		characterRenderer.enabled = false;
		deadTemp = true;
		deadTempParticles.GetComponent<ParticleSystem> ().Play ();

		regenHealth = true;
		StartCoroutine ("TempDeathTimer");
	}

// ALTERNATE DEATH USED TO REPLACE MADOKA'S MISSING DEATH SPRITES.
	IEnumerator AltTempDeath (float time)
	{
		yield return new WaitForSeconds (time);

		GetComponent<Rigidbody2D> ().isKinematic = true;
		deadTemp = true;
		deadTempParticles.GetComponent<ParticleSystem> ().Play ();
		
		regenHealth = true;
		StartCoroutine ("TempDeathTimer");
	}

	IEnumerator TempDeathTimer ()
	{
		while (health < maxHealth)
		{
			yield return null;
		}

		dead = false;
		deadTemp = false;
		GetComponent<Rigidbody2D> ().isKinematic = false;
		characterRenderer.enabled = true;
		deadTempParticles.GetComponent<ParticleSystem> ().Stop ();
		gameObject.SendMessage ("DeadTemp", false, SendMessageOptions.DontRequireReceiver);
	}

	void DeadMovement ()
	{
	// MOVE UP -- W
		if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow))
		{
			transform.position += Vector3.up * (deathSpeed * Time.deltaTime);
		}

	// MOVE LEFT -- A
		if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow))
		{
			transform.position += Vector3.left * (deathSpeed * Time.deltaTime);
		}

	// MOVE DOWN -- S
		if (Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow))
		{
			transform.position += Vector3.down * (deathSpeed * Time.deltaTime);
		}

	// MOVE RIGHT -- D
		if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow))
		{
			transform.position += Vector3.right * (deathSpeed * Time.deltaTime);
		}
	}

	void KillFromLifeTimer ()
	{
		StopAllCoroutines ();
		gameObject.SendMessage ("DeadPerm", SendMessageOptions.DontRequireReceiver);
		dead = true;
		health = 0;

		if (character == Character.Madoka)
		{
			characterRenderer.enabled = false;
			StartCoroutine ("StartDeadParticles", 1.6f);
		}
		else 
		{
			StartCoroutine ("StartDeadParticles", 1.41f);
		}
	}

	IEnumerator StartDeadParticles (float time)
	{
		yield return new WaitForSeconds (time);

		deadParticles.GetComponent<ParticleSystem> ().Play ();
		if (character == Character.Madoka)
			StartCoroutine ("EndGame", 11);
		else
			StartCoroutine ("EndGame", 8);
		StartCoroutine ("RemovePlayer", 3.0f);
	}

	IEnumerator RemovePlayer (float time)
	{
		yield return new WaitForSeconds (time);

		characterRenderer.enabled = false;
	}

	IEnumerator EndGame (float time)
	{
		yield return new WaitForSeconds (time);

		GameObject.FindGameObjectWithTag ("LevelController").SendMessage ("RemoveLevel");
		SendMessageUpwards ("RemoveBossStage");
	}

	void LevelCleared ()
	{
		levelCleared = true;
	}
}














