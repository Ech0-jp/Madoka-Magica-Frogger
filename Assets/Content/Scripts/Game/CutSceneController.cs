using UnityEngine;
using System.Collections;

public class CutSceneController : MonoBehaviour 
{
	private Animator animController;
	private GameObject player;

	void Start ()
	{
		animController = GetComponentInChildren<Animator> ();
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "BossPlayer")
		{
			player = other.gameObject;
			other.SendMessage ("StartCutScene", SendMessageOptions.DontRequireReceiver);
			GameObject.FindGameObjectWithTag ("BossCamera").SendMessage ("CutScene");
			StartCoroutine ("StartCutScene", 1.2f);
		}
	}

	IEnumerator StartCutScene (float time)
	{
		yield return new WaitForSeconds (time);

		animController.SetBool ("CutScene", true);
		StartCoroutine ("EndCutScene", 3f);
	}

	IEnumerator EndCutScene (float time)
	{
		yield return new WaitForSeconds (time);

		animController.SetBool ("CutScene", false);
		StartCoroutine ("DestroyObj", 1.37f);
	}

	IEnumerator DestroyObj (float time)
	{
		yield return new WaitForSeconds (time);

		GameObject.FindGameObjectWithTag ("BossController").SendMessage ("StartBossFight");
		player.SendMessage ("EndCutScene");
		Destroy (gameObject);
	}
}
