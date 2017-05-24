using UnityEngine;
using System.Collections;

public class CharacterStats : MonoBehaviour 
{
	public Stats characterStats = new Stats ();
	public float lifePercent;

	[System.Serializable]
	public class Stats
	{
		public Homura homura = new Homura ();
		public Kyouko kyouko = new Kyouko ();
		public Madoka madoka = new Madoka ();
		public Mami mami = new Mami ();
		public Sayaka sayaka = new Sayaka ();
		
		[System.Serializable]
		public class Homura
		{
			public GameObject character;
			public GameObject soulGem;
			public float lifeTimer;
			public float lifeBonus;
			public float health;
			public float deathPenalty;
		}
		
		[System.Serializable]
		public class Kyouko
		{
			public GameObject character;
			public GameObject soulGem;
			public float lifeTimer;
			public float lifeBonus;
			public float health;
			public float deathPenalty;
		}
		
		[System.Serializable]
		public class Madoka
		{
			public GameObject character;
			public GameObject soulGem;
			public float lifeTimer;
			public float lifeBonus;
			public float health;
			public float deathPenalty;
		}
		
		[System.Serializable]
		public class Mami
		{
			public GameObject character;
			public GameObject soulGem;
			public float lifeTimer;
			public float lifeBonus;
			public float health;
			public float deathPenalty;
		}
		
		[System.Serializable]
		public class Sayaka
		{
			public GameObject character;
			public GameObject soulGem;
			public float lifeTimer;
			public float lifeBonus;
			public float health;
			public float deathPenalty;
		}
	}
}
























