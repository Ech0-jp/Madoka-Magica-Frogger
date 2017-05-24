using UnityEngine;
using System.Collections;

public class MagicCharge : MonoBehaviour 
{
	[SerializeField] private float waitForNextCharge;			// The amount of time to wait for the next charge;
	private int chargeState								= 1;	// The state of the charge (max of 3);
	private float size;											// The original scale of the object.

	// Use this for initialization
	void Start () 
	{
		size = transform.localScale.x;
		StartCoroutine ("NextCharge", waitForNextCharge);
	}
	
	IEnumerator NextCharge (float time)
	{
		yield return new WaitForSeconds (time);

		switch (chargeState)
		{
		case 1:
			transform.localScale = new Vector3 (size * 1.5f, size * 1.5f, 1);
			transform.localPosition = new Vector3 (0.05f, 0.05f, 0);
			break;
		case 2:
			transform.localScale = new Vector3 (size * 2, size * 2, 1);
			transform.localPosition = new Vector3 (0.1f, 0.15f, 0);
			break;
		default:
			break;
		}
		chargeState ++;

		if (chargeState < 3)
			StartCoroutine ("NextCharge", waitForNextCharge);
	}
}
