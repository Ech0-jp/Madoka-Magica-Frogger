using UnityEngine;
using System.Collections;

/// <summary>
/// Character movement_ boss stage.
/// 
/// This controls the movement, animations and shooting properties of Homura.
/// 
/// </summary>

public class HomuraMovement_BossStage : MonoBehaviour 
{
	private bool bossFightStarted						= false;
	private bool loaded									= false;
	private Camera bossCamera;

	[SerializeField] private float speed				= 25.0f;	// Player's speed.
	[SerializeField] private float jumpHieght			= 3.0f;		// How high the player jumps.
	[SerializeField] private float jumpSpeed			= 45.0f;	// How fast the player jumps through the air.
	[SerializeField] private float continueAttack1		= 0.5f;		// Time to wait to decide to continue attack1 or not.
	[SerializeField] private float kickBackForce		= 10.0f;	// The force of the kick back from attack2.
	[SerializeField] private GameObject bullet;						// Bullet GameObject.
	[SerializeField] private GameObject handgun_SFX;
	[SerializeField] private GameObject machinegun_SFX;
	[SerializeField] private GameObject rocket;						// Rocket GameObject.
	[SerializeField] private GameObject rocket_SFX;
	[SerializeField] private GameObject attack1Spawn;				// Place to spawn the bullet for attack1.
	[SerializeField] private GameObject attack2Spawn;				// Place to spawn the bullet for attack2.
	[SerializeField] private GameObject attack3Spawn;				// Place to spawn the rocket for attack3.

	private Animator animController;								// Animation controller.
	private bool canMove								= true;		// If the player can move or not.
	private bool moving									= false;	// If the player is moving.
	private bool jumping								= false;	// If the player is jumping.
	private int direction 								= 1;		// The direction the player is facing

	private bool attacking								= false;	// If the player is attacking.
	private int attack1									= 0;		// The state of attack1 (0 = none // 1 = starting // 2 = continuing).
	private bool attack2								= false;	// The state of attack2.
	private bool attack3								= false;	// The state of attack3.

	private bool spawnBulletCoroutine					= false;	// if the spawnBullet coroutine is running or not.
	private bool spawnRocketCoroutine					= false;	// If the spawnRocket coroutine is running or not.

	private bool injured								= false;	// If the player got injured.
	private bool dead									= false;	// If the player is dead.
	private bool levelCleared							= false;	// If the level has been cleared.

	// Use this for initialization
	void Start ()
	{
		animController = GetComponentInChildren<Animator>();
		bossCamera = GameObject.FindGameObjectWithTag ("BossCamera").GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!injured && !dead && !levelCleared)
		{
			if (loaded)
				Movement ();
			if (bossFightStarted)
				Attack ();

			if (jumping)
			{
				if (transform.position.y >= jumpHieght)
					rigidbody2D.velocity = new Vector2 (0, -jumpSpeed);
			}
		}
		else if (injured)
		{
			Vector3 posMax = bossCamera.ViewportToWorldPoint(new Vector3 (1, 1, bossCamera.nearClipPlane));
			Vector3 posMin = bossCamera.ViewportToWorldPoint (new Vector3 (0, 0, bossCamera.nearClipPlane));
			if (transform.position.x < posMax.x - 2.5f && transform.position.x > posMin.x + 2.5f)
				transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x - (1 * direction), transform.position.y, transform.position.z), 25 * Time.deltaTime);
		}
	}

	void LoadComplete ()
	{
		loaded = true;
	}

	void StartCutScene ()
	{
		canMove = false;
		moving = false;
		animController.SetBool ("Moving", false);
	}
	
	void EndCutScene ()
	{
		canMove = true;
		bossFightStarted = true;
	}

// Communication from CharacterController.cs, springs the injured anim and locks controls for the length of the anim.
	void Injured ()
	{
		injured = true;
		animController.SetTrigger ("Injured");

		canMove = true;
		moving = false;
		animController.SetBool ("Moving", false);
		attacking = false;
		attack1 = 0;
		animController.SetInteger ("Attack1", 0);
		attack2 = false;
		animController.SetBool ("Attack2", false);
		attack3 = false;
		animController.SetBool ("Attack3", false);
		spawnBulletCoroutine = false;
		spawnRocketCoroutine = false;

		if (jumping)
		{
			jumping = false;
			rigidbody2D.velocity = new Vector2 (0, -jumpSpeed);
		}

		StopAllCoroutines ();
		StartCoroutine ("InjuredTimer", 0.24f);
	}

	void DeadPerm ()
	{
		StopAllCoroutines ();
		dead = true;
		animController.SetTrigger ("Dead");

		canMove = false;
		moving = false;
		animController.SetBool ("Moving", false);
		attacking = false;
		attack1 = 0;
		animController.SetInteger ("Attack1", 0);
		attack2 = false;
		animController.SetBool ("Attack2", false);
		attack3 = false;
		animController.SetBool ("Attack3", false);
		spawnBulletCoroutine = false;
		spawnRocketCoroutine = false;
		
		if (jumping)
		{
			jumping = false;
			rigidbody2D.velocity = new Vector2 (0, -jumpSpeed);
		}
	}

	void DeadTemp (bool state)
	{
		dead = state;
		if (state == true)
			animController.SetTrigger ("Dead");

		canMove = true;
		moving = false;
		animController.SetBool ("Moving", false);
		attacking = false;
		attack1 = 0;
		animController.SetInteger ("Attack1", 0);
		attack2 = false;
		animController.SetBool ("Attack2", false);
		attack3 = false;
		animController.SetBool ("Attack3", false);
		spawnBulletCoroutine = false;
		spawnRocketCoroutine = false;

		if (jumping)
		{
			jumping = false;
			rigidbody2D.velocity = new Vector2 (0, -jumpSpeed);
		}
	}

// The amount of time the player has to wait to regain controls.
	IEnumerator InjuredTimer (float time)
	{
		yield return new WaitForSeconds (time);
		injured = false;
	}

// Checks for movement input and moves the character.
	void Movement ()
	{
		Vector3 posMax = bossCamera.ViewportToWorldPoint(new Vector3 (1, 1, bossCamera.nearClipPlane));
		Vector3 posMin = bossCamera.ViewportToWorldPoint (new Vector3 (0, 0, bossCamera.nearClipPlane));

		if (!attacking && canMove)
		{
			if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow))
			{
				if (direction != -1)
				{
					direction = -1;
					Quaternion rotation = new Quaternion (0, 180, 0, 0);
					transform.rotation = rotation;
				}

				if (!moving)
				{
					moving = true;
					animController.SetBool ("Moving", moving);
				}

				if (transform.position.x > posMin.x + 2.5f)
					transform.position += Vector3.left * (speed * Time.deltaTime);
			}
			else if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow))
			{
				if (direction != 1)
				{
					direction = 1;
					Quaternion rotation = new Quaternion (0, 0, 0, 0);
					transform.rotation = rotation;
				}

				if (!moving)
				{
					moving = true;
					animController.SetBool ("Moving", moving);
				}

				if (transform.position.x < posMax.x - 2.5f)
					transform.position += Vector3.right * (speed * Time.deltaTime);
			}
			else if (moving)
			{
				moving = false;
				animController.SetBool ("Moving", moving);
			}

			if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow))
			{
				if (!jumping)
				{
					jumping = true;
					rigidbody2D.velocity = new Vector2 (0, jumpSpeed);
					animController.SetTrigger ("Jump");
					StartCoroutine ("JumpingCoroutine", 1.09f);
				}
			}
		}
	}

	IEnumerator JumpingCoroutine (float time)
	{
		yield return new WaitForSeconds (time);

		jumping = false;
	}

// Checks for attack input and plays attack sequence.
	void Attack ()
	{
	// Attack 1
		if (Input.GetMouseButton (0) || Input.GetKey (KeyCode.Z))
		{
			if (!attacking)
			{
				StartCoroutine (ContinueAttack1());
				attack1 = 1;
				animController.SetInteger ("Attack1", attack1);
				attacking = true;

				canMove = false;
				StartCoroutine (HaltMovement (0.62f, 1));
				StartCoroutine (SpawnBullet (0.31f, attack1Spawn));
			}
			else if (!spawnBulletCoroutine)
				StartCoroutine (SpawnBullet (0.31f, attack1Spawn));
		}

	// Attack 2
		if (Input.GetMouseButton (1) || Input.GetKey (KeyCode.X))
		{
			if (moving)
			{
				if (!attack3 && !attacking)
				{
					attack3 = true;
					animController.SetBool ("Attack3", attack3);
					attacking = true;

					canMove = false;
					StartCoroutine (HaltMovement (1.3f, 3));
					StartCoroutine ("SpawnRocket", 0.65f);
				}
				else if (!spawnRocketCoroutine)
					StartCoroutine ("SpawnRocket", 0.65f);
			}
			else
			{
				if (!attack2 && !attacking)
				{
					attack2 = true;
					animController.SetBool ("Attack2", attack2);
					attacking = true;

					canMove = false;
					StartCoroutine (HaltMovement (0.55f, 2));
					StartCoroutine (SpawnBullet (0.12f, attack2Spawn));
				}
				else if (!spawnBulletCoroutine)
					StartCoroutine (SpawnBullet (0.12f, attack2Spawn));
			}
		}
	}

// When the bullet can spawn.
	IEnumerator SpawnBullet (float time, GameObject spawn)
	{
		spawnBulletCoroutine = true;
		if (attack2)
		{
			Vector3 posMax = bossCamera.ViewportToWorldPoint(new Vector3 (1, 1, bossCamera.nearClipPlane));
			Vector3 posMin = bossCamera.ViewportToWorldPoint (new Vector3 (0, 0, bossCamera.nearClipPlane));
			if (transform.position.x < posMax.x - 2.5f && transform.position.x > posMin.x + 2.5f)
				transform.position += Vector3.right * (-direction * (kickBackForce * Time.deltaTime));
		}
		yield return new WaitForSeconds (time);

		GameObject cloneMachingunSFX;
		GameObject clonePistolSFX;
		GameObject cloneBullet = (GameObject) Instantiate (bullet, spawn.transform.position, transform.rotation);
		cloneBullet.name = bullet.name;
		if (attack2)
			cloneMachingunSFX = (GameObject) Instantiate (machinegun_SFX, spawn.transform.position, transform.rotation);
		else 
			clonePistolSFX = (GameObject) Instantiate (handgun_SFX, spawn.transform.position, transform.rotation);

		if (attack1 > 0 || attack2)
		{
			StartCoroutine (SpawnBullet (time, spawn));
		}
		else 
			spawnBulletCoroutine = false;
	}

	IEnumerator SpawnRocket (float time)
	{
		spawnRocketCoroutine = true;
		yield return new WaitForSeconds (time);

		GameObject cloneRocket = (GameObject) Instantiate (rocket, attack3Spawn.transform.position, transform.rotation);
		GameObject cloneRocketSFX = (GameObject) Instantiate (rocket_SFX, attack3Spawn.transform.position, transform.rotation);
		cloneRocket.name = rocket.name;
		if (attack3)
		{
			StartCoroutine ("SpawnRocket", 1.5f);
		}
		else
			spawnRocketCoroutine = false;
	}

// The wait time to continue attack1.
	IEnumerator ContinueAttack1 ()
	{
		yield return new WaitForSeconds (continueAttack1);

		if (Input.GetMouseButton (0) || Input.GetKey (KeyCode.Z))
		{
			attack1 = 2;
			animController.SetInteger ("Attack1", attack1);
		}
	}

// Stops player movement till attack finishes.
	IEnumerator HaltMovement (float time, int attack)
	{
		yield return new WaitForSeconds (time);

		if (Input.GetMouseButton (0) || Input.GetKey (KeyCode.Z) || Input.GetMouseButton (1) || Input.GetKey (KeyCode.X))
		{
			switch (attack)
			{
			case 1:
				StartCoroutine (HaltMovement (0.5f, 1));
				break;
			case 2:
				StartCoroutine (HaltMovement (0.44f, 2));
				break;
			case 3:
				StartCoroutine (HaltMovement (1.5f, 3));
				break;
			}
		}
		else
		{
			if (attack1 > 0)
			{
				attack1 = 0;
				animController.SetInteger ("Attack1", attack1);
				attacking = false;
			}
			if (attack2)
			{
				attack2 = false;
				animController.SetBool ("Attack2", attack2);
				attacking = false;
			}
			if (attack3)
			{
				attack3 = false;
				animController.SetBool ("Attack3", attack3);
				StopCoroutine ("SpawnRocket");
				attacking = false;
			}
			canMove = true;
		}
	}

	void LevelCleared ()
	{
		levelCleared = true;
		StopAllCoroutines ();

		canMove = false;
		moving = false;
		animController.SetBool ("Moving", false);
		attacking = false;
		attack1 = 0;
		animController.SetInteger ("Attack1", 0);
		attack2 = false;
		animController.SetBool ("Attack2", false);
		attack3 = false;
		animController.SetBool ("Attack3", false);
		spawnBulletCoroutine = false;
		spawnRocketCoroutine = false;

		if (jumping)
		{
			jumping = false;
			rigidbody2D.velocity = new Vector2 (0, -jumpSpeed);
		}
	}
}




























