using UnityEngine;
using System.Collections;

public class MamiMovement_BossStage : MonoBehaviour 
{
	private bool bossFightStarted						= false;
	private bool loaded									= false;
	private Camera bossCamera;

	[SerializeField] private float speed				= 25.0f;	// Player's speed.
	[SerializeField] private float jumpHieght			= 3.0f;		// How high the player jumps.
	[SerializeField] private float jumpSpeed			= 45.0f;	// How fast the player jumps through the air.
	[SerializeField] private GameObject bullet;						// Bullet GameObject.
	[SerializeField] private GameObject bullet_SFX;
	[SerializeField] private GameObject firework;					// Firework GameoObject.
	[SerializeField] private GameObject explosion_SFX;
	[SerializeField] private GameObject attack3_Gun;				// Attack3's gun GameObject.
	[SerializeField] private float attack1State			= 0.05f;	// If player will do attack1 or attack1_2.
	[SerializeField] private GameObject attack1_Spawn;				// Spawn point for attack1 bullet.
	[SerializeField] private GameObject attack1_2_1_Spawn;			// Spawn point for attack1_2.1 bullet.
	[SerializeField] private GameObject attack1_2_2_Spawn;			// Spawn point for attack1_2.2 bullet.
	[SerializeField] private GameObject attack2_1_Spawn;			// Spawn point for attack2.1 firework.
	[SerializeField] private GameObject attack2_2_Spawn;			// Spawn point for attack2.2 firework.
	[SerializeField] private GameObject[] attack3_Spawn;			// Spawn points for attack3.

	private Animator animController;								// Animation controller.
	private bool canMove								= true;		// If the player can move or not.
	private bool moving									= false;	// If the player is moving.
	private bool jumping								= false;	// If the player is jumping.
	private int direction 								= 1;		// The direction the player is facing

	private bool attacking								= false;	// If the player is attacking.
	private bool attack1								= false;	// The state of attack1.
	private bool attack1_2								= false;	// The state of attack1_2.
	private int attack1_2State							= 1;		// Wether to spawn a bullet at attack1_2.1_Spawn or attack1_2.2_Spawn.
	private bool attack2								= false;	// The state for attack2.
	private bool attack3								= false;	// The state of attack3.
	private int attack3_SpawnPoint						= 0;		// The current point to spawn attack3_Gun.
	private bool special								= false;	// The state of special attack.

	private bool injured								= false;	// If the player got injured.
	private bool dead									= false;	// If the player is dead.
	private bool levelCleared							= false;	// If the level has been cleared.

	// Use this for initialization
	void Start () 
	{
		animController = GetComponentInChildren<Animator> ();
		bossCamera = GameObject.FindGameObjectWithTag ("BossCamera").GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!injured && !dead && !levelCleared)
		{
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
		attack1 = false;
		attack1_2 = false;
		animController.SetBool ("Attack1_2", false);
		attack2 = false;
		attack3 = false;
		attack3_SpawnPoint = 0;
		animController.SetBool ("Attack3", false);
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
		attack1 = false;
		attack1_2 = false;
		animController.SetBool ("Attack1_2", false);
		attack2 = false;
		attack3 = false;
		attack3_SpawnPoint = 0;
		animController.SetBool ("Attack3", false);
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
		attack1 = false;
		attack1_2 = false;
		animController.SetBool ("Attack1_2", false);
		attack2 = false;
		attack3 = false;
		attack3_SpawnPoint = 0;
		animController.SetBool ("Attack3", false);
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

	void Attack ()
	{
	// Attack 1
		if (Input.GetMouseButton (0) || Input.GetKey (KeyCode.Z))
		{
			if (!attack1_2 && !attacking)
			{
				attacking = true;
				StartCoroutine ("Attack1State", attack1State);
			}
		}
		
	// Attack 2
		if (Input.GetMouseButton (1) || Input.GetKey (KeyCode.X))
		{
			if (moving)
			{
				if (!attack2 && !attacking)
				{
					attack2 = true;
					attacking = true;

					animController.SetTrigger ("Attack2");
					StartCoroutine ("Attack2", 0.46f);
					StartCoroutine (HaltMovement (1.0f, 3));
				}
			}
			else
			{
				if (!attack3 && !attacking)
				{
					attack3 = true;
					attacking = true;

					animController.SetBool ("Attack3", true);
					StartCoroutine ("Attack3", 0.1f);
					StartCoroutine (HaltMovement (0.2f, 4));
				}
			}
		}
	}

	IEnumerator Attack1State (float time)
	{
		yield return new WaitForSeconds (time);

		if (Input.GetMouseButton (0) || Input.GetKey (KeyCode.Z))
		{
			attack1_2 = true;
			StartCoroutine ("Attack1_2", 0.5f);
			animController.SetBool ("Attack1_2", true);
			StartCoroutine (HaltMovement (1.30f, 2));
		}
		else
		{
			attack1 = true;
			StartCoroutine ("Attack1", 0.5f);
			animController.SetTrigger ("Attack1");
			StartCoroutine (HaltMovement (1.0f, 1));
		}
	}

	IEnumerator Attack1 (float time)
	{
		yield return new WaitForSeconds (time);

		GameObject cloneBullet = (GameObject) Instantiate (bullet, attack1_Spawn.transform.position, transform.rotation);
		cloneBullet.name = bullet.name;
		GameObject cloneSFX = (GameObject) Instantiate (bullet_SFX, attack1_Spawn.transform.position, transform.rotation);
	}

	IEnumerator Attack1_2 (float time)
	{
		yield return new WaitForSeconds (time);

		switch (attack1_2State)
		{
		case 1:
			GameObject cloneBullet1 = (GameObject) Instantiate (bullet, attack1_2_1_Spawn.transform.position, transform.rotation);
			cloneBullet1.name = bullet.name;
			attack1_2State = 2;
			StartCoroutine ("Attack1_2", 0.8f);
			break;
		case 2:
			GameObject cloneBullet2 = (GameObject) Instantiate (bullet, attack1_2_2_Spawn.transform.position, transform.rotation);
			cloneBullet2.name = bullet.name;
			attack1_2State = 1;

			if (attack1_2)
			{
				StartCoroutine ("Attack1_2", 0.69f);
			}
			break;
		}
		GameObject cloneSFX = (GameObject) Instantiate (bullet_SFX, attack1_Spawn.transform.position, transform.rotation);
	}

	IEnumerator Attack2 (float time)
	{
		yield return new WaitForSeconds (time);

		GameObject cloneFirework1 = (GameObject) Instantiate (firework, attack2_1_Spawn.transform.position, attack2_1_Spawn.transform.rotation);
		cloneFirework1.name = firework.name;

		GameObject cloneFirework2 = (GameObject) Instantiate (firework, attack2_2_Spawn.transform.position, attack2_2_Spawn.transform.rotation);
		cloneFirework2.name = firework.name;

		GameObject cloneSFX = (GameObject) Instantiate (explosion_SFX,transform.position, transform.rotation);
	}

	IEnumerator Attack3 (float time)
	{
		yield return new WaitForSeconds (time);

		if (attack3_SpawnPoint < attack3_Spawn.Length)
		{
			GameObject cloneAttack3_Gun = (GameObject) Instantiate (attack3_Gun, attack3_Spawn[attack3_SpawnPoint].transform.position, transform.rotation);
			cloneAttack3_Gun.transform.parent = attack3_Spawn[attack3_SpawnPoint].transform;

			attack3_SpawnPoint++;
		}

		if (Input.GetMouseButton (1) || Input.GetKey (KeyCode.X))
		{
			StartCoroutine ("Attack3", 0.1f);
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
				attacking = false;
				break;
			case 2:
				StartCoroutine (HaltMovement (1.30f, 2));
				break;
			case 3:
				attacking = false;
				attack2 = false;
				break;
			case 4:
				StartCoroutine (HaltMovement (0.2f, 4));
				break;
			}
		}
		else
		{
			if (attack1)
			{
				attack1 = false;
				attacking = false;
			}
			if (attack1_2)
			{
				attack1_2 = false;
				animController.SetBool ("Attack1_2", false);
				attacking = false;
			}
			if (attack2)
			{
				attack2 = false;
				attacking = false;
			}
			if (attack3)
			{
				attack3 = false;
				animController.SetBool ("Attack3", false);
				StopCoroutine ("Attack3");
				attack3_SpawnPoint = 0;
				gameObject.BroadcastMessage ("Fire", SendMessageOptions.DontRequireReceiver);
				attacking = false;
			}
			canMove = true;
		}
	}

	void LevelCleared ()
	{
		levelCleared = true;
		StopAllCoroutines ();

		canMove = true;
		moving = false;
		animController.SetBool ("Moving", false);
		attacking = false;
		attack1 = false;
		attack1_2 = false;
		animController.SetBool ("Attack1_2", false);
		attack2 = false;
		attack3 = false;
		attack3_SpawnPoint = 0;
		animController.SetBool ("Attack3", false);

		if (jumping)
		{
			jumping = false;
			rigidbody2D.velocity = new Vector2 (0, -jumpSpeed);
		}
	}
}






































