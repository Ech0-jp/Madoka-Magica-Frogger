using UnityEngine;
using System.Collections;

/// <summary>
/// Player controller.
/// 
/// This controls the players state (dead or alive), the amount of lives that the player has and the players movement.
/// 
/// </summary>

public class PlayerController : MonoBehaviour 
{
// PLAYER VARIABLES.
	public enum PlayerCharacter
	{
		Akemi_Homura,
		Kaname_Madoka,
		Miki_Sayaka,
		Sakura_Kyoko,
		Tomoe_Mami
	}
	public PlayerCharacter playerCharacter;

	private enum PlayerState
	{
		Dead,
		Alive
	}
	private PlayerState playerState = PlayerState.Alive;	// The state of the player.
	[SerializeField] private float deathTimer;				// How long the player is dead for.
	private float resetDeathTimer;							// Resets the deathTimer to its original value.

	[HideInInspector] public int score;						// The players score.
	[HideInInspector] public int level = 1;					// The level the player is on.
	public float lifeTimer;									// The amount of time that the player lives for.
	private float resetLifeTimer;							// The original value of the players life timer.
	private float deathPenalty;								// How much time is taken from the life timer upon death.
	private GameObject respawn;								// Where the player will spawn when it gets hit by a car or falls into the water (if the player still has lives).

	private int safeZoneCount;								// The amount of safe zones the player must reach.
	private bool safeZone;									// If the player reached the safezone.
	[SerializeField] private float safeZoneSpawnTimer;		// The timer for when the player reaches the safezone, send player to the original respawn.
	private float resetSafeZoneSpawnTimer;

// SPECIAL VARIABLES
	private bool pauseTime;									// Homura's special ability ... Some movement properties will not apply while true.
	private bool dash;										// Kyoko's special ability ... Allows her to dash over / through game objects.
	private float dashSpeed;								// The speed of Kyoko's dash.
	private bool canUseSpecial = true;						// If the character can use their special.
	private float specialCooldowntimer;						// The cooldown timer of the special ability.
	private float resetSpecialCooldownTimer;				// The value to reset the cooldown timer.
	private float specialDurationTimer;						// The duration of the special (if required).
	private float resetSpecialDurationTimer;				// The calue to reset the duration timer.

// MOVEMENT VARIABES.
	private float speed;									// The speed of the players movement.
	[SerializeField] private float travelDistance;			// How far forward the player travels.
	private Vector3 targetLocation;							// The location the player travels to.
	public bool isMoving;									// If the player is moving or not.
	private bool vMoving;									// If the player is moving in the verticle direction.
	public bool canMove = true;								// If the player can move or not.
	private bool onLog;										// if the player is on a log.
	private float logSpeed;									// The speed of the log that the player is on.
	private int logDirection;								// The direction the log that the player is on is moving (-1 = left || 1 = right).

// ANIMATION VARIABLES
	protected Animator animator;
	private enum IdleState
	{
		IdleUp		= 0,
		IdleRight	= 1,
		IdleDown	= 2,
		IdleLeft	= 3
	}
	private IdleState idleState		= IdleState.IdleUp;
	private bool keyDown;

	Vector3 posMax;
	Vector3 posMin;
	public bool bossStage			= false;
	[HideInInspector] public bool levelComplete		= false;

	void Start ()
	{
		GetCharacterProperties ();
	// Sends a message to the camera so it can draw the correct amount of lives.
	// Sets the target position to the starting position.
		animator = GetComponentInChildren<Animator> ();
		targetLocation = transform.position;
		resetDeathTimer = deathTimer;
		resetSafeZoneSpawnTimer = safeZoneSpawnTimer;
	}

	void Update ()
	{
		// Runs the special ability funtion
		SpecialAbility ();
	}

	void FixedUpdate ()
	{
	// PLAYER PROPERTIES
		if (playerState == PlayerState.Dead)
		{
			if (lifeTimer > 0.0f)
			{
				deathTimer -= Time.deltaTime;
				if (deathTimer <= 0.0f)
				{
					transform.position = Vector3.MoveTowards (transform.position, respawn.transform.position, speed * Time.deltaTime);
					if (transform.position == respawn.transform.position)
					{
						Respawn ();
						deathTimer = resetDeathTimer;
					}
				}
			}
		}
		if (playerState == PlayerState.Alive && !bossStage && !levelComplete)
		{
			lifeTimer -= Time.deltaTime;
			if (lifeTimer <= 0.0f)
			{
				KillPlayer ();
			}
		}

		if (safeZone)
		{
			safeZoneSpawnTimer -= Time.deltaTime;
			if (safeZoneSpawnTimer <= 0.0f)
			{
				RespawnSafeZone ();
			}
		}

	// PLAYER MOVEMENT.
	// Gets the size of the screen so the player cannot travel off of the screen.
		if (!bossStage)
		{
			posMax = Camera.main.ViewportToWorldPoint(new Vector3 (1, 1, Camera.main.nearClipPlane));
			posMin = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, Camera.main.nearClipPlane));
		}

		if (canMove) // Used for smooth animation purposes.
		{
			if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow) 
			    || Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow) 
			    || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow)
			    || Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow))
			{
				keyDown = true;
			}
			else
			{
				keyDown = false;
			}
		}

	// Checks for up and down
		if (!isMoving)
		{
			if (playerState == PlayerState.Alive)
			{
				if (!safeZone)
				{
					if (canMove)
					{
						if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow)) // Move up.
						{
							targetLocation = new Vector3(transform.position.x, transform.position.y + travelDistance, transform.position.z);
							idleState = IdleState.IdleUp;
							animator.SetInteger ("IdleState", 0);
							isMoving = true;
							vMoving = true;
						}
						if (Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow)) // Move down.
						{
							targetLocation = new Vector3(transform.position.x, transform.position.y - travelDistance, transform.position.z);
							idleState = IdleState.IdleDown;
							animator.SetInteger ("IdleState", 2);
							isMoving = true;
							vMoving = true;
						}
					}
				}
			}
		}
	// Moves the player up and down.
		else if (isMoving)
		{
			if (playerState == PlayerState.Alive)
			{
				if (transform.position != targetLocation)
				{
					switch(idleState) // Sets the animation state for movement.
					{
					case IdleState.IdleUp:
						animator.SetInteger ("Direction", 1);
						if (dash)
							transform.position = Vector3.MoveTowards (transform.position, targetLocation, dashSpeed * Time.deltaTime);
						else 
							transform.position = Vector3.MoveTowards (transform.position, targetLocation, speed * Time.deltaTime);
						break;
					case IdleState.IdleDown:
						animator.SetInteger ("Direction", 3);
						transform.position = Vector3.MoveTowards (transform.position, targetLocation, speed * Time.deltaTime);
						break;
					}
					if (transform.position == targetLocation)
					{
						
						if (dash)
							dash = false;
						if (!keyDown)
							animator.SetInteger ("Direction", 0);
						isMoving = false;
						vMoving = false;
					}
				}
			}
		}
	// Moves the player left and right
		if (!vMoving)
		{
			if (playerState == PlayerState.Alive)
			{
				if (canMove)
				{
					if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow)) // Move right.
					{
						if (transform.position.x + 1.5f < posMax.x || onLog)
						{
							if (onLog && !pauseTime)
								transform.position += Vector3.right * ((speed + (logSpeed * logDirection)) * Time.deltaTime);
							else
								transform.position += Vector3.right * (speed * Time.deltaTime);
							targetLocation = transform.position;
							idleState = IdleState.IdleRight;
							animator.SetInteger ("IdleState", 1);
							animator.SetInteger ("Direction", 2);
							isMoving = true;
						}
					}
					else if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow)) // Move left.
					{
						if (transform.position.x - 1.5f > posMin.x || onLog)
						{
							if (onLog && !pauseTime)
								transform.position += Vector3.left * ((speed - (logSpeed * logDirection)) * Time.deltaTime);
							else
								transform.position += Vector3.left * (speed * Time.deltaTime);
							targetLocation = transform.position;
							idleState = IdleState.IdleLeft;
							animator.SetInteger ("IdleState", 3);
							animator.SetInteger ("Direction", 4);
							isMoving = true;
						}
					}
					else 
					{
						if (!keyDown)
							animator.SetInteger ("Direction", 0);
						isMoving = false;
					}
				}
			}
		}

	// Makes it so the player cannot travel off of the screen.
		if (targetLocation.y >= posMax.y - 1 || targetLocation.y <= posMin.y + 1)
		{
			targetLocation = transform.position;
			animator.SetInteger ("Direction", 0);
			isMoving = false;
		}
		if (transform.position.x > posMax.x - 0.5f || transform.position.x < posMin.x + 0.5f)
		{
			KillPlayer ();
		}
	}

	void GetCharacterProperties ()
	{
		CharacterClasses characterClasses = GameObject.FindGameObjectWithTag ("LevelController").GetComponent<CharacterClasses> ();
		switch (playerCharacter)
		{
		case PlayerCharacter.Akemi_Homura:
			lifeTimer = characterClasses.akemiHomura.lifeTimer;
			deathPenalty = characterClasses.akemiHomura.deathPenalty;
			speed = characterClasses.akemiHomura.speed;
			specialCooldowntimer = characterClasses.akemiHomura.specialCooldownTimer;
			specialDurationTimer = characterClasses.akemiHomura.specialDurationTimer;
			break;
		case PlayerCharacter.Kaname_Madoka:
			lifeTimer = characterClasses.kanameMadoka.lifeTimer;
			deathPenalty = characterClasses.kanameMadoka.deathPenalty;
			speed = characterClasses.kanameMadoka.speed;
			break;
		case PlayerCharacter.Miki_Sayaka:
			lifeTimer = characterClasses.mikiSayaka.lifeTimer;
			deathPenalty = characterClasses.mikiSayaka.deathPenalty;
			speed = characterClasses.mikiSayaka.speed;
			break;
		case PlayerCharacter.Sakura_Kyoko:
			lifeTimer = characterClasses.sakuraKyoko.lifeTimer;
			deathPenalty = characterClasses.sakuraKyoko.deathPenalty;
			speed = characterClasses.sakuraKyoko.speed;
			specialCooldowntimer = characterClasses.sakuraKyoko.specialCooldownTimer;
			dashSpeed = characterClasses.sakuraKyoko.dashSpeed;
			break;
		case PlayerCharacter.Tomoe_Mami:
			lifeTimer = characterClasses.tomoMami.lifeTimer;
			deathPenalty = characterClasses.tomoMami.deathPenalty;
			speed = characterClasses.tomoMami.speed;
			specialCooldowntimer = characterClasses.tomoMami.specialCooldownTimer;
			break;
		}
		resetLifeTimer = lifeTimer;
		resetSpecialCooldownTimer = specialCooldowntimer;
		resetSpecialDurationTimer = specialDurationTimer;
	}

// Uses the players special ability
	void SpecialAbility ()
	{
		if (Input.GetKeyDown (KeyCode.Space) && canUseSpecial)
		{
			switch (playerCharacter)
			{
			case PlayerCharacter.Akemi_Homura:
				pauseTime = !pauseTime;
				GameObject.FindGameObjectWithTag ("LevelController").BroadcastMessage ("SetPauseTime", pauseTime, SendMessageOptions.DontRequireReceiver);
				break;
			case PlayerCharacter.Sakura_Kyoko:
				if (canUseSpecial)
				{
					dash = true;
					targetLocation = new Vector3 (transform.position.x, transform.position.y + (travelDistance * 2), transform.position.z);
					idleState = IdleState.IdleUp;
					animator.SetInteger ("IdleState", 0);
					isMoving = true;
					vMoving = true;
					canUseSpecial = false;
				}
				break;
			case PlayerCharacter.Tomoe_Mami:
				GameObject.FindGameObjectWithTag ("LevelController").BroadcastMessage ("KillEnemy", SendMessageOptions.DontRequireReceiver);
				canUseSpecial = false;
				break;
			}
		}

		switch(playerCharacter)
		{
		case PlayerCharacter.Akemi_Homura:
				if ((specialDurationTimer < resetSpecialDurationTimer && specialDurationTimer > 0.0f) && !pauseTime)
				{
					specialDurationTimer += Time.deltaTime / specialCooldowntimer;
				}
				else if (specialDurationTimer <= 0.0f)
				{
					canUseSpecial = false;
					pauseTime = false;
					GameObject.FindGameObjectWithTag ("LevelController").BroadcastMessage ("SetPauseTime", pauseTime, SendMessageOptions.DontRequireReceiver);
					specialCooldowntimer -= Time.deltaTime;
					if (specialCooldowntimer <= 0.0f)
					{
						specialCooldowntimer = resetSpecialCooldownTimer;
						specialDurationTimer = resetSpecialDurationTimer;
						canUseSpecial = true;
					}
				}

				if (pauseTime)
				{
					specialDurationTimer -= Time.deltaTime;
				}
			break;
		case PlayerCharacter.Sakura_Kyoko:
			if (!canUseSpecial)
			{
				specialCooldowntimer -= Time.deltaTime;
				if (specialCooldowntimer <= 0.0f)
				{
					specialCooldowntimer = resetSpecialCooldownTimer;
					canUseSpecial = true;
				}
			}
			break;
		}
	}

// Sets where the player will spawn to when they die.
	void SetSpawn (GameObject other)
	{
		respawn = other;
	}

// If player dies, respawn at the last safe location.
	void Respawn ()
	{
		transform.position = respawn.transform.position;
		targetLocation = respawn.transform.position;
		animator.SetBool ("Dead", false);
		playerState = PlayerState.Alive;
		isMoving = false;
		vMoving = false;
	}

// If the player reaches the safezone, 
	void RespawnSafeZone ()
	{
		transform.position = respawn.transform.position;
		targetLocation = respawn.transform.position;
		safeZone = false;
		safeZoneSpawnTimer = resetSafeZoneSpawnTimer;
	}

	void SetNewLifeTimer (float f)
	{
		lifeTimer = resetLifeTimer * f;
	}

	void SafeZoneCleared (int i)
	{
		AddScore (i);
		if (safeZoneCount == 0)
		{
			Camera.main.SendMessage ("LevelUp", SendMessageOptions.DontRequireReceiver);
		}
	}

// Kills the player if they attempt to jump on an already taken safezone.
	void TakenSafeZone ()
	{
		KillPlayer ();
	}

	void KillPlayer ()
	{
		if (playerState == PlayerState.Alive)
		{
			lifeTimer -= deathPenalty;
			playerState = PlayerState.Dead;
			animator.SetBool ("Dead", true);
			animator.SetInteger ("Direction", 0);
			animator.SetInteger ("IdleState", 0);
			idleState = IdleState.IdleUp;

			if (lifeTimer <= 0.0f)
			{
				GetComponentInChildren<SpriteRenderer> ().enabled = false;
				Camera.main.SendMessage ("GameOver", SendMessageOptions.DontRequireReceiver);
				Camera.main.SendMessage ("EndGame", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

// Increments the safeZoneCount when called.
	void SetSafeZoneCount ()
	{
		safeZoneCount++;
	}

// Resets values on new level.
	void LevelUp ()
	{
		level++;
		lifeTimer = resetLifeTimer;
		levelComplete = true;
		Camera.main.SendMessage ("GetLevel", level, SendMessageOptions.DontRequireReceiver);
	}

// Adds to the players score.
	void AddScore (int other)
	{
		score += other;
		GameObject.FindGameObjectWithTag ("MainCamera").SendMessage ("GetScore", score, SendMessageOptions.DontRequireReceiver);
	}

// Increases players lifeTimer upon certain events.
	void AddToLifeTimer ()
	{
		lifeTimer += deathPenalty;
		if (lifeTimer > resetLifeTimer)
			lifeTimer = resetLifeTimer;
	}

// Gets the speed of the log that the player is on.
	void GetLogSpeed (float other)
	{
		logSpeed = other;
	}

// Gets the direction of the log that the player is on.
	void GetLogDirection (int other)
	{
		logDirection = other;
	}

// When the player gets hit remove a life.
	void OnTriggerEnter2D (Collider2D other)
	{
		if (playerState == PlayerState.Alive)
		{
			if ((other.tag == "Vehicle" || other.tag == "Water") && !dash)
			{
				KillPlayer ();
			}

			if (other.tag == "SafeZone")
			{
				if (!safeZone)
				{
					other.SendMessage ("SafeZoneReached", safeZoneSpawnTimer, SendMessageOptions.DontRequireReceiver);
					SafeZoneReached ();
					safeZone = true;
					safeZoneCount--;
				}
			}

			if (other.tag == "Log")
			{
				onLog = true;
			}
		}
	}

	void SafeZoneReached ()
	{
		if (!bossStage)
		{
			switch (playerCharacter)
			{
			case PlayerCharacter.Akemi_Homura:
				GameObject.FindGameObjectWithTag ("LevelController").SendMessage ("SafeZoneReached", 1, SendMessageOptions.DontRequireReceiver);
				break;
			case PlayerCharacter.Sakura_Kyoko:
				GameObject.FindGameObjectWithTag ("LevelController").SendMessage ("SafeZoneReached", 2, SendMessageOptions.DontRequireReceiver);
				break;
			case PlayerCharacter.Kaname_Madoka:
				GameObject.FindGameObjectWithTag ("LevelController").SendMessage ("SafeZoneReached", 3, SendMessageOptions.DontRequireReceiver);
				break;
			case PlayerCharacter.Tomoe_Mami:
				GameObject.FindGameObjectWithTag ("LevelController").SendMessage ("SafeZoneReached", 4, SendMessageOptions.DontRequireReceiver);
				break;
			case PlayerCharacter.Miki_Sayaka:
				GameObject.FindGameObjectWithTag ("LevelController").SendMessage ("SafeZoneReached", 5, SendMessageOptions.DontRequireReceiver);
				break;
			}
			GameObject.FindGameObjectWithTag ("LevelController").SendMessage ("SetLifePercent", lifeTimer / resetLifeTimer, SendMessageOptions.DontRequireReceiver);
			canMove = false;
			bossStage = true;
		}
	}

	void BossStage (bool other)
	{
		canMove = !other;
		bossStage = other;
	}

	void OnTriggerExit2D (Collider2D other)
	{
		if (other.tag == "Log")
		{
			onLog = false;
		}
	}
}



































