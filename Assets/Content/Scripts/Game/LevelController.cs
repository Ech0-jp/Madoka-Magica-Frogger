using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour 
{
	private GameObject camera;
	private int character;
	private float lifePercent;
	private Color fade;
	[SerializeField] private int bossStageClearedScore			= 500;
	[SerializeField] private float fadeTime;					// Time it takes to fade.
	[SerializeField] private Texture2D[] loadScreens;
	[SerializeField] private float loadScreenDuration;
	private bool bossLevel										= false;

	void Start ()
	{
		camera = Camera.main.gameObject;
		fade = Color.white;
		fade.a = 0;
	}

	void FixedUpdate ()
	{
		GetComponent<DisplayTextureFullScreen>().GUIColor = fade;
	}

	void RemoveLevel ()
	{
		camera.GetComponent<Camera> ().enabled = true;
		camera.SendMessage ("GameOver");
	}

	void SafeZoneReached (int i)
	{
		if (!bossLevel)
		{
			bossLevel = true;
			character = i;
			LoadBossFight ();
		}
	}

	void SetLifePercent (float f)
	{
		lifePercent = f;
	}

	void LoadBossFight ()
	{
		Application.LoadLevelAdditive ("Boss1_Stage");
		int i = Random.Range (0, loadScreens.Length);
		GetComponent<DisplayTextureFullScreen>().graphic.texture = loadScreens[i];

		StartCoroutine (FadeTo (1, fadeTime));
		StartCoroutine ("LoadBossFight_IE");
		StartCoroutine ("LoadDuration", loadScreenDuration);
		StartCoroutine (LevelVisibility (1, false));
		camera.SendMessage ("BossStage", true);
	}

	void ExitBossFight ()
	{
		if (bossLevel)
		{
			int i = Random.Range (0, loadScreens.Length);
			GetComponent<DisplayTextureFullScreen>().graphic.texture = loadScreens[i];
			
			StartCoroutine (FadeTo (1, fadeTime));
			StartCoroutine ("LoadDuration", loadScreenDuration);
			StartCoroutine (LevelVisibility (1, true));
			camera.SendMessage ("BossStage", false);
			StartCoroutine ("RemoveBossStage", 2);
			camera.GetComponent<Camera> ().enabled = true;
			bossLevel = false;
			StartCoroutine ("SafeZoneCleared", loadScreenDuration + fadeTime);
		}
	}

	IEnumerator LevelVisibility (float time, bool visible)
	{
		yield return new WaitForSeconds (time);

		BroadcastMessage ("BossStage", !visible);
		foreach (Renderer rend in gameObject.GetComponentsInChildren<Renderer>())
		{
			rend.enabled = visible;
		}
	}

	IEnumerator RemoveBossStage (float time)
	{
		yield return new WaitForSeconds (time);

		GameObject.FindGameObjectWithTag ("BossStage").SendMessage ("RemoveBossStage");
	}

	IEnumerator LoadBossFight_IE ()
	{
		while (GameObject.Find ("BossLevel_Cont") == null)
		{
			yield return null;
		}
		GameObject.FindGameObjectWithTag ("BossStage").SendMessage ("SetLifePercent", lifePercent);
		GameObject.FindGameObjectWithTag ("BossStage").SendMessage ("LoadBossFight", character);
		camera.GetComponent<Camera> ().enabled = false;
	}

	IEnumerator LoadDuration (float time)
	{
		yield return new WaitForSeconds (time);

		StartCoroutine(FadeTo (0, fadeTime));
		if (bossLevel)
			GameObject.FindGameObjectWithTag ("BossPlayer").SendMessage ("LoadComplete");
	}

	IEnumerator SafeZoneCleared (float time)
	{
		yield return new WaitForSeconds (time);

		GameObject.FindGameObjectWithTag ("Player").SendMessage ("SetNewLifeTimer", lifePercent);
		GameObject.FindGameObjectWithTag ("Player").SendMessage ("SafeZoneCleared", bossStageClearedScore);
	}

	IEnumerator FadeTo(float aValue, float aTime)
	{
		float alpha = fade.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			Color newColor = new Color(1, 1, 1, Mathf.Lerp (alpha, aValue, t));
			fade = newColor;
			yield return null;
		}
	}
}








