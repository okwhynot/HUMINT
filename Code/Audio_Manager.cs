using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using libhumint;

public class Audio_Manager : MonoBehaviour {
	Game_Manager GameManager;
	AudioSource MusicPlayer;
	AudioSource MenuAudio;
	public AudioClip mainMenuMusic;
	public AudioClip menuClick;
	
	void Awake() {
		GameManager = this.gameObject.GetComponent<Game_Manager>();
		MusicPlayer = this.gameObject.GetComponents<AudioSource>()[0];
		MenuAudio = this.gameObject.GetComponents<AudioSource>()[1];
		MusicPlayer.clip = mainMenuMusic;
		MenuAudio.clip = menuClick;
	}
	
	void Start() {
		MusicPlayer.Play();
	}
	
	public void PlayMenuSound() {
		MenuAudio.Play();
	}
	
	void SetMusic() {//Watches the current state of the game, plays music accordingly.
		switch(GameManager.MenuManager.state) {
			case Menu_Manager.STATE.Menu: {
				MusicPlayer.clip = mainMenuMusic;
				break;
			}
			case Menu_Manager.STATE.Game: {
				
				break;
			}
		}
	}
	
}