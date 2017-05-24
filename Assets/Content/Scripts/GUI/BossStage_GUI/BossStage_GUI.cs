using UnityEngine;
using System.Collections;

public class BossStage_GUI : MonoBehaviour 
{
	void Character (int other)
	{
		CharacterStats stats = GameObject.FindGameObjectWithTag ("BossStage").GetComponent<CharacterStats> ();
		switch (other)
		{
		case 1:
			GameObject cloneGem1 = (GameObject) Instantiate (stats.characterStats.homura.soulGem, transform.position, transform.rotation);
			cloneGem1.transform.parent = transform;
			cloneGem1.name = stats.characterStats.homura.soulGem.name;
			break;
		case 2:
			GameObject cloneGem2 = (GameObject) Instantiate (stats.characterStats.kyouko.soulGem, transform.position, transform.rotation);
			cloneGem2.transform.parent = transform;
			cloneGem2.name = stats.characterStats.kyouko.soulGem.name;
			break;
		case 3:
			GameObject cloneGem3 = (GameObject) Instantiate (stats.characterStats.madoka.soulGem, transform.position, transform.rotation);
			cloneGem3.transform.parent = transform;
			cloneGem3.name = stats.characterStats.madoka.soulGem.name;
			break;
		case 4:
			GameObject cloneGem4 = (GameObject) Instantiate (stats.characterStats.mami.soulGem, transform.position, transform.rotation);
			cloneGem4.transform.parent = transform;
			cloneGem4.name = stats.characterStats.mami.soulGem.name;
			break;
		case 5:
			GameObject cloneGem5 = (GameObject) Instantiate (stats.characterStats.sayaka.soulGem, transform.position, transform.rotation);
			cloneGem5.transform.parent = transform;
			cloneGem5.name = stats.characterStats.sayaka.soulGem.name;
			break;
		}
	}
}
