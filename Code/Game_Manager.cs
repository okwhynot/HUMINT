using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using libhumint;

public class Game_Manager : MonoBehaviour {
	public System.DateTime curDate = new System.DateTime(1980,1,1,12,0,0);
	public Menu_Manager MenuManager;
	public Object_Manager ObjectManager;
	public World_Manager WorldManager;
	
	void Start() {
		
	}
	
	void OnGUI() {
		MenuManager.Render();
	}
	
	void Update() {
		Control();
	}
	
	void Tick() {
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
				string[] restricted = {"i","c","escape","return"};
				string key = Input.inputString;
				if(key != "i" && key != "c" && key != "escape" && key != "return") {
					Tick();
				}
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
	
	bool CheckCollision(int nx,int ny,int z) {
		bool cmt = false;
		if(WorldManager.world.map[nx,ny,z].canMoveTo == true) {
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