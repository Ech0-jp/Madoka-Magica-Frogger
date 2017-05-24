using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGUI : MonoBehaviour 
{
	[SerializeField] private GUISkin skin;
	private float originalLifeTimer;
	private float lifeTimer;
	private int level			= 1;
	private int score			= 0;

	private PlayerController playerController;
	[SerializeField] private _GUIClasses.Location levelCenter = new _GUIClasses.Location ();
	[SerializeField] private _GUIClasses.Location scoreCenter = new _GUIClasses.Location ();

	private GameObject soulGem;

	private bool gameStart;
	private bool bossStage;

	void GameStart ()
	{
		gameStart = true;
		playerController = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
	}

	void FixedUpdate ()
	{
		levelCenter.updateLocation ();
		scoreCenter.updateLocation ();
	}

	void SoulGem (Vector3 pos)
	{
		CharacterClasses characterClasses = GameObject.FindGameObjectWithTag ("LevelController").GetComponent<CharacterClasses> ();
		GameObject cloneGem = null;
		switch (playerController.playerCharacter)
		{
		case PlayerController.PlayerCharacter.Akemi_Homura:
			cloneGem = (GameObject) Instantiate (characterClasses.akemiHomura.SoulGem, pos, transform.rotation);
			cloneGem.SendMessage ("MaxLife", characterClasses.akemiHomura.lifeTimer);
			break;
		case PlayerController.PlayerCharacter.Kaname_Madoka:
			cloneGem = (GameObject) Instantiate (characterClasses.kanameMadoka.SoulGem, pos, transform.rotation);
			cloneGem.SendMessage ("MaxLife", characterClasses.kanameMadoka.lifeTimer);
			break;
		case PlayerController.PlayerCharacter.Miki_Sayaka:
			cloneGem = (GameObject) Instantiate (characterClasses.mikiSayaka.SoulGem, pos, transform.rotation);
			cloneGem.SendMessage ("MaxLife", characterClasses.mikiSayaka.lifeTimer);
			break;
		case PlayerController.PlayerCharacter.Sakura_Kyoko:
			cloneGem = (GameObject) Instantiate (characterClasses.sakuraKyoko.SoulGem, pos, transform.rotation);
			cloneGem.SendMessage ("MaxLife", characterClasses.sakuraKyoko.lifeTimer);
			break;
		case PlayerController.PlayerCharacter.Tomoe_Mami:
			cloneGem = (GameObject) Instantiate (characterClasses.tomoMami.SoulGem, pos, transform.rotation);
			cloneGem.SendMessage ("MaxLife", characterClasses.tomoMami.lifeTimer);
			break;
		}
		soulGem = cloneGem;
		soulGem.transform.parent = GameObject.FindGameObjectWithTag ("LevelController").transform;
	}

	void GetLevel(int other)
	{
		level = other;
	}

	void GetScore(int other)
	{
		score = other;
	}

	void EndGame ()
	{
		gameStart = false;
		bossStage = false;
		soulGem.GetComponentInChildren<SpriteRenderer> ().enabled = false;
	}

	void BossStage (bool other)
	{
		bossStage = other;
		soulGem.GetComponentInChildren<SpriteRenderer> ().enabled = !other;
	}

	void OnGUI()
	{
		if (gameStart && !bossStage)
		{
			GUI.skin = skin;
			DrawLevel ();
			DrawScore ();
		}
	}

	void DrawLevel()
	{
		GUI.Label (new Rect (levelCenter.offset.x + levelCenter.position.x, levelCenter.offset.y + levelCenter.position.y, 200, 50), "Level: " + level);
	}

	void DrawScore()
	{
		GUI.Label (new Rect (scoreCenter.offset.x + scoreCenter.position.x, scoreCenter.offset.y + scoreCenter.position.y, 400, 50), "Score: " + score);
	}
}























