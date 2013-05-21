using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using libhumint;

[System.Serializable]
public class Interface : ColorLib {
	public enum State {Game,Inventory,Character,Map,Log};
	public State state = State.Game;
	public HUMINT_Game parent;
	public HUMINT_Object.Player player;
	public HUMINT_Object playerGeneral;
	public HUMINT_World worldScript;
	public Console Bg,Fg,Rf;
	public bool isLooking,isPaused;
	
	public Interface(HUMINT_Object.Player p,HUMINT_Object pG) {
		player = p;
		playerGeneral = pG;
		worldScript = Camera.main.GetComponent<HUMINT_World>();
		parent = Camera.main.GetComponent<HUMINT_Game>();
		isPaused = false;
		Bg = new Console("terminal10x16_gs_ro");
		Fg = new Console("terminal8x12_gs_ro");
		Rf = new Console("terminal10x16_gs_ro");
		Initialize();
		Refresh();
	}
	public void Draw() {
		Bg.Render();
		Fg.Render();
		Rf.Render();
	}
	public void Initialize() {
		Bg.Flush();
		//Generic Borders
		for(int y=0;y<15;y++) {
			Bg.Put(0,y,(char)219,LightGray("natural"));
			Bg.Put(61,y,(char)219,LightGray("natural"));
		}
		for(int x=1;x<62;x++)
		{
			Bg.Put(x,0,(char)219,LightGray("natural"));
			Bg.Put(x,14,(char)219,LightGray("natural"));
		}
		switch(state) {
			case State.Game: {
				//Borders
				for(int x=37;x<61;x++)
				{
					Bg.Put(x,0,(char)223,LightGray("natural"));
					//Bg.Put(x,14,(char)220,LightGray("natural"));
				}
				Bg.Put(49,0,(char)223,LightGray("natural"),LightGray("natural"),(char)179);
				for(int y=1;y<14;y++)
				{
					Bg.Put(36,y,(char)219,LightGray("natural"));
					Bg.Put(49,y,(char)179,LightGray("natural"));
				}
				//Text
				Bg.Put(39,1,"Sem Amet");
				Bg.Put(40,2,"health");
				Bg.Put(41,3,"100%",LightGreen("natural")); //THIS WILL BE MOVED TO REFRESH
				Bg.Put(40,4,"weapon");
				Bg.Put(40,5,"psm(h)",LightGray("natural")); //THIS WILL BE MOVED TO REFRESH
				Bg.Put(41,6,"ammo");
				for(int x=38;x<48;x++)
					Bg.Put(x,7,(char)254,Yellow("natural")); //THIS WILL BE MOVED TO REFRESH
				Bg.Put(50,1,"inventory");
				Bg.Put(50,2,"character");
				Bg.Put(50,3,"map");
				Bg.Put(50,4,"look");
				Bg.Put(50,5,"open");
				Bg.Put(50,6,"attack");
				for(int y=1;y<=6;y++)
					Bg.Recolor(50,y,LightBlue("natural"));
				break;
			}
			case State.Inventory: {
				//Borders
				for(int y=1;y<14;y++)
				{
					Bg.Put(2,y,(char)179,LightGray("natural"));
					Bg.Put(37,y,(char)179,LightGray("natural"));
					Bg.Put(45,y,(char)179,LightGray("natural"));
				}
				//Text
				Bg.Put(38,1,"Equip");
				Bg.Put(38,2,"Unequip");
				Bg.Put(38,3,"Drop");
				Bg.Put(38,4,"Reload");
				Bg.Put(38,5,"Combine");
				Bg.Put(38,6,"Look");
				for(int y=1;y<=6;y++)
					Bg.Recolor(38,y,LightBlue("natural"));
				Bg.Put(47,1,"EQUIPPED");
				break;
			}
			case State.Character: {
				break;
			}
			case State.Map: {
				break;
			}
			case State.Log: {
				break;
			}
		}
	}
	public void Refresh() {
		Rf.Flush();
		//CalculateVisibleTiles();
		//CalculateVisibleTiles2();
		//35 wide, 13 tall
		//MIDDLE = (18,7)
		switch(state) {
			case State.Game: {
				//Display Area
				int pX = (int)playerGeneral.Coordinates.x;
				int pY = (int)playerGeneral.Coordinates.y;
				int size = worldScript.world.map.Length;
				int[] mapXV = {pX-17,pX-16,pX-15,pX-14,pX-13,pX-12,pX-11,pX-10,pX-9,pX-8,pX-7,pX-6,pX-5,pX-4,pX-3,pX-2,pX-1,pX,pX+1,pX+2,pX+3,pX+4,pX+5,pX+6,pX+7,pX+8,pX+9,pX+10,pX+11,pX+12,pX+13,pX+14,pX+15,pX+16,pX+17};
				int[] mapYV = {pY-6,pY-5,pY-4,pY-3,pY-2,pY-1,pY,pY+1,pY+2,pY+3,pY+4,pY+5,pY+6};
				for(int x=1;x<=35;x++)
				{
					for(int y=1;y<=13;y++)
					{
						if(mapXV[x-1] < 0 || mapYV[y-1] < 0 || worldScript.world.map[mapXV[x-1],mapYV[y-1],0].tChar == ' ')
							continue;
						else
							Rf.Put(x,y,worldScript.world.map[mapXV[x-1],mapYV[y-1],0].tChar,worldScript.world.map[mapXV[x-1],mapYV[y-1],0].tColor);
					}
				}
				Rf.Put(18,7,"@");
				//Event log example
				EventLog.curDisplayed = "event log fits 57 characters.";
				Rf.Put(1,14,(char)240,Black());
				Rf.Put(2,14,EventLog.curDisplayed,White());
				Rf.Put(59,14,(char)240,Black());
				//switch between ! and * for combat. Figure out a transition.
				break;
			}
			case State.Inventory: {
				int childcount = 0;
				int eqcount = 0;
				char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
				char[] lowercase = alphabet.ToString().ToLower().ToCharArray();
				string[] slotnames = {"Weapon","Hat","Jacket","Shirt","Gloves","Pants","Boots"};
				foreach(Transform child in GameObject.FindWithTag("Player").transform) {
					if(player.equipped.Contains(child.gameObject) == false) //UNEQUIPPED
					{	
						Rf.Put(1,1+childcount,lowercase[childcount]);
						Rf.Put(3,1+childcount,child.name);
						childcount += 1;
					}
				}
				for(int slot = 0;slot<player.equipped.Length;slot++)
				{					
					if(player.equipped[slot] == null)
						Rf.Put(46,2+slot,slotnames[slot]);
					else
						Rf.Put(46,2+slot,player.equipped[slot].name);
				}
				break;
			}
			case State.Character: {
				break;
			}
			case State.Log: {
				break;
			}
		}
	}
	//FOV -- UNFINISHED, IMPLEMENTATION #1
	public void CalculateVisibleTiles() {
		int pX = (int)playerGeneral.Coordinates.x;
		int pY = (int)playerGeneral.Coordinates.y;
		List<Line.Point> lrLOS = new List<Line.Point>();
		List<Line.Point> udLOS = new List<Line.Point>();
		List<Line.Point> lrBlockers = new List<Line.Point>();
		List<Line.Point> udBlockers = new List<Line.Point>();
		//Left and Right LOS Cones
		for(int i=-6;i<=6;i++) {
			bool fBL = false;
			bool fBL2 = false;
			Line L = new Line(pX,pY,pX-17,pY+i);
			Line L2 = new Line(pX,pY,pX+17,pY+i);
			foreach(Line.Point p in L.getPoints()) {
				if(p.x >= 0 && p.y >= 0) {
					//if(fBL == false && worldScript.world.z1[p.x,p.y].canSeeThrough == false) {
					//	lrBlockers.Add(p);
					//	fBL = true;
					//}
					worldScript.world.map[p.x,p.y,0].tColor = Blue("natural");
					lrLOS.Add(p);
				}
			}
			foreach(Line.Point p in L2.getPoints()) {
				if(p.x < worldScript.world.w && p.y >= 0) {
					//if(fBL2 == false && worldScript.world.z1[p.x,p.y].canSeeThrough == false) {
					//	lrBlockers.Add(p);
					//	fBL2 = true;
					//}
					worldScript.world.map[p.x,p.y,0].tColor = Green("natural");
					lrLOS.Add(p);
				}
			}
		}
		//Up and Down LOS Cones - ! and ¡ denote overlap.
		for(int i=-17;i<=17;i++) {
			bool fBL = false;
			bool fBL2 = false;
			Line L = new Line(pX,pY,pX+i,pY-6);
			Line L2 = new Line(pX,pY,pX+i,pY+6);
			foreach(Line.Point p in L.getPoints()) {
				if(p.x >= 0 && p.y >= 0) {
					if(p.x != pX-1 && p.x != pX+1 || p.y != pY) {
						if(worldScript.world.map[p.x,p.y,0].tColor == Blue("natural") || worldScript.world.map[p.x,p.y,0].tColor == Green("natural")) {
							worldScript.world.map[p.x,p.y,0].tChar = '!';	
						}
						else {
							//if(fBL == false && worldScript.world.z1[p.x,p.y].canSeeThrough == false) {
							//	udBlockers.Add(p);
							//	fBL = true;
							//}
							worldScript.world.map[p.x,p.y,0].tColor = Red("natural");
							udLOS.Add(p);
						}
					}
				}
			}
			foreach(Line.Point p in L2.getPoints()) {
				if(p.x >= 0 && p.y >= 0) {
					if(p.x != pX-1 && p.x != pX+1 || p.y != pY) {
						if(worldScript.world.map[p.x,p.y,0].tColor == Blue("natural") || worldScript.world.map[p.x,p.y,0].tColor == Green("natural")) {
							worldScript.world.map[p.x,p.y,0].tChar = (char)173;
						}
						else {
							//if(fBL2 == false && worldScript.world.z1[p.x,p.y].canSeeThrough == false) {
							//	udBlockers.Add(p);
							//	fBL2 = true;
							//}
							worldScript.world.map[p.x,p.y,0].tColor = Yellow("natural");
							udLOS.Add(p);
						}
					}
				}
			}
		}
		foreach(Line.Point p in udBlockers) {
			worldScript.world.map[p.x,p.y,0].tColor = worldScript.world.map[p.x,p.y,0].tCDefault;
		}
		foreach(Line.Point p in lrBlockers) {
			worldScript.world.map[p.x,p.y,0].tColor = worldScript.world.map[p.x,p.y,0].tCDefault;
		}
		//Restore color - up and down
		foreach(Line.Point p in udLOS) {
			//worldScript.world.z1[p.x,p.y].tColor = worldScript.world.z1[p.x,p.y].tCDefault;
		}
	}
	//FOV -- UNFINISHED, IMPLEMENTATION #2
	public void CalculateVisibleTiles2() {
		int px = (int)playerGeneral.Coordinates.x;
		int py = (int)playerGeneral.Coordinates.y;
		int[] block = new int[4];
		Line left = new Line(px-1,py,px-17,py);
		Line right = new Line(px+1,py,px+17,py);
		Line up = new Line(px,py-1,px,py-6);
		Line down = new Line(px,py+1,px,py+6);
		foreach(Line.Point p in left.getPoints()) {
			if(p.x >= 0 && p.y >= 0 && block[0] == 0)
			{
				worldScript.world.map[p.x,p.y,0].tColor = Red("natural");
				if(worldScript.world.map[p.x,p.y,0].canSeeThrough == false)
					block[0] = left.getPoints().IndexOf(p);
			}
		}
		foreach(Line.Point p in right.getPoints()) {
			if(p.x >= 0 && p.y >= 0 && block[1] == 0) {
				worldScript.world.map[p.x,p.y,0].tColor = Red("natural");
				if(worldScript.world.map[p.x,p.y,0].canSeeThrough == false)
					block[1] = right.getPoints().IndexOf(p);
			}
		}
		foreach(Line.Point p in up.getPoints()) {
			if(p.x >= 0 && p.y >= 0 && block[2] == 0) {
				worldScript.world.map[p.x,p.y,0].tColor = Red("natural");
				if(worldScript.world.map[p.x,p.y,0].canSeeThrough == false)
					block[2] = up.getPoints().IndexOf(p);
			}
		}
		foreach(Line.Point p in down.getPoints()) {
			if(p.x >= 0 && p.y >= 0 && block[3] == 0) {
				worldScript.world.map[p.x,p.y,0].tColor = Red("natural");
				if(worldScript.world.map[p.x,p.y,0].canSeeThrough == false)
					block[3] = down.getPoints().IndexOf(p);
			}
		}
	}
	//FOV -- UNFINISHED, IMPLEMENTATION #3
	public Color HPColor(int hp) {
		Color32 hpc = LightGreen("natural");
		if(hp > 75)
			hpc = LightGreen("natural");
		else if(hp < 75 && hp > 25)
			hpc = Yellow("natural");
		else if(hp < 25)
			hpc = Red("natural");
		return hpc;
	}
	public int BoolToInt(bool boolean) {
		int i = new int();
		if(boolean == false)
			i = 0;
		else
			i = 1;
		return i;
	}
}

public class Cursor : ColorLib {
	char cursor = 'x';
	Color cursorColor = White();
	public Vector2 cursorCoordinates;
	public Line lineFromPlayer;
	public Cursor(char icon = 'x',Color col = new Color()) {
		cursorCoordinates = new Vector2(18,7);
	}
	public void Animate() {
		
	}
}

public static class EventLog {
	public static List<string> log = new List<string>();
	public static string curDisplayed;
}