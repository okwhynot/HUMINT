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
	public int turnsPassed = 0;
	
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
		if(CheckActionStates(5))
			MenuManager.DisplayCursor(WorldManager);
	}
	
	void Update() {
		Control();
		if(Input.GetKeyDown("4"))
			Tick();
		if(Input.GetKeyDown("=")) {
			GameObject pMag = GameObject.Instantiate(GameObject.Find("9x18mm Makarov Magazine (G)")) as GameObject;
			GameObject pGun = GameObject.Instantiate(GameObject.Find("Makarov PM (G)")) as GameObject;
			pMag.name = pMag.GetComponent<Object>().name;
			pMag.tag = "Item";
			pMag.GetComponent<Object>().item.capacity = 10;
			pMag.GetComponent<Object>().Coordinates = Player.GetComponent<Object>().Coordinates;
			pMag.transform.parent = Player.transform;
			pGun.name = pGun.GetComponent<Object>().name;
			pGun.tag = "Item";
			pGun.GetComponent<Object>().Coordinates = Player.GetComponent<Object>().Coordinates;
			pGun.transform.parent = Player.transform;
			Player.GetComponent<Object>().player.EquippedWeapon = pGun;
			Tick();
		}	
	}
	
	void UpdateInventory() {
		List<GameObject> inventory = new List<GameObject>();
		foreach(Transform child in Player.transform) {
			inventory.Add(child.gameObject);
		}
		Player.GetComponent<Object>().player.inventory = inventory.ToArray();
		foreach(GameObject NPC in GameObject.FindGameObjectsWithTag("NPC")) {
			inventory = new List<GameObject>();
			foreach(Transform child in NPC.transform)
				inventory.Add(child.gameObject);
			NPC.GetComponent<Object>().npc.inventory = inventory.ToArray();
		}
	}
	
	void UpdateAI() {
		foreach(GameObject gobj in GameObject.FindGameObjectsWithTag("NPC")) {
			AI ai = gobj.GetComponent<AI>();
			ai.CheckPlayerDist();
		}
	}
	
	void Tick() {
		turnsPassed++;
		UpdateAI();
		UpdateInventory();
		FOV();
		WorldManager.world.UpdateContents();
		curDate = Clock(curDate, 0.1667);
		MenuManager.Refresh();
	}
	
	public void FOV() {
		int px = (int)Player.GetComponent<Object>().Coordinates.x;
		int py = (int)Player.GetComponent<Object>().Coordinates.y;
		int pz = (int)Player.GetComponent<Object>().Coordinates.z;
		#region Fog
		for(int x = 0; x < WorldManager.world.map.GetLength(0); x++) {
			for(int y = 0; y < WorldManager.world.map.GetLength(1); y++) {
				WorldManager.world.map[x,y,pz].tColor = ColorLib.DarkGray("cga");
			}
		}
		#endregion
		#region FOV
		MenuManager.visible = new List<Line.Point>();
		for(int x = 1;x < 19;x++) {
			int tx = x - 10;
			tx += px;
			int ty = py - 7;
			Line l = new Line(px,py,tx,ty);
			foreach(Line.Point p in l.getPoints()) {
				WorldManager.world.map[p.x,p.y,p.z].hasBeenSeen = true;
				MenuManager.visible.Add(p);
				if(WorldManager.world.map[p.x,p.y,p.z].tileContents.Count == 0)
					WorldManager.world.map[p.x,p.y,p.z].tColor = WorldManager.world.map[p.x,p.y,p.z].tCDefault;
				if(WorldManager.world.map[p.x,p.y,p.z].npcPresent == true)
					WorldManager.world.map[p.x,p.y,p.z].tColor = WorldManager.world.map[p.x,p.y,p.z].tileContents[0].GetComponent<Object>().objectColor;
				if(WorldManager.world.map[p.x,p.y,p.z].canSeeThrough == false) {
					break;
				}
			}
			ty = py + 7;
			l = new Line(px,py,tx,ty);
			foreach(Line.Point p in l.getPoints()) {
				WorldManager.world.map[p.x,p.y,p.z].hasBeenSeen = true;
				WorldManager.world.map[p.x,p.y,p.z].tColor = WorldManager.world.map[p.x,p.y,p.z].tCDefault;
				MenuManager.visible.Add(p);
				if(WorldManager.world.map[p.x,p.y,p.z].canSeeThrough == false) {
					break;
				}
			}
		}
		for(int y = 1; y < 13; y++) {
			int tx = px - 10;
			int ty = y - 7;
			
			ty += py;
			
			Line l = new Line(px,py,tx,ty);
			foreach(Line.Point p in l.getPoints()) {
				WorldManager.world.map[p.x,p.y,p.z].hasBeenSeen = true;
				WorldManager.world.map[p.x,p.y,p.z].tColor = WorldManager.world.map[p.x,p.y,p.z].tCDefault;
				MenuManager.visible.Add(p);
				if(WorldManager.world.map[p.x,p.y,p.z].canSeeThrough == false) {
					break;
				}
			}
			
			tx = px + 10;
			l = new Line(px,py,tx,ty);
			foreach(Line.Point p in l.getPoints()) {
				WorldManager.world.map[p.x,p.y,p.z].hasBeenSeen = true;
				WorldManager.world.map[p.x,p.y,p.z].tColor = WorldManager.world.map[p.x,p.y,p.z].tCDefault;
				MenuManager.visible.Add(p);
				if(WorldManager.world.map[p.x,p.y,p.z].canSeeThrough == false) {
					break;
				}
			}
		}
		#endregion
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
			else if(Input.GetKeyDown("p")) {
				if(WorldManager.world.map[px,py,pz].tileContents.Count > 1) {
					FlipActionState(6);
				}
				else if(WorldManager.world.map[px,py,pz].tileContents.Count == 1) {
					ActionManager.PickUpObject(px,py,WorldManager.world.map[px,py,pz].tileContents[0]);
					Debug.Log(WorldManager.world.map[px,py,pz].tileContents[0].name);
				}
				else if(WorldManager.world.map[px,py,pz].tileContents.Count == 0)
					Debug.Log("Nothing to pickup.");
			}
			else if(Input.GetKeyDown("r")) {
				ActionManager.Reload(Player.GetComponent<Object>().player.inventory, Player.GetComponent<Object>().player.EquippedWeapon);
			}
			#region Move
			if(CheckActionStates(0)) {//MOVE
				if(Input.GetKeyDown("up")) {
					if(CheckCollision(px,py-1,pz) && py > 0)
						player.Coordinates.y -= 1;
					else if(WorldManager.world.map[px,py-1,pz].isDoor && WorldManager.world.map[px,py-1,pz].isOpen == false)
						WorldManager.world.map[px,py-1,pz].Open();
					else if(WorldManager.world.map[px,py-1,pz].npcPresent && player.player.EquippedWeapon == null)
						ActionManager.Punch(WorldManager.world.map[px,py-1,pz].tileContents[0]);
					else if(WorldManager.world.map[px,py-1,pz].npcPresent && player.player.EquippedWeapon != null)
						ActionManager.Attack(WorldManager.world.map[px,py-1,pz].tileContents[0],player.player.EquippedWeapon);
				}
				else if(Input.GetKeyDown("down")) {
					if(CheckCollision(px,py+1,pz))
						player.Coordinates.y += 1;
					else if(WorldManager.world.map[px,py+1,pz].isDoor && WorldManager.world.map[px,py+1,pz].isOpen == false)
						WorldManager.world.map[px,py+1,pz].Open();
					else if(WorldManager.world.map[px,py+1,pz].npcPresent && player.player.EquippedWeapon == null)
						ActionManager.Punch(WorldManager.world.map[px,py+1,pz].tileContents[0]);
					else if(WorldManager.world.map[px,py+1,pz].npcPresent && player.player.EquippedWeapon != null)
						ActionManager.Attack(WorldManager.world.map[px,py+1,pz].tileContents[0],player.player.EquippedWeapon);
				}
				else if(Input.GetKeyDown("left")) {
					if(CheckCollision(px-1,py,pz) && px > 0)
						player.Coordinates.x -= 1;
					else if(WorldManager.world.map[px-1,py,pz].isDoor && WorldManager.world.map[px-1,py,pz].isOpen == false)
						WorldManager.world.map[px-1,py,pz].Open();
					else if(WorldManager.world.map[px-1,py,pz].npcPresent && player.player.EquippedWeapon == null)
						ActionManager.Punch(WorldManager.world.map[px-1,py,pz].tileContents[0]);
					else if(WorldManager.world.map[px-1,py,pz].npcPresent && player.player.EquippedWeapon != null)
						ActionManager.Attack(WorldManager.world.map[px-1,py,pz].tileContents[0],player.player.EquippedWeapon);
				}
				else if(Input.GetKeyDown("right")) {
					if(CheckCollision(px+1,py,pz))
						player.Coordinates.x += 1;
					else if(WorldManager.world.map[px+1,py,pz].isDoor && WorldManager.world.map[px+1,py,pz].isOpen == false)
						WorldManager.world.map[px+1,py,pz].Open();
					else if(WorldManager.world.map[px+1,py,pz].npcPresent && player.player.EquippedWeapon == null)
						ActionManager.Punch(WorldManager.world.map[px+1,py,pz].tileContents[0]);
					else if(WorldManager.world.map[px+1,py,pz].npcPresent && player.player.EquippedWeapon != null)
						ActionManager.Attack(WorldManager.world.map[px+1,py,pz].tileContents[0],player.player.EquippedWeapon);
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
			#endregion
			#region Attack
			else if(CheckActionStates(1)) {//ATTACK -- UNFINISHED
				if(Input.GetKeyDown("up") && MenuManager.cursorLoc.y > 1)
					MenuManager.cursorLoc.y -= 1;
				else if(Input.GetKeyDown("down") && MenuManager.cursorLoc.y < 13)
					MenuManager.cursorLoc.y += 1;
				else if(Input.GetKeyDown("left") && MenuManager.cursorLoc.x > 1)
					MenuManager.cursorLoc.x -= 1;
				else if(Input.GetKeyDown("right") && MenuManager.cursorLoc.x < 19)
					MenuManager.cursorLoc.x += 1;
				else if(Input.GetKeyDown("return")) {
					int targetX = (int)MenuManager.cursorLoc.x - 10;
					int targetY = (int)MenuManager.cursorLoc.y - 7;
					targetX += (int)player.Coordinates.x;
					targetY += (int)player.Coordinates.y;
					ActionManager.Fire((int)player.Coordinates.x, (int)player.Coordinates.y,targetX,targetY,player.player.EquippedWeapon);
					FlipActionState(1);
					Tick();
				}
			}
			#endregion
			#region Close
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
			#endregion
			#region Open
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
			#endregion
			else if(CheckActionStates(4)) {//EXAMINE -- UNFINISHED
				
			}
			#region Look
			else if(CheckActionStates(5)) {//LOOK -- UNFINISHED
				if(Input.GetKeyDown("up") && MenuManager.cursorLoc.y > 1)
					MenuManager.cursorLoc.y -= 1;
				else if(Input.GetKeyDown("down") && MenuManager.cursorLoc.y < 13)
					MenuManager.cursorLoc.y += 1;
				else if(Input.GetKeyDown("left") && MenuManager.cursorLoc.x > 1)
					MenuManager.cursorLoc.x -= 1;
				else if(Input.GetKeyDown("right") && MenuManager.cursorLoc.x < 19)
					MenuManager.cursorLoc.x += 1;
			}
			#endregion
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