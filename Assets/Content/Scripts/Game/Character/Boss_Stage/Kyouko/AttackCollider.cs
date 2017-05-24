using UnityEngine;
using System.Collections;

public class AttackCollider : MonoBehaviour 
{
	[SerializeField] private GameObject bloodSpawn;
	[SerializeField] private GameObject blood;

	[SerializeField] private float attack1_1_MaxDamage		= 10;
	[SerializeField] private float attack1_1_MinDamage		= 5;
	[SerializeField] private float attack1_1_CD				= 0.11f;

	[SerializeField] private float attack1_2_MaxDamage		= 20;
	[SerializeField] private float attack1_2_MinDamage		= 10;
	[SerializeField] private float attack1_2_CD				= 0.20f;

	[SerializeField] private float attack2_MaxDamage_1		= 35;
	[SerializeField] private float attack2_MinDamage_1		= 20;

	[SerializeField] private float attack2_MaxDamage_2		= 50;
	[SerializeField] private float attack2_MinDamage_2		= 35;

	[SerializeField] private float attack2_MaxDamage_3		= 65;
	[SerializeField] private float attack2_MinDamage_3		= 50;
	[SerializeField] private float attack2_CD				= 0.05f;

	[SerializeField] private float attack3_MaxDamage		= 50;
	[SerializeField] private float attack3_MinDamage		= 35;
	[SerializeField] private float attack3_CD				= 0.15f;

	private bool attack1_1									= false;
	private bool attack1_2									= false;

	private bool attack2									= false;
	private int attack2_Charge								= 0;
	private int attack2_HitCount							= 0;

	private bool attack3									= false;
	private int attack3_HitCount							= 0;

	private bool canHit										= true;

	void SetAttack (int attack)
	{
		switch (attack)
		{
		case 1:
			attack1_1 = true;
			break;
		case 2:
			attack1_2 = true;
			break;
		case 3:
			attack2 = true;
			break;
		case 4:
			attack3 = true;
			break;
		}
	}

	void EndAttack ()
	{
		attack1_1 = false;
		attack1_2 = false;
		attack2 = false;
		attack2_HitCount = 0;
		attack3 = false;
		attack3_HitCount = 0;
	}

	void SetCharge (int charge)
	{
		attack2_Charge = charge;
	}

	void OnTriggerStay2D (Collider2D other)
	{
		if (other.tag == "Boss")
		{
			if (attack1_1)
			{
				Attack1_1 (other.gameObject);
			}
			else if (attack1_2)
			{
				Attack1_2 (other.gameObject);
			}
			else if (attack2)
			{
				Attack2 (other.gameObject);
			}
			else if (attack3)
			{
				Attack3 (other.gameObject);
			}
		}
	}

	void Attack1_1 (GameObject other)
	{
		if (canHit)
		{
			GameObject cloneBlood = (GameObject) Instantiate (blood, bloodSpawn.transform.position, bloodSpawn.transform.rotation);
			other.SendMessageUpwards ("Damage", Random.Range (attack1_1_MinDamage, attack1_1_MaxDamage + 1), SendMessageOptions.DontRequireReceiver);
			canHit = false;
			StartCoroutine ("HitCD", attack1_1_CD);
		}
	}

	void Attack1_2 (GameObject other)
	{
		if (canHit)
		{
			GameObject cloneBlood = (GameObject) Instantiate (blood, bloodSpawn.transform.position, bloodSpawn.transform.rotation);
			other.SendMessageUpwards ("Damage", Random.Range (attack1_2_MinDamage, attack1_2_MaxDamage + 1), SendMessageOptions.DontRequireReceiver);
			canHit = false;
			StartCoroutine ("HitCD", attack1_2_CD);
		}
	}

	void Attack2 (GameObject other)
	{
		if (canHit)
		{
			if (attack2_HitCount <= 5)
			{
				GameObject cloneBlood = (GameObject) Instantiate (blood, bloodSpawn.transform.position, bloodSpawn.transform.rotation);
				attack2_HitCount ++;
				switch (attack2_Charge)
				{
				case 1:
					other.SendMessageUpwards ("Damage", Random.Range (attack2_MinDamage_1, attack2_MaxDamage_1 + 1), SendMessageOptions.DontRequireReceiver);
					break;
				case 2:
					other.SendMessageUpwards ("Damage", Random.Range (attack2_MinDamage_2, attack2_MaxDamage_2 + 1), SendMessageOptions.DontRequireReceiver);
					break;
				case 3:
					other.SendMessageUpwards ("Damage", Random.Range (attack2_MinDamage_3, attack2_MaxDamage_3 + 1), SendMessageOptions.DontRequireReceiver);
					break;
				}
				canHit = false;
				StartCoroutine ("HitCD", attack2_CD);
			}
		}
	}

	void Attack3 (GameObject other)
	{
		if (canHit)
		{
			if (attack3_HitCount <= 2)
			{
				attack3_HitCount ++;
				GameObject cloneBlood = (GameObject) Instantiate (blood, bloodSpawn.transform.position, bloodSpawn.transform.rotation);
				other.SendMessageUpwards ("Damage", Random.Range (attack3_MinDamage, attack3_MaxDamage + 1), SendMessageOptions.DontRequireReceiver);
				canHit = false;
				StartCoroutine ("HitCD", attack3_CD);
			}
		}
	}

	IEnumerator HitCD (float time)
	{
		yield return new WaitForSeconds (time);

		canHit = true;
	}
}













