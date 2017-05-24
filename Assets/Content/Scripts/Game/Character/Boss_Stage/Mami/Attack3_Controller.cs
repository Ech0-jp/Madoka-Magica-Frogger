using UnityEngine;
using System.Collections;

public class Attack3_Controller : MonoBehaviour 
{
	[SerializeField] private GameObject bullet;				// The bullet GameObject.
	[SerializeField] private GameObject bullet_SFX;
	[SerializeField] private GameObject bulletSpawn;		// The point where the bullet will spawn.
	[SerializeField] private float maxWait		= 1.0f;		// The max amount of time before the gun shoots.
	[SerializeField] private float minWait		= 0.1f;		// The min amount of time before the gun shoots.

	void Fire ()
	{
		Debug.Log ("Firing attack3");
		StartCoroutine ("Wait", Random.Range (minWait, maxWait));
	}

	IEnumerator Wait (float time)
	{
		yield return new WaitForSeconds (time);

		GameObject cloneBullet = (GameObject) Instantiate (bullet, bulletSpawn.transform.position, transform.rotation);
		cloneBullet.name = bullet.name;
		GameObject cloneSFX = (GameObject) Instantiate (bullet_SFX, bulletSpawn.transform.position, transform.rotation);
		GetComponentInChildren<Animator> ().SetTrigger ("Finish");
		StartCoroutine ("DestroyGun", 0.5);
	}

	IEnumerator DestroyGun (float time)
	{
		yield return new WaitForSeconds (time);

		Destroy (gameObject);
	}
}
