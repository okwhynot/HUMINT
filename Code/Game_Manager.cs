using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using libhumint;

public class Game_Manager : MonoBehaviour {
	public System.DateTime curDate = new System.DateTime(1980,1,1,12,0,0);
	public Audio_Manager AudioManager;
	public Menu_Manager MenuManager;
	public Object_Manager ObjectManager;
	public World_Manager WorldManager;
	public Action_Manager ActionManager;
	public GameObject Player;
	
	void Awake() {
		foreach(GameObject npc in GameObject.FindGameObjectsWithTag("NPC")) {
			DontDestroyOnLoad(npc);
		}
	}
	
	void Start() {
		//this.gameObject.GetComponent<AudioSource>().Play();
	}
	
	void OnGUI() {
		MenuManager.Render();
		if(CheckActionStates(1))
			MenuManager.DisplayLine(WorldManager);
	}
	
	void Update() {
		Control();
		if(Input.GetKeyDown("4"))
			Tick();
		if(Input.GetKeyDown("=")) {
			GameObject.FindWithTag("NPC").GetComponent<AI>().Pathfind();
			Tick();
		}
	}
	
	void Tick() {
		GameObject.FindWithTag("NPC").GetComponent<AI>().FollowPath();
		WorldManager.world.UpdateContents();
		curDate = Clock(curDate, 0.1667);
		MenuManager.Refresh();
	}
	
	void Control() {
		MenuManager.Control();
		Object player = GameObject.Find("Player").GetComponent<Object>();
		int px = (int)player.GetComponent<Object>().Coordinates.x;
		int py = (int)player.GetComponent<Object>().Coordinates.y;
		int pz = (int)player.GetComponent<Object>().Coordinates.z;
		if(CheckStates("Game","UNPAUSED")) {
			if(Input.GetKeyDown("a"))
				FlipActionState(1);
			else if(Input.GetKeyDown("u"))
				FlipActionState(2);
			else if(Input.GetKeyDown("o"))
				FlipActionState(3);
			else if(Input.GetKeyDown("e"))
				FlipActionState(4);
			else if(Input.GetKeyDown("l"))
				FlipActionState(5);
			
			if(CheckActionStates(0)) {//MOVE
				if(Input.GetKeyDown("up")) {
					if(CheckCollision(px,py-1,pz) && py > 0)
							player.Coordinates.y -= 1;
					else if(WorldManager.world.map[px,py-1,pz].isDoor && WorldManager.world.map[px,py-1,pz].isOpen == false)
							WorldManager.world.map[px,py-1,pz].Open();
				}
				else if(Input.GetKeyDown("down")) {
					if(CheckCollision(px,py+1,pz))
						player.Coordinates.y += 1;
					else if(WorldManager.world.map[px,py+1,pz].isDoor && WorldManager.world.map[px,py+1,pz].isOpen == false)
							WorldManager.world.map[px,py+1,pz].Open();
				}
				else if(Input.GetKeyDown("left")) {
					if(CheckCollision(px-1,py,pz) && px > 0)
						player.Coordinates.x -= 1;
					else if(WorldManager.world.map[px-1,py,pz].isDoor && WorldManager.world.map[px-1,py,pz].isOpen == false)
						WorldManager.world.map[px-1,py,pz].Open();
				}
				else if(Input.GetKeyDown("right")) {
					if(CheckCollision(px+1,py,pz))
						player.Coordinates.x += 1;
					else if(WorldManager.world.map[px+1,py,pz].isDoor && WorldManager.world.map[px+1,py,pz].isOpen == false)
						WorldManager.world.map[px+1,py,pz].Open();
				}
			
				if(Input.GetKeyDown("escape")) {
					MenuManager.gameState = Menu_Manager.GAME.PAUSED;
					MenuManager.LoadBackground("Menu Backgrounds/G_Paused");
					MenuManager.ColorBackground();
				}
			
				if(Input.anyKeyDown) {
					string key = null;
					string[] allowed = {"up","down","left","right"};
					foreach(string s in allowed) {
						if(Input.GetKeyDown(s))
							key = s;
					}
					if(allowed.Contains(key))
						Tick();
				}
			}
			else if(CheckActionStates(1)) {//ATTACK -- UNFINISHED
				if(Input.GetKeyDown("up") && MenuManager.cursorLoc.y > 1)
					MenuManager.cursorLoc.y -= 1;
				else if(Input.GetKeyDown("down") && MenuManager.cursorLoc.y < 13)
					MenuManager.cursorLoc.y += 1;
				else if(Input.GetKeyDown("left") && MenuManager.cursorLoc.x > 1)
					MenuManager.cursorLoc.x -= 1;
				else if(Input.GetKeyDown("right") && MenuManager.cursorLoc.x < 19)
					MenuManager.cursorLoc.x += 1;
			}
			else if(CheckActionStates(2)) {//CLOSE
				if(Input.GetKeyDown("up") && WorldManager.world.map[px,py-1,pz].isDoor && WorldManager.world.map[px,py-1,pz].isOpen)
					WorldManager.world.map[px,py-1,pz].Close();
				else if(Input.GetKeyDown("down") && WorldManager.world.map[px,py+1,pz].isDoor && WorldManager.world.map[px,py+1,pz].isOpen)
					WorldManager.world.map[px,py+1,pz].Close();
				else if(Input.GetKeyDown("left") && WorldManager.world.map[px-1,py,pz].isDoor && WorldManager.world.map[px-1,py,pz].isOpen)
					WorldManager.world.map[px-1,py,pz].Close();
				else if(Input.GetKeyDown("right") && WorldManager.world.map[px+1,py,pz].isDoor && WorldManager.world.map[px+1,py,pz].isOpen)
					WorldManager.world.map[px+1,py,pz].Close();
			}
			else if(CheckActionStates(3)) {//OPEN
				if(Input.GetKeyDown("up") && WorldManager.world.map[px,py-1,pz].isDoor && WorldManager.world.map[px,py-1,pz].isOpen == false)
					WorldManager.world.map[px,py-1,pz].Open();
				else if(Input.GetKeyDown("down") && WorldManager.world.map[px,py+1,pz].isDoor && WorldManager.world.map[px,py+1,pz].isOpen == false)
					WorldManager.world.map[px,py+1,pz].Open();
				else if(Input.GetKeyDown("left") && WorldManager.world.map[px-1,py,pz].isDoor && WorldManager.world.map[px-1,py,pz].isOpen == false)
					WorldManager.world.map[px-1,py,pz].Open();
				else if(Input.GetKeyDown("right") && WorldManager.world.map[px+1,py,pz].isDoor && WorldManager.world.map[px+1,py,pz].isOpen == false)
					WorldManager.world.map[px+1,py,pz].Open();
			}
			else if(CheckActionStates(4)) {//EXAMINE -- UNFINISHED
				
			}
			else if(CheckActionStates(5)) {//LOOK -- UNFINISHED
				
			}
			if(Input.anyKeyDown) {
				MenuManager.Refresh();
			}
		}
		else if(CheckStates("Game","PAUSED")) {
			if(Input.GetKeyDown("up") && MenuManager.selected > 0)
				MenuManager.selected -= 1;
			else if(Input.GetKeyDown("down") && MenuManager.selected < 4)
				MenuManager.selected += 1;
			
			if(Input.GetKeyDown("return")) {
				if(MenuManager.selected == 0) {
					MenuManager.ColorBackground();
					MenuManager.gameState = Menu_Manager.GAME.UNPAUSED;
				}
			}
			
			if(Input.anyKeyDown) {
				MenuManager.Refresh();
			}
		}
	}
	
	void FlipActionState(int state) {
		int cur = (int)ActionManager.state;
		if(cur == state)
			cur = 0;
		else if(cur == 0)
			cur = state;
		
		ActionManager.state = (Action_Manager.ACTIONSTATE)cur;
	}
	
	System.DateTime Clock(System.DateTime toEdit, double t) {
		toEdit = toEdit.AddMinutes(t);
		return toEdit;
	}
	
	bool CheckStates(string a, string b) {
		Menu_Manager menu = this.gameObject.GetComponent<Menu_Manager>();
		bool cs = false;
		if(System.Enum.GetName(typeof(Menu_Manager.STATE),(int)menu.state) == a && System.Enum.GetName(typeof(Menu_Manager.GAME),(int)menu.gameState) == b) {
			cs = true;
		}
		return cs;
	}
	
	bool CheckActionStates(int index) {
		bool thisIsCurState = false;
		string state = System.Enum.GetName(typeof(Action_Manager.ACTIONSTATE),index);
		if(System.Enum.GetName(typeof(Action_Manager.ACTIONSTATE),(int)ActionManager.state) == state)
			thisIsCurState = true;
		
		return thisIsCurState;
	}
	
	bool CheckCollision(int nx,int ny,int z) {
		bool cmt = false;
		if(WorldManager.world.map[nx,ny,z].canMoveTo == true && WorldManager.world.map[nx,ny,z].npcPresent == false) {
			if(WorldManager.world.map[nx,ny,z].isDoor == true) {
				if(WorldManager.world.map[nx,ny,z].isOpen == false)
					cmt = false;
				else
					cmt = true;
			}
			else if(WorldManager.world.map[nx,ny,z].isDoor == false)
				cmt = true;
		}
		else
			cmt = false;
		return cmt;
	}
}