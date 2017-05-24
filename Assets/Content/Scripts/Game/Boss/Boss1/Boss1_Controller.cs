using UnityEngine;
using System.Collections;

/// <summary>
/// Boss1_ controller.
/// 
/// This controls the behavior of the Boss_1.
/// 
/// </summary>

public class Boss1_Controller : MonoBehaviour 
{
	[SerializeField] private GameObject cutSceneController;
	private Camera bossCamera;

	public int health											= 5000;		// The bosses health.
	[SerializeField] private float speed						= 25.0f;	// The bosses speed.
	private bool dead											= false;	// If the boss is dead.

	[SerializeField] private CircleCollider2D hitCollider_Left;				// The hit collider when the boss is facing left.
	[SerializeField] private CircleCollider2D hitCollider_Right;			// The hit collider when the boss is facing right.

	[SerializeField] private BoxCollider2D hitCollider_Chomp_Left;			// The hit collider when the boss is facing left during it's  chomp attack.
	[SerializeField] private BoxCollider2D hitCollider_Chomp_Right;			// The hit collider when the boss is facing right during it's chomp attack.

	[SerializeField] private CircleCollider2D hitCollider_Taunt_Left;		// The hit collider when the boss is taunting/toge attacking to the left.
	[SerializeField] private CircleCollider2D hitCollider_Taunt_Right;		// The hit collider when the boxx is taunting/toge attacking to the right.

	[SerializeField] private GameObject deathParticles;						// The particle system for the boss's death.

	[SerializeField] private float maxDistanceFromPlayer		= 40.0f;	// The max distance from the player before the boss is forced to move towards player.
	[SerializeField] private float minDistanceFromPlayer		= 20.0f;	// How close the boss can move towards the player.
	private float setDistance;												// how far away the boss will travel from the player.

	private GameObject player;												// The players transform.
	private Animator animController;										// The anim controller for the boss.

	private int direction										= -1;		// The direction the boss is facing.
	private bool isBusy;													// If the boss is currently doing something.
	[SerializeField] float minNextMoveTime						= 0.5f;		// The minimum amount of time to elapse before the boss makes it's next move.
	[SerializeField] float maxNextMoveTime						= 2.0f;		// The maximum amount of time to elapse before the boss makes it's next move.

	private bool walk;														// If the boss should walk.

	private bool taunt;														// If the boss is using it's taunt move.
	[SerializeField] private float maxTauntTime					= 2.0f;		// The maximum amount of time the boss will taunt.
	[SerializeField] private float minTauntTime					= 0.5f;		// The minumum amount of time the boss will taunt.

	private bool togeAttack;												// If the boss is using it's toge attack.
	[SerializeField] GameObject toge;										// The toge prefab for the togeAttack.
	[SerializeField] GameObject[] togePlaceHolders;							// The placeholders for the toges.
	[SerializeField] float togeAttackTime;									// The amount of time to elapse for the toge attack.

	private bool chompAttack;												// If the boss is using it's chomp attack.
	[SerializeField] private float maxChompAttackTime			= 2.0f;		// The maximum amount of time the boss will use chomp attack.
	[SerializeField] private float minChompAttackTime			= 0.5f;		// The minimum amount of time the boss will use chomp attack.

	private bool jumpAttack;												// If the boss is using it's jump attack.
	[SerializeField] private float jumpHieght					= 25.0f;	// How high up the boss will jump.
	[SerializeField] private float jumpAttackTime				= 5.0f;		// The amount of time that it takes for the jump attack to complete.
	[SerializeField] private float jumpAttack2Time				= 5.0f;		// The amount of time that it takes for the boss to fall to the ground.

	[SerializeField] private GameObject chompColliderLeft;					// The collider for the chomp attack when the boss is facing the left.
	[SerializeField] private GameObject chompColliderRight;					// The collider for the cjomp attack when the boss is facing the right.

	[SerializeField] private GameObject jumpColliderLeft;					// The collider for the jump attack when the boss is facing the left.
	[SerializeField] private GameObject jumpColliderRight;					// The collider for the jump attack when the boss is facing the right.

	// Use this for initialization
	void Start () 
	{
		animController = GetComponentInChildren<Animator> ();
		bossCamera = GameObject.FindGameObjectWithTag ("BossCamera").GetComponent<Camera> ();
	}

	void Update ()
	{
		if (health <= 0)
		{
			if (!dead)
				Dead ();
		}

		if (walk)
			MoveBoss ();

		if (chompAttack)
			transform.position += Vector3.right * (direction * ((speed / 2) * Time.deltaTime));

		if (jumpAttack)
		{
			rigidbody2D.gravityScale = 0;

			Vector3 posMax = bossCamera.ViewportToWorldPoint(new Vector3 (1, 1, bossCamera.nearClipPlane));
			Vector3 target = new Vector3 (transform.position.x, posMax.y + jumpHieght, transform.position.z);
			transform.position = Vector3.MoveTowards (transform.position, target, speed * Time.deltaTime);

			if (transform.position.y == target.y);
			{
				float distance = Mathf.Abs (player.transform.position.x - transform.position.x);
				if (distance > 10)
				{
					transform.position = Vector3.MoveTowards (transform.position, new Vector3 (player.transform.position.x, target.y, transform.position.z), speed * Time.deltaTime);
				}
			}
		}
	}

	void StartBossFight ()
	{
		player = GameObject.FindGameObjectWithTag ("BossPlayer");
		taunt = true;
		Taunt ();
	}

// Take's damage
	void Damage (int damage)
	{
		health -= damage;
	}

// DECISION FUNCTIONS
	void NextMove ()
	{
		if (!isBusy && !dead)
		{
			if (player.transform.position.x < transform.position.x)
			{
				if (direction != -1)
				{
					direction = -1;
					Quaternion rotation = new Quaternion (0, 180, 0, 0);
					hitCollider_Left.enabled = true;
					hitCollider_Right.enabled = false;
					transform.rotation = rotation;
				}
			}
			if (player.transform.position.x > transform.position.x)
			{
				if (direction != 1)
				{
					direction = 1;
					Quaternion rotation = new Quaternion (0, 0, 0, 0);
					hitCollider_Left.enabled = false;
					hitCollider_Right.enabled = true;
					transform.rotation = rotation;
				}
			}

			float distance = Vector3.Distance (player.transform.position, transform.position);
			if (distance > maxDistanceFromPlayer)
			{
				setDistance = Random.Range (minDistanceFromPlayer, maxDistanceFromPlayer);
				isBusy = true;
				walk = true;
			}
			else
			{
				int randMove = Mathf.RoundToInt (Random.Range (-0.49f, 4.49f));
				switch (randMove)
				{
				case 0:
					setDistance = Random.Range (minDistanceFromPlayer, maxDistanceFromPlayer);
					walk = true;
					break;
				case 1:
					taunt = true;
					Taunt ();
					break;
				case 2:
					togeAttack = true;
					TogeAttack ();
					break;
				case 3:
					chompAttack = true;
					ChompAttack ();
					break;
				case 4:
					jumpAttack = true;
					JumpAttack ();
					break;
				}
			}
		}
	}

	IEnumerator WaitForNextMove (float time)
	{
		yield return new WaitForSeconds (time);
		NextMove ();
	}

// KILLS THE BOSS
	void Dead ()
	{
		dead = true;
		gameObject.BroadcastMessage ("DestroyToge", SendMessageOptions.DontRequireReceiver);
		deathParticles.GetComponent<ParticleSystem> ().Play ();
		animController.SetTrigger ("Dead");
		StartCoroutine ("RemoveBoss", 3.0f);
		StartCoroutine ("ClearLevel", 10.0f);

		player.SendMessage ("LevelCleared", SendMessageOptions.DontRequireReceiver);
	}

	IEnumerator RemoveBoss (float time)
	{
		yield return new WaitForSeconds (time);

		GetComponentInChildren<SpriteRenderer> ().enabled = false;
	}

	IEnumerator ClearLevel (float time)
	{
		yield return new WaitForSeconds (time);

		GameObject.FindGameObjectWithTag ("BossCamera").BroadcastMessage ("LevelCleared", SendMessageOptions.DontRequireReceiver);
	}

// MOVE
	void MoveBoss ()
	{
		isBusy = true;

		animController.SetBool ("Walking", true);
		transform.position = Vector3.MoveTowards (transform.position, new Vector3 (player.transform.position.x, transform.position.y, transform.position.z), speed * Time.deltaTime);
		float distance = Vector3.Distance (player.transform.position, transform.position);
		if (distance < setDistance)
		{
			animController.SetBool ("Walking", false);
			walk = false;
			isBusy = false;

			float waitTime = Random.Range (minNextMoveTime, maxNextMoveTime);
			StartCoroutine ("WaitForNextMove", waitTime);
		}
	}

// TAUNT ATTACK
	void Taunt ()
	{
		isBusy = true;

		hitCollider_Left.enabled			= false;
		hitCollider_Right.enabled			= false;
		if (direction == -1)
			hitCollider_Taunt_Left.enabled	= true;
		else if (direction == 1)
			hitCollider_Taunt_Right.enabled	= true;

		animController.SetBool ("Attack1", true);
		float time = Random.Range (minTauntTime, maxTauntTime);
		StartCoroutine ("TauntCoroutine", time);
	}

	IEnumerator TauntCoroutine (float time)
	{
		yield return new WaitForSeconds (time);

		animController.SetBool ("Attack1", false);
		StartCoroutine ("ExitTaunt", 0.16);
	}

	IEnumerator ExitTaunt (float time)
	{
		yield return new WaitForSeconds (time);
		taunt = false;
		isBusy = false;

		hitCollider_Taunt_Left.enabled	= false;
		hitCollider_Taunt_Right.enabled	= false;
		if (direction == -1)
			hitCollider_Left.enabled			= true;
		else if (direction == 1)
			hitCollider_Right.enabled			= true;


		float waitTime = Random.Range (minNextMoveTime, maxNextMoveTime);
		StartCoroutine ("WaitForNextMove", waitTime);
	}

// TOGE ATTACK
	void TogeAttack ()
	{
		isBusy = true;
		animController.SetBool ("Attack1", true);

		hitCollider_Left.enabled			= false;
		hitCollider_Right.enabled			= false;
		if (direction == -1)
			hitCollider_Taunt_Left.enabled	= true;
		else if (direction == 1)
			hitCollider_Taunt_Right.enabled	= true;

		for (int i = 0; i < togePlaceHolders.Length; i++)
		{
			GameObject cloneToge = (GameObject) Instantiate (toge, togePlaceHolders[i].transform.position, transform.rotation);
			cloneToge.transform.parent = togePlaceHolders[i].transform;
			cloneToge.SendMessage ("GetDirection", direction, SendMessageOptions.DontRequireReceiver);
		}

		StartCoroutine ("TogeAttackCoroutine", togeAttackTime);
	}

	IEnumerator TogeAttackCoroutine (float time)
	{
		yield return new WaitForSeconds (time);

		animController.SetBool ("Attack1", false);
		isBusy = false;

		StartCoroutine ("ExitTogeAttack", 0.16f);
	}

	IEnumerator ExitTogeAttack (float time)
	{
		yield return new WaitForSeconds (time);

		hitCollider_Taunt_Left.enabled	= false;
		hitCollider_Taunt_Right.enabled	= false;
		if (direction == -1)
			hitCollider_Left.enabled			= true;
		else if (direction == 1)
			hitCollider_Right.enabled			= true;

		float waitTime = Random.Range (minNextMoveTime, maxNextMoveTime);
		StartCoroutine ("WaitForNextMove", waitTime);
	}

// CHOMP ATTACK
	void ChompAttack ()
	{
		isBusy = true;
		chompAttack = true;
		animController.SetBool ("Attack2", true);

		hitCollider_Left.enabled			= false;
		hitCollider_Right.enabled			= false;
		if (direction == -1)
			hitCollider_Chomp_Left.enabled	= true;
		else if (direction == 1)
			hitCollider_Chomp_Right.enabled	= true;

		float time = Random.Range (minChompAttackTime, maxChompAttackTime);
		StartCoroutine ("ChompAttackCoroutine", time);
		StartCoroutine ("EnableChompCollider", 0.4f);
	}

	IEnumerator ChompAttackCoroutine (float time)
	{
		yield return new WaitForSeconds (time);

		chompAttack = false;
		isBusy = false;
		animController.SetBool ("Attack2", false);
		chompColliderLeft.collider2D.enabled = false;
		chompColliderRight.collider2D.enabled = false;

		StartCoroutine ("ExitChompAttack", 0.3f);
	}

	IEnumerator ExitChompAttack (float time)
	{
		yield return new WaitForSeconds (time);

		hitCollider_Chomp_Left.enabled			= false;
		hitCollider_Chomp_Right.enabled			= false;
		if (direction == -1)
			hitCollider_Left.enabled			= true;
		else if (direction == 1)
			hitCollider_Right.enabled			= true;

		float waitTime = Random.Range (minNextMoveTime, maxNextMoveTime);
		StartCoroutine ("WaitForNextMove", waitTime);
	}

// JUMP ATTACK
	void JumpAttack ()
	{
		isBusy = true;
		StartCoroutine ("JumpAttackCoroutine", jumpAttackTime);
		StartCoroutine ("EnableJumpCollider", 0.4f);
		animController.SetBool("Attack2", true);

		hitCollider_Left.enabled			= false;
		hitCollider_Right.enabled			= false;
		if (direction == -1)
			hitCollider_Chomp_Left.enabled	= true;
		else if (direction == 1)
			hitCollider_Chomp_Right.enabled	= true;
	}

	IEnumerator JumpAttackCoroutine (float time)
	{
		yield return new WaitForSeconds (time);

		rigidbody2D.gravityScale = 2;
		jumpAttack = false;
		isBusy = false;
		StartCoroutine ("JumpAttackCoroutine2", jumpAttack2Time);
	}

	IEnumerator JumpAttackCoroutine2 (float time)
	{
		yield return new WaitForSeconds(time);

		jumpColliderLeft.collider2D.enabled = false;
		jumpColliderRight.collider2D.enabled = false;
		rigidbody2D.gravityScale = 1;
		animController.SetBool ("Attack2", false);
		StartCoroutine ("ExitJumpAttack", 0.3f);
	}

	IEnumerator ExitJumpAttack (float time)
	{
		yield return new WaitForSeconds (time);

		hitCollider_Chomp_Left.enabled			= false;
		hitCollider_Chomp_Right.enabled			= false;
		if (direction == -1)
			hitCollider_Left.enabled			= true;
		else if (direction == 1)
			hitCollider_Right.enabled			= true;

		float waitTime = Random.Range (minNextMoveTime, maxNextMoveTime);
		StartCoroutine ("WaitForNextMove", waitTime);
	}

	IEnumerator EnableChompCollider (float time)
	{
		yield return new WaitForSeconds (time);

		switch (direction)
		{
		case 1:
			chompColliderRight.collider2D.enabled = true;
			break;
		case -1:
			chompColliderLeft.collider2D.enabled = true;
			break;
		}
	}

	IEnumerator EnableJumpCollider (float time)
	{
		yield return new WaitForSeconds (time);
		
		switch (direction)
		{
		case 1:
			jumpColliderRight.collider2D.enabled = true;
			break;
		case -1:
			jumpColliderLeft.collider2D.enabled = true;
			break;
		}
	}
}

































