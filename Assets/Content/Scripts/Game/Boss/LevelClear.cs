using UnityEngine;
using System.Collections;

public class LevelClear : MonoBehaviour 
{
	private Color fade;									// The color that allows the level clear sprite to fade in.
	[SerializeField] private float fadeTime		= 1;	// The time it takes for the level to fade in.
	private float lifePercent;

	void Start ()
	{
		fade = Color.white;
		fade.a = 0;
	}

	void Update ()
	{
		GetComponent<SpriteRenderer> ().color = fade;
	}

	void LevelCleared ()
	{
		CharacterController_BossStage player = GameObject.FindGameObjectWithTag ("BossPlayer").GetComponent<CharacterController_BossStage> ();
		CharacterStats stats = GameObject.FindGameObjectWithTag ("BossStage").GetComponent<CharacterStats> ();
		switch (player.character)
		{
		case CharacterController_BossStage.Character.Homura:
			player.lifeTimer += stats.characterStats.homura.lifeBonus;
			if (player.lifeTimer > stats.characterStats.homura.lifeTimer)
				player.lifeTimer = stats.characterStats.homura.lifeTimer;
			lifePercent = player.lifeTimer / stats.characterStats.homura.lifeTimer;
			break;
		case CharacterController_BossStage.Character.Kyouko:
			player.lifeTimer += stats.characterStats.kyouko.lifeBonus;
			if (player.lifeTimer > stats.characterStats.kyouko.lifeTimer)
				player.lifeTimer = stats.characterStats.kyouko.lifeTimer;
			lifePercent = player.lifeTimer / stats.characterStats.kyouko.lifeTimer;
			break;
		case CharacterController_BossStage.Character.Madoka:
			player.lifeTimer += stats.characterStats.madoka.lifeBonus;
			if (player.lifeTimer > stats.characterStats.madoka.lifeTimer)
				player.lifeTimer = stats.characterStats.madoka.lifeTimer;
			lifePercent = player.lifeTimer / stats.characterStats.madoka.lifeTimer;
			break;
		case CharacterController_BossStage.Character.Mami:
			player.lifeTimer += stats.characterStats.mami.lifeBonus;
			if (player.lifeTimer > stats.characterStats.mami.lifeTimer)
				player.lifeTimer = stats.characterStats.mami.lifeTimer;
			lifePercent = player.lifeTimer / stats.characterStats.mami.lifeTimer;
			break;
		case CharacterController_BossStage.Character.Sayaka:
			player.lifeTimer += stats.characterStats.sayaka.lifeBonus;
			if (player.lifeTimer > stats.characterStats.sayaka.lifeTimer)
				player.lifeTimer = stats.characterStats.sayaka.lifeTimer;
			lifePercent = player.lifeTimer / stats.characterStats.sayaka.lifeTimer;
			break;
		}

		StartCoroutine (FadeTo (1, fadeTime));
		GetComponent<Animator> ().SetTrigger ("LevelClear");
		StartCoroutine ("ExitBossFight", 3);
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

	IEnumerator ExitBossFight (float time)
	{
		yield return new WaitForSeconds (time);

		GameObject.FindGameObjectWithTag ("LevelController").SendMessage ("SetLifePercent", lifePercent);
		GameObject.FindGameObjectWithTag ("LevelController").SendMessage ("ExitBossFight");
	}
}
