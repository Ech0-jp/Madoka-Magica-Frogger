using UnityEngine;
using System.Collections;

public class Musicplayer : MonoBehaviour 
{
	private AudioSource audio;
	[SerializeField] private AudioClip[] introMusic;
	[SerializeField] private AudioClip[] music;
	private int currentSong;
	private int nextSong;

	// Use this for initialization
	void Start () 
	{
		audio = GetComponent<AudioSource> ();
		currentSong = 0;
		nextSong = 0;
		audio.clip = introMusic[0];
		audio.Play ();
		StartCoroutine ("NextSong_Intro", audio.clip.length);
		Debug.Log (music.Length);
	}

	IEnumerator NextSong_Intro (float time)
	{
		yield return new WaitForSeconds (time);

		currentSong = 1;
		nextSong = 1;
		audio.clip = introMusic[1];
		audio.Play ();
		StartCoroutine ("NextSong", audio.clip.length);
	}

	IEnumerator NextSong (float time)
	{
		yield return new WaitForSeconds (time);


		while (nextSong == currentSong)
		{
			if (currentSong == 1)
			{
				while (nextSong == currentSong || nextSong == 0)
				{
					nextSong = Random.Range (0, music.Length);
				}
			}
			else
				nextSong = Random.Range (0, music.Length);
		}
		if (nextSong == 1)
			nextSong = 0;
		if (currentSong == 0)
			nextSong = 1;

		currentSong = nextSong;
		audio.clip = music[nextSong];
		audio.Play ();
		StartCoroutine ("NextSong", audio.clip.length);
	}
}
