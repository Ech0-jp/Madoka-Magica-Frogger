using UnityEngine;
using System.Collections;

public class MadokaMovement_BossStage : MonoBehaviour 
{
	private bool bossFightStarted						= false;
	private bool loaded 								= false;
	private Camera bossCamera;

	[SerializeField] private float speed				= 25.0f;	// Player's speed.
	[SerializeField] private float jumpHieght			= 3.0f;		// How high the player jumps.
	[SerializeField] private float jumpSpeed			= 45.0f;	// How fast the player jumps through the air.

	[SerializeField] private GameObject charge;						// The "Charging Arrow".
	[SerializeField] private GameObject chargeSpawn;				// The spawn point for the "charging arrow".

	[SerializeField] private GameObject magic;						// The "Magic Charge".
	[SerializeField] private GameObject magicSpawn;					// The spawn point for the "Magic Charge".

	[SerializeField] private float waitForNextCharge;				// The amount of time to elapse to increase the charge.

	[SerializeField] private GameObject attackSpawn;				// The spawn point for attack 1 and 2.
	[SerializeField] private GameObject specialSpawn;				// The spawn point for the special attack.

	[SerializeField] private GameObject smallArrow;					// The small arrow GameObject used for attack1 and special attack.
	[SerializeField] private GameObject largeArrow1;				// The 1st large arrow GameObject used for attack2 charge 1 and 2.
	[SerializeField] private GameObject largeArrow2;				// The 2nd large arrow GameObject used for attack2 charge 3.
	[SerializeField] private GameObject arrow_SFX;
	[SerializeField] private int attack1Charge1Arrows	= 5;		// The amount of arrows shot from attack1 on it's first charge.
	[SerializeField] private int attack1Charge2Arrows	= 13;		// The amount of arrows shot from attack1 on it's second charge.
	[SerializeField] private int attack1Charge3Arrows	= 26;		// The amount of arrows shot from attack1 on it's third charge.

	[SerializeField] private ParticleSystem explodingDeathAnim;			// The alternate death animation part 1.
	[SerializeField] private float waitToReform;					// The time to elapse before her soul reforms.
	[SerializeField] private ParticleSystem reformingDeathAnim;			// The alternate death animation part 2.

	public int chargeState								= 1;		// The current charge.

	private Animator animController;								// Animation controller.
	private bool canMove								= true;		// If the player can move or not.
	private bool moving									= false;	// If the player is moving.
	private bool jumping								= false;	// If the player is jumping.
	private int direction 								= 1;		// The direction the player is facing

	private bool charged								= false;	// If the player has pulled out an arrow.
	private bool attacking								= false;	// If the player is attacking.
	private bool attack1								= false;	// The state of attack1.
	private bool attack2								= false;	// The state for attack2.
	private bool special								= false;	// The state of the special attack.

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

		gameObject.BroadcastMessage ("DestroyEffect", SendMessageOptions.DontRequireReceiver);
		canMove = true;
		moving = false;
		animController.SetBool ("Moving", false);
		attacking = false;
		attack1 = false;
		attack2 = false;
		chargeState = 1;
		animController.SetBool ("Attack1", false);
		animController.SetBool ("Attack2", false);
		animController.SetBool ("Attacking", false);
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
		animController.SetTrigger ("Injured");
		explodingDeathAnim.Play ();
		StartCoroutine ("DeathAnim", waitToReform);
		gameObject.BroadcastMessage ("DestroyEffect", SendMessageOptions.DontRequireReceiver);
		canMove = false;
		moving = false;
		animController.SetBool ("Moving", false);
		attacking = false;
		attack1 = false;
		attack2 = false;
		animController.SetBool ("Attack1", false);
		animController.SetBool ("Attack2", false);
		animController.SetBool ("Attacking", false);
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
		{
			animController.SetTrigger ("Injured");
			explodingDeathAnim.Play ();
			StartCoroutine ("DeathAnim", waitToReform);
		}
		gameObject.BroadcastMessage ("DestroyEffect", SendMessageOptions.DontRequireReceiver);
		canMove = true;
		moving = false;
		animController.SetBool ("Moving", false);
		attacking = false;
		attack1 = false;
		attack2 = false;
		animController.SetBool ("Attack1", false);
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

	IEnumerator DeathAnim (float time)
	{
		yield return new WaitForSeconds (time);

		reformingDeathAnim.Play ();
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
			if (!attack1 && !attacking)
			{
				attacking = true;
				attack1 = true;
				animController.SetBool ("Attack1", true);
				animController.SetBool ("Attacking", true);
				StartCoroutine ("SpawnCharge", 0.44f);
			}
		}
		if (attack1 && attacking)
		{
			if (!Input.GetMouseButton (0) && !Input.GetKey (KeyCode.Z))
			{
				if (charged)
				{
					gameObject.BroadcastMessage ("DestroyEffect", SendMessageOptions.DontRequireReceiver);
					StopCoroutine ("SpawnCharge");
					StopCoroutine ("NextCharge");
					attack1 = false;
					animController.SetBool ("Attack1", false);
					StartCoroutine ("Attack1", 0.1f);
				}
			}
		}
		
		// Attack 2
		if (Input.GetMouseButton (1) || Input.GetKey (KeyCode.X))
		{
			if (!attack2 && !attacking)
			{
				attacking = true;
				attack2 = true;
				animController.SetBool ("Attack1", true);
				StartCoroutine ("SpawnCharge", 0.44f);
			}
		}
		if (attack2 && attacking)
		{
			if (!Input.GetMouseButton (1) && !Input.GetKey (KeyCode.X))
			{
				if (charged)
				{
					gameObject.BroadcastMessage ("DestroyEffect", SendMessageOptions.DontRequireReceiver);
					StopCoroutine ("SpawnCharge");
					StopCoroutine ("NextCharge");
					attack2 = false;
					attacking = false;
					animController.SetBool ("Attack1", false);

					switch (chargeState)
					{
					case 1:
					case 2:
						GameObject cloneArrow1 = (GameObject) Instantiate (largeArrow1, attackSpawn.transform.position, transform.rotation);
						if (chargeState == 1)
							cloneArrow1.SendMessage ("ArrowState", 1, SendMessageOptions.DontRequireReceiver);
						else if (chargeState == 2)
							cloneArrow1.SendMessage ("ArrowState", 2, SendMessageOptions.DontRequireReceiver);
						break;
					case 3:
						GameObject cloneArrow2 = (GameObject) Instantiate (largeArrow2, attackSpawn.transform.position, transform.rotation);
						break;
					}
					GameObject cloneSFX = (GameObject) Instantiate (arrow_SFX, transform.position, transform.rotation);
					chargeState = 1;
				}
			}
		}
	}

	IEnumerator SpawnCharge (float time)
	{
		yield return new WaitForSeconds (time);

		GameObject cloneCharge = (GameObject) Instantiate (charge, chargeSpawn.transform.position, chargeSpawn.transform.rotation);
		cloneCharge.transform.parent = chargeSpawn.transform;

		GameObject cloneMagic = (GameObject) Instantiate (magic, magicSpawn.transform.position, magicSpawn.transform.rotation);
		cloneMagic.transform.parent = magicSpawn.transform;

		if (attack1 || attack2)
			StartCoroutine ("NextCharge", waitForNextCharge);

		charged = true;
	}

	IEnumerator NextCharge (float time)
	{
		yield return new WaitForSeconds (time);

		chargeState ++;
		if (chargeState < 3)
		{
			if (attack1 || attack2)
				StartCoroutine ("NextCharge", waitForNextCharge);
		}
	}

	IEnumerator Attack1 (float time)
	{
		int a = 0;
		switch (chargeState)
		{
		case 1:
			a = attack1Charge1Arrows;
			break;
		case 2:
			a = attack1Charge2Arrows;
			break;
		case 3:
			a = attack1Charge3Arrows;
			break;
		}
		for (int i = 0; i < a; i++)
		{
			GameObject cloneArrow = (GameObject) Instantiate (smallArrow, attackSpawn.transform.position, transform.rotation);
			GameObject cloneSFX = (GameObject) Instantiate (arrow_SFX, transform.position, transform.rotation);
			yield return new WaitForSeconds (time);
		}
		charged = false;
		chargeState = 1;
		attacking = false;
		animController.SetBool ("Attacking", false);
	}

	void LevelCleared ()
	{
		levelCleared = true;
		StopAllCoroutines ();
		
		gameObject.BroadcastMessage ("DestroyEffect", SendMessageOptions.DontRequireReceiver);
		canMove = true;
		moving = false;
		animController.SetBool ("Moving", false);
		attacking = false;
		attack1 = false;
		attack2 = false;
		animController.SetBool ("Attack1", false);
		animController.SetBool ("Attack2", false);
		animController.SetBool ("Attacking", false);

		if (jumping)
		{
			jumping = false;
			rigidbody2D.velocity = new Vector2 (0, -jumpSpeed);
		}
	}
}






























