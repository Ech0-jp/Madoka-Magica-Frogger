using UnityEngine;
using System.Collections;

public class LevelFinishedGUI : MonoBehaviour 
{
	[SerializeField] private GUISkin gameOverSkin;
	[SerializeField] private GUISkin levelUpSkin;
	[SerializeField] private Texture2D gameOverTexture;			// The texture that is displayed when the game is over.
	[SerializeField] private Texture2D levelUpTexture;			// The texture that is displayed when the player levels up.
	private Color fade;											// Used for the alpha color of the texture.
	[SerializeField] private float fadeTime;					// Time it takes to fade.

	private bool levelUp;										// If the player leveled up.
	private bool gameOver;										// If the game is over.

	void Start ()
	{
		fade = Color.black;
		fade.a = 0;
	}

	void FixedUpdate ()
	{
		GetComponent<DisplayTextureFullScreen>().GUIColor = fade;
	}

	void GameOver ()
	{
		GameObject.FindGameObjectWithTag ("LevelController").BroadcastMessage ("DestroyGameObject", SendMessageOptions.DontRequireReceiver);
		GetComponent<DisplayTextureFullScreen>().graphic.texture = gameOverTexture;
		StartCoroutine (FadeTo (1, fadeTime));
		gameOver = true;
	}

	void LevelUp ()
	{
		GameObject.FindGameObjectWithTag ("LevelController").BroadcastMessage ("DestroyGameObject", SendMessageOptions.DontRequireReceiver);
		GetComponent<DisplayTextureFullScreen>().graphic.texture = levelUpTexture;
		StartCoroutine (FadeTo (1, fadeTime));
		levelUp = true;
	}

	void OnGUI ()
	{
		GUI.depth = 0;

		if (gameOver)
			GameOverGUI ();

		if (levelUp)
			levelUpGUI ();
	}

	void GameOverGUI ()
	{
		GUI.skin = gameOverSkin;
		GUI.Label (new Rect (Screen.width / 2 - 75, 75, 100, 50), "Game Over!");
		if (GUI.Button (new Rect ((Screen.width / 2 - 100), 125, 200, 100), "New Game"))
		{
			PlayerController player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
			PlayerPrefs.SetInt ("NewLevel", player.level);
			PlayerPrefs.SetInt ("NewScore", player.score);
			gameOver = false;
			GameObject.FindGameObjectWithTag ("Menu").SendMessage ("GameOver");
			SendMessage ("EndGame");
			Destroy (GameObject.FindGameObjectWithTag ("LevelController"));
			GetComponent<DisplayTextureFullScreen>().graphic.texture = null;
		}
	}

	void levelUpGUI ()
	{
		GUI.skin = levelUpSkin;
		GUI.Label (new Rect (Screen.width / 2 - 100, 20, 100, 50), "Level Up!");
		if (GUI.Button (new Rect ((Screen.width / 2 + 25), 20, 200, 50), "Next Level"))
		{
			GameObject.FindGameObjectWithTag ("LevelController").SendMessage ("GenerateLevel", SendMessageOptions.DontRequireReceiver);
			GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController>().canMove = true;
			GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController>().levelComplete = false;
			GameObject.FindGameObjectWithTag ("Player").SendMessage ("LevelUp", SendMessageOptions.DontRequireReceiver);
			StartCoroutine (FadeTo (0, fadeTime));
			levelUp = false;
		}
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
