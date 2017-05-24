using UnityEngine;
using System.Collections;

public class Intro : MonoBehaviour 
{
	[SerializeField] private Texture2D intro1;
	[SerializeField] private Texture2D intro2;
	private Color fade;

	// Use this for initialization
	void Start () 
	{
		fade = new Color (0, 0, 0, 0);
		GetComponent<DisplayTextureFullScreen> ().graphic.texture = intro1;
		StartCoroutine (FadeIn (1, 1));
	}

	void Update ()
	{
		GetComponent<DisplayTextureFullScreen> ().GUIColor = fade;
	}

	IEnumerator FadeIn(float aValue, float aTime)
	{
		float alpha = fade.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			Color newColor = new Color(1, 1, 1, Mathf.Lerp (alpha, aValue, t));
			fade = newColor;
			yield return null;
		}
		StartCoroutine (Wait1 (10));
	}

	IEnumerator Wait1 (float time)
	{
		yield return new WaitForSeconds (time);

		StartCoroutine (FadeOut (0, 1));
	}

	IEnumerator FadeOut(float aValue, float aTime)
	{
		float alpha = fade.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			Color newColor = new Color(1, 1, 1, Mathf.Lerp (alpha, aValue, t));
			fade = newColor;
			yield return null;
		}
		GetComponent<DisplayTextureFullScreen> ().graphic.texture = intro2;
		StartCoroutine (FadeIn2 (1, 1));
	}

	IEnumerator FadeIn2(float aValue, float aTime)
	{
		float alpha = fade.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			Color newColor = new Color(1, 1, 1, Mathf.Lerp (alpha, aValue, t));
			fade = newColor;
			yield return null;
		}
		StartCoroutine (Wait2 (8));
	}

	IEnumerator Wait2 (float time)
	{
		yield return new WaitForSeconds (time);
		
		StartCoroutine (FadeOut2 (0, 1));
	}

	IEnumerator FadeOut2(float aValue, float aTime)
	{
		float alpha = fade.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			Color newColor = new Color(1, 1, 1, Mathf.Lerp (alpha, aValue, t));
			fade = newColor;
			yield return null;
		}
		Application.LoadLevelAdditive ("MainMenu");
		Destroy (gameObject);
	}
}






















