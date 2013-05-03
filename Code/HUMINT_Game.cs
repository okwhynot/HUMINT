using UnityEngine;
using System;
using System.Collections;
using libhumint;

public class HUMINT_Game : MonoBehaviour {
	public bool gameIsPaused;
	public Interface iface;
	GameObject player;
	
	void Awake() {
		Screen.SetResolution(620,240,false);
		if(GameObject.FindWithTag("Player") == null)
			player = (GameObject)Instantiate(Resources.Load("Player_DEBUG"));
		else
			player = GameObject.FindWithTag("Player");
	}
	
	void Start() {
		iface = new Interface(player.GetComponent<HUMINT_Object>().player,player.GetComponent<HUMINT_Object>());
	}
	
	void Update() {
		//Only do player movement when unpaused & out of menus
		switch(iface.state) {
			case Interface.State.Game: {
				GUIHotkeys();
				break;
			}
			case Interface.State.Inventory: {
				break;
			}
			case Interface.State.Character: {
				break;
			}
		}
		if(gameIsPaused == false)
			player.GetComponent<HUMINT_Object>().player.Movement();
		GUIHotkeys();
	}
	
	void OnGUI() {
		iface.Draw();
	}
	//Actions should ONLY be performed during a pulse.
	public void Pulse() {
		iface.Refresh();
	}
	
	void GUIHotkeys() {
		if(Input.GetKeyDown("space"))
			gameIsPaused = !gameIsPaused;
		else if(Input.GetKeyDown("i"))
		{
			//iface.state = Interface.State.Inventory;
			//iface.Initialize();
		}
		else if(Input.GetKeyDown("c"))
		{
			//iface.state = Interface.State.Character;
			//iface.Initialize();
		}
		else if(Input.GetKeyDown("m"))
		{
			//iface.state = Interface.State.Map;
			//iface.Initialize();
		}
		if(Input.anyKeyDown)
			Pulse();
	}
}