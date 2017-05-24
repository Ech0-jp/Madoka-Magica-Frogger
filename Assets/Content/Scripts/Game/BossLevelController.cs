using UnityEngine;
using System.Collections;

public class BossLevelController : MonoBehaviour 
{
	[SerializeField] private Vector3 spawnPoint;
	private float percent;

	void SetLifePercent(float f)
	{
		percent = f;
	}

	void LoadBossFight (int character)
	{
		CharacterStats stats = GetComponent<CharacterStats> ();

		switch (character)
		{
		case 1:
			GameObject cloneHomura = (GameObject) Instantiate (stats.characterStats.homura.character, spawnPoint, transform.rotation);
			cloneHomura.SendMessage ("SetLifePercent", percent);
			cloneHomura.transform.parent = transform;
			break;
		case 2:
			GameObject cloneKyouko = (GameObject) Instantiate (stats.characterStats.kyouko.character, spawnPoint, transform.rotation);
			cloneKyouko.SendMessage ("SetLifePercent", percent);
			cloneKyouko.transform.parent = transform;
			break;
		case 3:
			GameObject cloneMadoka = (GameObject) Instantiate (stats.characterStats.madoka.character, spawnPoint, transform.rotation);
			cloneMadoka.SendMessage ("SetLifePercent", percent);
			cloneMadoka.transform.parent = transform;
			break;
		case 4:
			GameObject cloneMami = (GameObject) Instantiate (stats.characterStats.mami.character, spawnPoint, transform.rotation);
			cloneMami.SendMessage ("SetLifePercent", percent);
			cloneMami.transform.parent = transform;
			break;
		case 5:
			GameObject cloneSayaka = (GameObject) Instantiate (stats.characterStats.sayaka.character, spawnPoint, transform.rotation);
			cloneSayaka.SendMessage ("SetLifePercent", percent);
			cloneSayaka.transform.parent = transform;
			break;
		}
		GameObject.FindGameObjectWithTag ("BossCamera").SendMessage ("SetPlayer");
	}

	void RemoveBossStage ()
	{
		Destroy (gameObject);
	}
}






