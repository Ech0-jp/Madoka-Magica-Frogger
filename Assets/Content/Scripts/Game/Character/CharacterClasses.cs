using UnityEngine;
using System.Collections;

public class CharacterClasses : MonoBehaviour 
{
	/*
	 * Madoka ability: Largest life timer.
	 * Homura ability: Freeze time.
	 * Sayaka ability: Speed.
	 * Mami ability: Kill all witches.
	 * Kyoko ability: Dash.
	 */

	public Akemi_Homura akemiHomura = new Akemi_Homura ();
	public Kaname_Madoka kanameMadoka = new Kaname_Madoka ();
	public Miki_Sayaka mikiSayaka = new Miki_Sayaka ();
	public Sakura_Kyoko sakuraKyoko = new Sakura_Kyoko ();
	public Tomoe_Mami tomoMami = new Tomoe_Mami ();


	[System.Serializable]
	public class Akemi_Homura
	{
		public GameObject AkemiHomura_PFB;
		public GameObject SoulGem;
		public float lifeTimer;
		public float deathPenalty;
		public float speed;
		public float specialCooldownTimer;
		public float specialDurationTimer;
	}

	[System.Serializable]
	public class Kaname_Madoka
	{
		public GameObject KanameMadoka_PFB;
		public GameObject SoulGem;
		public float lifeTimer;
		public float deathPenalty;
		public float speed;
	}

	[System.Serializable]
	public class Miki_Sayaka
	{
		public GameObject MikiSayaka_PFB;
		public GameObject SoulGem;
		public float lifeTimer;
		public float deathPenalty;
		public float speed;
	}

	[System.Serializable]
	public class Sakura_Kyoko
	{
		public GameObject SakuraKyoko_PFB;
		public GameObject SoulGem;
		public float lifeTimer;
		public float deathPenalty;
		public float speed;
		public float specialCooldownTimer;
		public float dashSpeed;
	}

	[System.Serializable]
	public class Tomoe_Mami
	{
		public GameObject TomoeMami_PFB;
		public GameObject SoulGem;
		public float lifeTimer;
		public float deathPenalty;
		public float speed;
		public float specialCooldownTimer;
	}
}
