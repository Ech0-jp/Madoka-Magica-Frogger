using UnityEngine;
using System.Collections;

public class KyoukoMovement_BossStage : MonoBehaviour 
{
	private bool bossFightStarted						= false;
	private bool loaded									= false;
	private Camera bossCamera;

	[SerializeField] private float speed				= 25.0f;	// Player's speed.
	[SerializeField] private float jumpHieght			= 3.0f;		// How high the player jumps.
	[SerializeField] private float jumpSpeed			= 45.0f;	// How fast the player jumps through the air.
	[SerializeField] private float waitForNextCharge	= 2.5f;		// How fast the charge will increase.
	[SerializeField] private float attack2_Duration		= 2.5f;		// How long the player will lung for.
	[SerializeField] private ParticleSystem attack2_Particles;		// The particle effect for attack2.
	[SerializeField] private ParticleSystem attack2_Charge;			// The charging particles for attack2.
	[SerializeField] private GameObject attackCollider_Right;		// The collider for the attacks.
	[SerializeField] private GameObject attackCollider_Left;		// The collider for the attacks.

	private Animator animController;								// Animation controller.
	private bool canMove								= true;		// If the player can move or not.
	private bool moving									= false;	// If the player is moving.
	private bool jumping								= false;	// If the player is jumping.
	private int direction 								= 1;		// The direction the player is facing

	private int charge									= 1;		// The state of the charge.

	private bool attacking								= false;	// If the player is attacking.
	private bool attack1_1								= false;	// State for attack1.1
	private int attack1_1Count							= 0;		// Amount of attack1.1's have been done (used to move to attack1.2)
	private bool attack1_2								= false;	// State for attack1.2
	private int attack1_2Count							= 0;		// Amount of attack1.2's have been done (used to finish attack1.2);
	private bool attack2								= false;	// State for attack2
	private bool canAttack2_2							= false;	// If the anim attack2_3 can play yet.
	private bool attack2_Move							= false;	// Moves the player forward during attack2.
	private bool attack3								= false;	// State for attack3
	private bool attack3Move							= false;	// Moves the player forward in jolts when using attack3.
	private bool special								= false;	// State for special attack.

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

		if (attack3Move)
		{
			Vector3 posMax = bossCamera.ViewportToWorldPoint(new Vector3 (1, 1, bossCamera.nearClipPlane));
			Vector3 posMin = bossCamera.ViewportToWorldPoint (new Vector3 (0, 0, bossCamera.nearClipPlane));
			if (transform.position.x < posMax.x - 2.5f && transform.position.x > posMin.x + 2.5f)
				transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x + (1 * direction), transform.position.y, transform.position.z), speed * Time.deltaTime);
		}

		if (attack2_Move)
		{
			Vector3 posMax = bossCamera.ViewportToWorldPoint(new Vector3 (1, 1, bossCamera.nearClipPlane));
			Vector3 posMin = bossCamera.ViewportToWorldPoint (new Vector3 (0, 0, bossCamera.nearClipPlane));
			if (transform.position.x < posMax.x - 2.5f && transform.position.x > posMin.x + 2.5f)
				transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x + (1 * direction), transform.position.y, transform.position.z), (speed * 3) * Time.deltaTime);
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
		attack1_1 = false;
		animController.SetBool ("Attack1_1", false);
		attack1_2 = false;
		animController.SetBool ("Attack1_2", false);
		attack2 = false;
		attack2_Move = false;
		animController.SetBool ("Attack2_1", false);
		animController.SetBool ("Attack2_2", false);
		attack2_Particles.Stop();
		attack2_Charge.Stop ();
		attack2_Charge.startSize = 4;
		attack3 = false;
		attack3Move = false;

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
		attack1_1 = false;
		animController.SetBool ("Attack1_1", false);
		attack1_2 = false;
		animController.SetBool ("Attack1_2", false);
		attack2 = false;
		attack2_Move = false;
		animController.SetBool ("Attack2_1", false);
		animController.SetBool ("Attack2_2", false);
		attack2_Particles.Stop();
		attack2_Charge.Stop ();
		attack2_Charge.startSize = 4;
		attack3 = false;
		attack3Move = false;
		
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
		attack1_1 = false;
		animController.SetBool ("Attack1_1", false);
		attack1_2 = false;
		animController.SetBool ("Attack1_2", false);
		attack2 = false;
		attack2_Move = false;
		animController.SetBool ("Attack2_1", false);
		animController.SetBool ("Attack2_2", false);
		attack2_Particles.Stop();
		attack2_Charge.Stop ();
		attack2_Charge.startSize = 4;
		attack3 = false;
		attack3Move = false;

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
			if (!attack1_1 && !attack1_2 && !attacking)
			{
				attack1_1 = true;
				attacking = true;
				animController.SetBool ("Attack1_1", true);

				StartCoroutine ("Activate_Attack1_1", 0.31f);
				StartCoroutine ("Attack1_1", 0.417f);
				StartCoroutine (HaltMovement (0.25f, 1));
			}
		}
		
		// Attack 2
		if (Input.GetMouseButton (1) || Input.GetKey (KeyCode.X))
		{
			if (moving)
			{
				if (!attack3 && !attacking)
				{
					attack3 = true;
					attacking = true;
					StartCoroutine (HaltMovement (1.00f, 3));
					animController.SetTrigger ("Attack3");
					attack3Move = true;

					StartCoroutine ("Activate_Attack3", 0.36f);
					StartCoroutine (Attack3 (0.8f));
				}
			}
			else
			{
				if (!attack2 && !attacking)
				{
					attack2 = true;
					attacking = true;
					animController.SetBool ("Attack2_1", true);
					animController.SetBool ("Attack2_2", true);
					StartCoroutine ("WaitForAttack2", 0.15f);
				}
			}
		}
		else if (attack2 && attacking)
		{
			if (canAttack2_2)
			{
				attack2 = false;
				animController.SetBool ("Attack2_1", false);
				attack2_Charge.Stop ();
				attack2_Charge.startSize = 4;
				StopCoroutine ("NextCharge");
				StartCoroutine ("Attack2_2_Start", 0.21f);
			}
		}
	}

	IEnumerator Attack1_1 (float time)
	{
		yield return new WaitForSeconds (time);

		attack1_1Count++;
		if (attack1_1Count == 3)
		{
			attack1_1Count = 0;
			attack1_1 = false;
			attack1_2 = true;
			animController.SetBool ("Attack1_2", true);
			animController.SetBool ("Attack1_1", false);

			StartCoroutine ("Activate_Attack1_2", 0.27f);
			StartCoroutine ("Attack1_2", 0.75f);
		}
		else 
			StartCoroutine ("Attack1_1", 0.417f);
	}

	IEnumerator Activate_Attack1_1 (float time)
	{
		yield return new WaitForSeconds (time);

		if (attack1_1 && attacking)
		{
			switch (direction)
			{
			case 1:
				attackCollider_Right.SendMessage ("SetAttack", 1, SendMessageOptions.DontRequireReceiver);
				break;
			case -1:
				attackCollider_Left.SendMessage ("SetAttack", 1, SendMessageOptions.DontRequireReceiver);
				break;
			}
		}
	}

	IEnumerator Attack1_2 (float time)
	{
		yield return new WaitForSeconds (time);

		attack1_2Count ++;
		if (attack1_2Count == 4)
		{
			attack1_2Count = 0;
			attack1_2 = false;
			attacking = false;
			animController.SetBool ("Attack1_2", false);
		}
		else
			StartCoroutine ("Attack1_2", 0.75f);
	}

	IEnumerator Activate_Attack1_2 (float time)
	{
		yield return new WaitForSeconds (time);

		if (attack1_2 && attacking)
		{
			switch (direction)
			{
			case 1:
				attackCollider_Right.SendMessage ("SetAttack", 2, SendMessageOptions.DontRequireReceiver);
				break;
			case -1:
				attackCollider_Left.SendMessage ("SetAttack", 2, SendMessageOptions.DontRequireReceiver);
				break;
			}
		}
	}

	IEnumerator WaitForAttack2 (float time)
	{
		yield return new WaitForSeconds (time);

		canAttack2_2 = true;
		attack2_Charge.Play ();
		StartCoroutine ("NextCharge", waitForNextCharge);
	}

	IEnumerator NextCharge (float time)
	{
		yield return new WaitForSeconds (time);
		
		charge ++;
		attack2_Charge.startSize += 4;
		if (charge < 3)
		{
			if (attack2)
				StartCoroutine ("NextCharge", waitForNextCharge);
		}
	}

	IEnumerator Attack2_2_Start (float time)
	{
		yield return new WaitForSeconds (time);

		switch (direction)
		{
		case 1:
			attackCollider_Right.SendMessage ("SetAttack", 3, SendMessageOptions.DontRequireReceiver);
			attackCollider_Right.SendMessage ("SetCharge", charge, SendMessageOptions.DontRequireReceiver);
			break;
		case -1:
			attackCollider_Left.SendMessage ("SetAttack", 3, SendMessageOptions.DontRequireReceiver);
			attackCollider_Left.SendMessage ("SetCharge", charge, SendMessageOptions.DontRequireReceiver);
			break;
		}

		attack2_Move = true;
		attack2_Particles.Play ();
		StartCoroutine ("Attack2_2_End", attack2_Duration);
	}

	IEnumerator Attack2_2_End (float time)
	{
		yield return new WaitForSeconds (time);

		attackCollider_Left.SendMessage ("EndAttack", SendMessageOptions.DontRequireReceiver);
		attackCollider_Right.SendMessage ("EndAttack", SendMessageOptions.DontRequireReceiver);

		attack2_Move = false;
		attack2_Particles.Stop ();
		animController.SetBool ("Attack2_2", false);
		StartCoroutine ("FinishAttack2", 0.15f);
	}

	IEnumerator FinishAttack2 (float time)
	{
		yield return new WaitForSeconds (time);

		attacking = false;
		charge = 1;
	}
	
	IEnumerator Attack3 (float time)
	{
		yield return new WaitForSeconds (time);

		attack3Move = false;
	}

	IEnumerator Activate_Attack3 (float time)
	{
		yield return new WaitForSeconds (time);

		if (attack3 && attacking)
		{
			switch (direction)
			{
			case 1:
				attackCollider_Right.SendMessage ("SetAttack", 4, SendMessageOptions.DontRequireReceiver);
				break;
			case -1:
				attackCollider_Left.SendMessage ("SetAttack", 4, SendMessageOptions.DontRequireReceiver);
				break;
			}
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
				if (attack1_1)
					StartCoroutine (HaltMovement (0.12f, 1));
				else if (attack1_2)
					StartCoroutine (HaltMovement (0.22f, 1));
				break;
			case 3:
				attack3 = false;
				attacking = false;
				attackCollider_Left.SendMessage ("EndAttack", SendMessageOptions.DontRequireReceiver);
				attackCollider_Right.SendMessage ("EndAttack", SendMessageOptions.DontRequireReceiver);
				break;
			}
		}
		else
		{
			if (attack1_1)
			{
				attack1_1Count = 0;
				attack1_1 = false;
				StopCoroutine ("Attack1_1");
				animController.SetBool ("Attack1_1", false);
			}
			if (attack1_2)
			{
				attack1_2Count = 0;
				attack1_2 = false;
				animController.SetBool ("Attack1_2", false);
				StopCoroutine ("Attack1_2");
			}
			if (attack3)
			{
				attack3 = false;
			}
			attackCollider_Left.SendMessage ("EndAttack", SendMessageOptions.DontRequireReceiver);
			attackCollider_Right.SendMessage ("EndAttack", SendMessageOptions.DontRequireReceiver);
			attacking = false;
			canMove = true;
		}
	}

	void LevelCleared ()
	{
		levelCleared = true;
		StopAllCoroutines ();

		attackCollider_Left.SendMessage ("EndAttack", SendMessageOptions.DontRequireReceiver);
		attackCollider_Right.SendMessage ("EndAttack", SendMessageOptions.DontRequireReceiver);

		canMove = true;
		moving = false;
		animController.SetBool ("Moving", false);
		attacking = false;
		attack1_1 = false;
		animController.SetBool ("Attack1_1", false);
		attack1_2 = false;
		animController.SetBool ("Attack1_2", false);
		attack2 = false;
		attack2_Move = false;
		animController.SetBool ("Attack2_1", false);
		animController.SetBool ("Attack2_2", false);
		attack2_Particles.Stop();
		attack2_Charge.Stop ();
		attack2_Charge.startSize = 4;
		attack3 = false;
		attack3Move = false;

		if (jumping)
		{
			jumping = false;
			rigidbody2D.velocity = new Vector2 (0, -jumpSpeed);
		}
	}
}
