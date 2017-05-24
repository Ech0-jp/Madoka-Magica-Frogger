using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour 
{
	[SerializeField] private float time		= 5.0f;

	// Use this for initialization
	void Start () 
	{
		StartCoroutine ("WaitToDestroy", time);
	}

	IEnumerator WaitToDestroy (float wait)
	{
		yield return new WaitForSeconds (wait);

		Destroy (gameObject);
	}
}
