using UnityEngine;
using System;
using System.Collections;
using libhumint;

public class HUMINT_Game : MonoBehaviour {
	public enum UI_States {Game,Inventory,Character,Map};
	public UI_States uistate = UI_States.Game;
	Overlay over;
	public GameArea game;
	GameObject player;
	
	void Awake() {
		if(GameObject.FindWithTag("Player") == null)
			player = (GameObject)Instantiate(Resources.Load("Player_DEBUG"));
		else
			player = GameObject.FindWithTag("Player");
	}
	
	void Start() {
		GameObject.Find("Main Camera").GetComponent<HUMINT_World>().worldMap = GameObject.Find("Main Camera").GetComponent<HUMINT_World>().wMap();
		over = new Overlay();
		game = new GameArea();
	}
	
	void Update() {
		player.GetComponent<HUMINT_Object>().player.Movement();
		if(Input.GetKeyDown("i"))
			uistate = UI_States.Inventory;
	}
	
	void OnGUI() {
		switch(uistate)
		{
			case UI_States.Game:
			{
				over.Draw();
				game.Draw();
				break;
			}
			case UI_States.Inventory:
			{
				break;
			}
			case UI_States.Character:
			{
				break;
			}
			case UI_States.Map:
			{
				break;
			}
		}
	}
	//Actions should ONLY be performed during a pulse.
	public void Pulse() {
		game.Refresh();
	}
}