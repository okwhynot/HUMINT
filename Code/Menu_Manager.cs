using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using libhumint;

//All menu variables should be stored in here. This manages BOTH in game menus and the main menu.
public class Menu_Manager : MonoBehaviour {
	//static System.DateTime curDate = new System.DateTime(1980,1,1);
	List<string> loadedFields = new List<string>(); //internal storage of loaded strings
	public static string[] Titles = {"title","subtitle"}; //Game title & abbreviation
	public static string[] MMItemTitles = {"ng_button","lg_button","am_button","aa_button","go_button","ab_button","re_button"}; //Main Menu Buttons
	public static string[] CCTitles = {"bz_title","sn_title","fn_title","sp_title","pk_title","gd_title"}; //Character creator categories
	public static string[] CCTitles2 = {"sc_title","hc_title","ec_title"};
	public static string[] GPButtons = {"resume","save","settings","abandon","quit"};
	public static string[] Neighborhoods = {"Friedrichshain","K"+(char)148+"penick","Lichtenberg","Marzahn","Mitte","Pankow","Prenzlaur Berg","Treptow","Weissensee"};
	public static string[] Cities = {"Berlin","Leipzig","Dresden","Rostock","Postdam","Cottbus","Dessau"};
	public static string[] Seasons = {"spring","summer","autumn","winter"};
	public static string[] Months = {"January","February","March","April","may","June","July","August","September","October","November","December"};
	public static string[] Factions = {"sis","cia"};
	public static string[] Factions_Short = {"sis_s","cia_s"};
	public static string[] Specializations = {"p_title","r_title","g_title","e_title","m_title"};
	public static string[] Perks = {"cs_title","st_title"};
	public static string[] Perk_Descriptions = {"cs_desc","st_desc"};
	public static string[] Genders = {"Male","Female"};
	public static string[] SkinColors = {"1","2","3"};
	public static string[] HairColors = {"Brown","Red","Blonde"};
	public static string[] EyeColors = {"Green","Blue","Hazel","Gray","Brown"};
	public static string[][] ccStrings = {Neighborhoods,Seasons,Factions,Specializations,Perks,Genders,SkinColors,HairColors,EyeColors};
	Console Menu_BG,Menu_FG;
	public enum STATE {Menu,Game};
	public enum MENU {MM,NG,LG,AM,AA,GO,AH,RE};
	public enum GAME {UNPAUSED,PAUSED};
	public enum SUB {A,B,C};
	public STATE state = STATE.Menu;
	public GAME gameState = GAME.UNPAUSED;
	public MENU mMenuState = MENU.MM;
	public SUB nGMenuState = SUB.A;
	public SUB gOMenuState = SUB.A;
	public int selected = 0;
	//CC Variables
	public List<Line.Point> visible = new List<Line.Point>();
	static int neighborhood,season,faction,gender,specialization,perk,skinC,hairC,eyeC;
	static int cAge,cCOB;
	static System.DateTime cDOB;
	static Color32 sc1 = new Color32(229,194,152,255);
	static Color32 sc2 = new Color32(227,161,115,255);
	static Color32 sc3 = new Color32(204,132,67,255);
	static Color32 hc1 = new Color32(106,78,66,255);
	static Color32 hc2 = new Color32(141,74,67,255);
	static Color32 hc3 = new Color32(229,200,168,255);
	static Color32 ec1 = new Color32(172,186,114,255);
	static Color32 ec2 = new Color32(182,195,209,255);
	static Color32 ec3 = new Color32(209,187,119,255);
	static Color32 ec4 = new Color32(113,99,90,255);
	static Color32 ec5 = new Color32(56,0,7,255);
	public static Color32[] ccSkin = {sc1,sc2,sc3};
	public static Color32[] ccHair = {hc1,hc2,hc3};
	public static Color32[] ccEyes = {ec1,ec2,ec3,ec4,ec5};
	public static Color32[][] ccCols = {ccSkin,ccHair,ccEyes};
	int[] ccGeneral = {neighborhood,season,faction,specialization,perk,gender,skinC,hairC,eyeC};
	string CharacterName = null;
	string CharacterAlias = null;
	public NameGenerator gNG,aNG,bNG;
	bool confChar;
	//In game variables
	bool bgLoaded = false;
	public Vector2 cursorLoc = new Vector2(10,7);
	
	public string combatLog;
	
	Game_Manager GameManager;
	#region Initialization
	void Awake() {
		Screen.SetResolution(700,240,false);
		GameManager = this.gameObject.GetComponent<Game_Manager>();
		LoadStrings();
		SetStrings();
		Menu_BG = new Console("terminal10x16_gs_ro");
		Menu_FG = new Console("terminal10x16_gs_ro");
		
		gNG = new NameGenerator("German");
		aNG = new NameGenerator("American");
		bNG = new NameGenerator("British");
	}
	void Start() {
		LoadBackground("Menu Backgrounds/MM");
		ColorBackground();
		MenuForeground();
	}	
	void Update() {
		if(Application.loadedLevel == 1 && bgLoaded == false) {
			LoadBackground("Menu Backgrounds/G_Unpaused");
			ColorBackground();
			bgLoaded = true;
		}
	}
	
	public void Render() {
		Menu_BG.Render();
		Menu_FG.Render();
	}
	void LoadStrings() {
		TextAsset source = Resources.Load("Interface Strings") as TextAsset;
		foreach(string row in source.text.Split('\n')) {
			if(row.StartsWith("#"))
				continue;
			else
				loadedFields.Add(row);
		}
	}
	void SetStrings() {
		foreach(string field in loadedFields) {
			string f = field.Remove(field.IndexOf("=")).TrimEnd();
			if(Titles.Contains(f))
				Titles[System.Array.IndexOf(Titles,f)] = formatField(field);
			else if(MMItemTitles.Contains(f))
				MMItemTitles[System.Array.IndexOf(MMItemTitles,f)] = formatField(field);
			else if(CCTitles.Contains(f))
				CCTitles[System.Array.IndexOf(CCTitles,f)] = formatField(field);
			else if(CCTitles2.Contains(f))
				CCTitles2[System.Array.IndexOf(CCTitles2,f)] = formatField(field);
			else if(Seasons.Contains(f))
				Seasons[System.Array.IndexOf(Seasons,f)] = formatField(field);
			else if(Factions.Contains(f))
				Factions[System.Array.IndexOf(Factions,f)] = formatField(field);
			else if(Factions_Short.Contains(f))
				Factions_Short[System.Array.IndexOf(Factions_Short,f)] = formatField(field);
			else if(Specializations.Contains(f))
				Specializations[System.Array.IndexOf(Specializations,f)] = formatField(field);
			else if(Perks.Contains(f))
				Perks[System.Array.IndexOf(Perks,f)] = formatField(field);
			else if(Perk_Descriptions.Contains(f))
				Perk_Descriptions[System.Array.IndexOf(Perk_Descriptions,f)] = formatField(field);
			else if(GPButtons.Contains(f))
				GPButtons[System.Array.IndexOf(GPButtons,f)] = formatField(field);
		}
	}
	#endregion
	#region Background
	public void LoadBackground(string filename) {
		Menu_BG.Flush();
		TextAsset source = Resources.Load(filename) as TextAsset;
		string[] rows = source.text.Split('\n');
		int y = 0;
		foreach(string row in rows) {
			char[] cont = row.ToCharArray();
			int x = 0;
			foreach(char c in cont) {
				Menu_BG.Put(x,y,CharOps.ToASCII(c));
				x++;
			}
			y++;
		}
	}
	public void ColorBackground() {
		switch(state) {
			case STATE.Menu: {
				switch(mMenuState) {
					case MENU.MM: {//UNFINISHED -- MAIN MENU
						for(int x=0;x<25;x++) {//AMERICAN FLAG
							for(int y=0;y<15;y++) {
								if(y % 2 != 1 && (x > 5 && y < 7 || y >= 7))
									Menu_BG.Recolor(x,y,new Color32(),ColorLib.Red());
								else if(y < 7 && x <= 5)
									Menu_BG.Recolor(x,y,new Color32(),ColorLib.Blue());
								if(y%2 != 1 && y <7 && x == 5)
									Menu_BG.Recolor(x,y,ColorLib.Red());
							}
						}
						for(int x=45;x<70;x++) {//EAST GERMAN FLAG
							for(int y=0;y<15;y++) {
								if(y<5)
									Menu_BG.Recolor(x,y,new Color32(),ColorLib.Black());
								else if(y >= 5 && y < 10)
									Menu_BG.Recolor(x,y,new Color32(),ColorLib.Red());
								else if(y >= 10)
									Menu_BG.Recolor(x,y,new Color32(),ColorLib.Yellow("natural"));
							}
						}
						for(int y=0;y<15;y++) { //BORDER COLORING
							Menu_BG.Recolor(25,y,ColorLib.LightGray("natural"));
							Menu_BG.Recolor(44,y,ColorLib.LightGray("natural"));
						}
						break;
					}
					case MENU.NG: {//UNFINISHED -- NEW GAME
						switch(nGMenuState) {
							case SUB.A: {
								int[] xCoordinates = {0,69};
								foreach(int x in xCoordinates) {
									Menu_BG.Recolor(x,0,ColorLib.Black(),ColorLib.White());
								}
								Menu_BG.Recolor(9,14,ColorLib.Black(),ColorLib.White());
								Menu_BG.Recolor(60,14,ColorLib.White(),ColorLib.White());
								break;
							}
							case SUB.B: {//CONFIRM SCREEN
								Menu_BG.Recolor(60,14,ColorLib.White(),ColorLib.White());
								Menu_BG.Recolor(9,14,ColorLib.Black(),ColorLib.White());
								break;
							}
						}
						break;
					}
					case MENU.LG: {
						break;
					}
				}
				break;
			}
			case STATE.Game: {
				switch(gameState) {
					case GAME.UNPAUSED: {
						int[] xC = {0,69};
						foreach(int x in xC) {
							Menu_BG.Recolor(x,14,ColorLib.Black(),ColorLib.LightGray("natural"));
							//Menu_BG.grid[14*70+x].background = ColorLib.LightGray("natural");
							//Menu_BG.grid[14*70+x].foreground = ColorLib.Black();
						}
						for(int x = 0; x < 70; x++) {
							for(int y = 0; y < 15; y++) {
								Menu_BG.Recolor(x,y,ColorLib.LightGray("natural"));
								//Menu_BG.grid[y*70+x].foreground = ColorLib.LightGray("natural");
							}
						}
						int start = 35-CharacterAlias.Length/2;
						int end = start + CharacterAlias.Length;
						//for(int x = start;x < end;x++) {//NAME BG
						//	Menu_BG.Put(x,0,(char)219,ColorLib.LightGray("natural"));
						//}
						//for(int x = start-11;x < start-1;x++) {//DATE BG
						//	Menu_BG.Put(x,0,(char)219,ColorLib.LightGray("natural"));
						//}
						for(int x = end+1;x < end+9;x++) {
							Menu_BG.Put(x,0,(char)219,ColorLib.LightGray("natural"));
						}
						for(int y = 1; y < 14;y++) {
							Menu_BG.Put(20,y,(char)179,ColorLib.LightGray("natural"));
							Menu_BG.Put(46,y,(char)179,ColorLib.LightGray("natural"));
						}
						break;
					}
					case GAME.PAUSED: {
						for(int x = 0;x < 70; x++) {
							for(int y = 0; y < 15; y++) {
								Menu_BG.Recolor(x,y,ColorLib.LightGray("natural"));
							}
						}
						break;
					}
				}
				break;
			}
		}
	}
	#endregion
	#region Foreground
	void MenuForeground() {
		switch(state) {
			#region Main Menu
			case STATE.Menu: {//UNFINISHED -- MENUS (OUT OF GAME)
				switch(mMenuState) {
					case MENU.MM: {//UNFINISHED -- MAIN MENU
						//usable width = 18
						Menu_FG.Put(26,0,Titles[0]);
						Menu_FG.Put(26+(18-Titles[1].Length)/2,1,Titles[1]);
						foreach(string title in MMItemTitles) {
							int index = System.Array.IndexOf(MMItemTitles,title);
							if(index == selected)
								Menu_FG.Put(26+(18-title.Length)/2,3+System.Array.IndexOf(MMItemTitles,title),title,ColorLib.LightBlue("natural"));
							else
								Menu_FG.Put(26+(18-title.Length)/2,3+System.Array.IndexOf(MMItemTitles,title),title);
						}
						break;
					}
					case MENU.NG: {//UNFINISHED -- NEW GAME
						switch(nGMenuState) {
							case SUB.A: {//UNFINISHED -- FIRST SCREEN
								foreach(string title in CCTitles) {
									int index = System.Array.IndexOf(CCTitles,title);
									if(index == selected) {
										Menu_FG.Put(2,1+2*index,title,ColorLib.LightBlue("natural"));
										Menu_FG.Put(2,2+2*index,27);
										Menu_FG.Put(3+ccStrings[index][ccGeneral[index]].Length,2+2*index,26);
									}
									else
										Menu_FG.Put(2,1+2*index,title);
								}
								foreach(string[] s in ccStrings) {
									int index = System.Array.IndexOf(ccStrings,s);
									if(index < 6)
										Menu_FG.Put(3,2+(2*index),s[ccGeneral[index]]);
									else if(index >= 6)
										Menu_FG.Put(32,2+(2*(index-6)),s[ccGeneral[index]]);
								}
								foreach(string title in CCTitles2) {
									int index = System.Array.IndexOf(CCTitles2,title);
									if(index == selected-6) {
										Menu_FG.Put(31,1+2*index,title,ColorLib.LightBlue("natural"));
										Menu_FG.Put(31,2+2*index,27);
										Menu_FG.Put(32+ccStrings[index+6][ccGeneral[index+6]].Length,2+2*index,26);
									}
									else
										Menu_FG.Put(31,1+2*index,title);
									Menu_FG.Put(46,1+2*index,(char)219,ccCols[index][ccGeneral[index+6]]);
								}
								Menu_FG.Put(61,14,"[REVIEW >",ColorLib.Black());
								Menu_FG.Put(0,14,"< CANCEL]",ColorLib.Black());
								break;
							}
							case SUB.B: {//UNFINISHED -- DESCRIPTION
								//Figure out word wrap for this.
								string[] pronouns = {"He","She"};
								string[] n = CharacterName.Split(' ');
								string name = CharacterName+", "+cAge+", is a " +Genders[ccGeneral[5]].ToLower() + " agent working for the "+Factions_Short[ccGeneral[2]]+".";
								string spec = n[1] + " excels with "+Specializations[ccGeneral[3]].ToLower()+".";
								string ali = pronouns[ccGeneral[5]]+" operates under the alias "+'"'+CharacterAlias+'"'+".";
								//Add professions.
								string[] al = CharacterAlias.Split(' ');
								string cob = al[1] +", a worker," +" was born in "+Cities[cCOB]+" on "+Months[cDOB.Month-1] + " " + cDOB.Day + ", " + cDOB.Year+".";
								Menu_FG.Put(1,1,name);
								Menu_FG.Put(1,2,spec);
								Menu_FG.Put(1,3,ali);
								Menu_FG.Put(1,4,cob);
								Menu_FG.Put(1,5,"Perk: " + Perks[ccGeneral[4]]);
								Menu_FG.Put(0,14,"< RETURN]",ColorLib.Black());
								Menu_FG.Put(61,14,"[DEPLOY >",ColorLib.Black());
								if(confChar) {//CONFIRM DEPLOYMENT
									Menu_FG.Put(25,13,"CONFIRM DEPLOYMENT (c)");
								}
								break;
							}
							case SUB.C: {
								break;
							}
						}
						break;
					}
					case MENU.LG: {//UNFINISHED
						break;
					}
					case MENU.AM: {//UNFINISHED
						break;
					}
					case MENU.AA: {//UNFINISHED
						break;
					}
					case MENU.GO: {//UNFINISHED
						switch(gOMenuState) {
							case SUB.A: {
								break;
							}
							case SUB.B: {
								break;
							}
							case SUB.C: {
								break;
							}
						}
						break;
					}
					case MENU.AH: {//UNFINISHED
						break;
					}
					case MENU.RE: {//UNFINISHED
						break;
					}
				}
				break;
			}
			#endregion
			#region Game
			case STATE.Game: {//UNFINISHED -- GAME INTERFACE & IN GAME MENUS
				switch(gameState) {
					#region Unpaused
					case GAME.UNPAUSED: {//UNFINISHED -- IN GAME UI
						World_Manager w = this.gameObject.GetComponent<World_Manager>();
						int px = (int)GameObject.FindWithTag("Player").GetComponent<Object>().Coordinates.x;
						int py = (int)GameObject.FindWithTag("Player").GetComponent<Object>().Coordinates.y;
						int pz = (int)GameObject.FindWithTag("Player").GetComponent<Object>().Coordinates.z;
						Object player = GameObject.FindWithTag("Player").GetComponent<Object>();
						string date = this.gameObject.GetComponent<Game_Manager>().curDate.ToString("dd/MM/yyyy");
						string time = this.gameObject.GetComponent<Game_Manager>().curDate.ToString("HH:mm:ss");
						int start = 35-CharacterAlias.Length/2;
						int end = start + CharacterAlias.Length;
						Menu_FG.Put(start,0,CharacterAlias,ColorLib.White());
						//Menu_FG.Put(start-11,0,date,ColorLib.White());
						//Menu_FG.Put(end+1,0,time,ColorLib.White());
						Menu_FG.Put(0,0,GameManager.turnsPassed.ToString()); //TURNS PASSED - DEBUG
						Menu_FG.Put(21,5,"a - AIM/CANCEL");
						Menu_FG.Put(21,6,"enter while aiming - FIRE");
						Menu_FG.Put(21,7,"r - RELOAD");
						Menu_FG.Put(21,8,"p - PICKUP");
						Menu_FG.Put(21,9,"= - WAIT");
						if(player.player.health <= 0)
							Menu_FG.Put(64,0,"*DEAD*",ColorLib.Yellow("cga"));
						//Actual display area. Center is 10x7
						#region Display area. Center @ (10,7)
						Line width = new Line(px-9,py,px+9,py);
						Line height = new Line(px,py-6,px,py+6);
						int endW = w.world.map.GetLength(0);
						int endH = w.world.map.GetLength(1);
						foreach(Line.Point wi in width.getPoints()) {
							int x = width.getPoints().IndexOf(wi);
							foreach(Line.Point h in height.getPoints()) {
								int y = height.getPoints().IndexOf(h);
								if(wi.x < 0 || h.y < 0 || wi.x >= endW || h.y >= endH || w.world.map[wi.x,h.y,pz].tChar == (char)0 || w.world.map[wi.x,h.y,pz].hasBeenSeen == false)
									continue;
								char c = w.world.map[wi.x,h.y,pz].tChar;
								Color col = w.world.map[wi.x,h.y,pz].tColor;
								if(y == 6 && x == 9) {
									c = GameObject.FindWithTag("Player").GetComponent<Object>().display[0];
									col = ColorLib.White();
								}
								Menu_FG.Put(x+1,y+1,c,col);
							}
						}
						#endregion	
						#region Information
						Color healthCol = ColorLib.Green("cga");
						if(player.player.health < 70 && player.player.health >= 40)
							healthCol = ColorLib.Yellow("cga");
						if(player.player.health < 40)
							healthCol =	ColorLib.Red("cga");
						Menu_FG.Put(21,1,player.player.health.ToString(),healthCol);
						string wpnstring = null;
						if(player.player.EquippedWeapon == null)
							wpnstring = "Fists";
						else
							wpnstring = player.player.EquippedWeapon.name;
							
						Menu_FG.Put (21,2,wpnstring);
						if(wpnstring != "Fists" && player.player.EquippedWeapon.GetComponent<Object>().id.ToString()[2] == '2') {
							string ammoStr = null;
							//Color amc = new Color32(181,166,0,255);
							Color amc = ColorLib.LightGray("cga");
							int cap = player.player.EquippedWeapon.GetComponent<Object>().item.capacity;
							int cur = player.player.EquippedWeapon.GetComponent<Object>().item.curRounds;
							for(int i = 0; i < cap; i++)
								ammoStr += (char)254;
							Menu_FG.Put(21,3,ammoStr,amc);
							ammoStr = string.Empty;
							for(int i = 0; i < cur; i++)
								ammoStr += (char)254;
							amc = new Color32(181,166,0,255);
							Menu_FG.Put(21,3,ammoStr,amc);
						}
						#endregion
						#region Combat Log
						Menu_FG.Put (1,14,combatLog);
						#endregion
						break;
					}
					#endregion
					#region Paused
					case GAME.PAUSED: {//UNFINISHED -- PAUSE MENU
						Menu_FG.Put(30,0,"*PAUSED*",ColorLib.White());
						foreach(string s in GPButtons) {
							int index = System.Array.IndexOf(GPButtons,s);
							if(selected == index)
								Menu_FG.Put(34-s.Length/2,2+2*index,s,ColorLib.LightBlue("natural"));
							else
								Menu_FG.Put(34-s.Length/2,2+2*index,s,ColorLib.White());
						}
						break;
					}
					#endregion
				}
				break;
			}
			#endregion
		}
	}
	public void Refresh() {
		Menu_FG.Flush();
		MenuForeground();
	}
	#endregion
	void ObtainObjects(World_Manager w, int x, int y, int z) {
		string alphabet = "abcdefghijklmno";
		foreach(GameObject gobj in w.world.map[x,y,z].tileContents) {
			int index = w.world.map[x,y,z].tileContents.IndexOf(gobj);
			Menu_FG.Put(47,1 + index,alphabet[index]+"."+gobj.name);
		}
	}
	
	void DisplayContents(World_Manager w, int x, int y, int z) {
		List<string> tiDisp = new List<string>();
		foreach(GameObject gobj in w.world.map[x,y,z].tileContents) {
			Menu_FG.Put(47,1 + w.world.map[x,y,z].tileContents.IndexOf(gobj),gobj.name);
		}
	}
	public void DisplayCursor(World_Manager w) {
		int px = (int)GameObject.FindWithTag("Player").GetComponent<Object>().Coordinates.x;
		int py = (int)GameObject.FindWithTag("Player").GetComponent<Object>().Coordinates.y;
		int cx = (int)cursorLoc.x;
		int cy = (int)cursorLoc.y;
		
		int[] mx = {px-9,px-8,px-7,px-6,px-5,px-4,px-3,px-2,px-1,px,px+1,px+2,px+3,px+4,px+5,px+6,px+7,px+8,px+9};
		int[] my = {py-6,py-5,py-4,py-3,py-2,py-1,py,py+1,py+2,py+3,py+4,py+5,py+6};
		
		Menu_FG.Put(cx,cy,'x',ColorLib.Yellow("cga"));
		DisplayContents(w, mx[cx-1], my[cy - 1],0);
	}
	public void DisplayLine(World_Manager w) {
		int px = (int)GameObject.FindWithTag("Player").GetComponent<Object>().Coordinates.x;
		int py = (int)GameObject.FindWithTag("Player").GetComponent<Object>().Coordinates.y;
		int cx = (int)cursorLoc.x;
		int cy = (int)cursorLoc.y;
		int[] mx = {px-9,px-8,px-7,px-6,px-5,px-4,px-3,px-2,px-1,px,px+1,px+2,px+3,px+4,px+5,px+6,px+7,px+8,px+9};
		int[] my = {py-6,py-5,py-4,py-3,py-2,py-1,py,py+1,py+2,py+3,py+4,py+5,py+6};
		Line l = new Line(10,7,cx,cy);
		Color32 col = ColorLib.Yellow("natural");
		foreach(Line.Point p in l.getPoints()) {
			if(w.world.map[mx[p.x-1],my[p.y-1],0].canMoveTo == false)
				col = ColorLib.Red("natural");
			
			if(p.x == cx && p.y == cy)
				Menu_FG.Put(p.x,p.y,'x',col);
			else if(l.getPoints().IndexOf(p) > 0)
				Menu_FG.Put(p.x,p.y,'.',col);
		}
	}
	void TransitionStates(MENU newState) {
		mMenuState = newState;
		string fn = System.Enum.GetName(typeof(MENU),newState);
		if(Resources.Load("Menu Backgrounds/"+fn) != null)
			LoadBackground("Menu Backgrounds/"+fn);
		else
			Menu_BG.Flush();
		ColorBackground();
		selected = 0;
	}
	//Implement keybind loading from a text file!
	public void Control() {
		switch(state) {
			case STATE.Menu: {
				switch(mMenuState) {
					case MENU.MM: {//UNFINISHED -- MAIN MENU
						int min = 0;
						int max = 6;
						if(Input.GetKeyDown("up") && selected > min) {
							selected--;
							GameManager.AudioManager.PlayMenuSound();
						}
						else if(Input.GetKeyDown("down") && selected < max) {
							selected++;
							GameManager.AudioManager.PlayMenuSound();
						}
						else if(Input.GetKeyDown(KeyCode.Return)) {
							if(selected == 0) {
								selected = 0;
								NameGenerator[] nGens = {bNG,aNG};
								CharacterAlias = gNG.Name(ccGeneral[5]);
								CharacterName = nGens[ccGeneral[2]].Name(ccGeneral[5]);
								cAge = Random.Range(22,40);
								cCOB = Random.Range(0,Cities.Length);
								int[] monLeng = {32,29,32,31,32,31,32,32,31,32,31,32};
								int mo = Random.Range(0,12);
								cDOB = new System.DateTime(1980-cAge,mo+1,Random.Range(1,monLeng[mo]));
								
								GameObject p = GameObject.FindWithTag("Player");
								p.GetComponent<Object>().player = new Object.Player(p.GetComponent<Object>(),CharacterAlias,CharacterName,ccGeneral);
								//p.GetComponent<Object>().Coordinates = new Vector3(2,2,0);
								DontDestroyOnLoad(p);
								DontDestroyOnLoad(GameObject.FindWithTag("Manager"));
								state = STATE.Game;
								mMenuState = MENU.MM;
								Menu_BG.Flush();
								int w = UnityEngine.Random.Range(5,20);
								int l = UnityEngine.Random.Range(5,15);
								Debug.Log (w + "x" + l);
								this.gameObject.GetComponent<World_Manager>().world = new World("s",w,l,1,10,6);
								GameObject weapon = GameObject.Instantiate(GameObject.Find("Makarov PM (G)")) as GameObject;
								weapon.tag = "Item";
								weapon.name = weapon.GetComponent<Object>().name;
								weapon.transform.parent = p.transform;
								weapon.GetComponent<Object>().item.curRounds = weapon.GetComponent<Object>().item.capacity;
								DontDestroyOnLoad(weapon);
								p.GetComponent<Object>().player.EquippedWeapon = weapon;
								for(int i = 0; i < 2; i++) {
									GameObject magazine = GameObject.Instantiate(GameObject.Find("9x18mm Makarov Magazine (G)")) as GameObject;
									DontDestroyOnLoad(magazine);
									Object o = magazine.GetComponent<Object>();
									magazine.name = o.name;
									o.item.capacity = 10;
									magazine.transform.parent = p.transform;
								}
								this.gameObject.GetComponent<World_Manager>().world.IndexContents();
								Application.LoadLevel("HUMINT_Game");
								GameManager.FOV();
								this.gameObject.GetComponent<World_Manager>().world.UpdateContents();
							}
							//TransitionStates((MENU)(1+selected));
						}
						break;
					}
					case MENU.NG: {//UNFINISHED -- NEW GAME MENU
						switch(nGMenuState) {
							case SUB.A: {
								if(Input.GetKeyDown("up") && selected > 0)
									selected--;
								else if(Input.GetKeyDown("down") && selected < ccGeneral.Length-1)
									selected++;
								else if(Input.GetKeyDown("right") && ccGeneral[selected] < ccStrings[selected].Length-1)
									ccGeneral[selected]++;
								else if(Input.GetKeyDown("left") && ccGeneral[selected] > 0)
									ccGeneral[selected]--;
								else if(Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(",")) {
									TransitionStates(MENU.MM);
								}
								else if(Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(".")) {
									selected = 0;
									NameGenerator[] nGens = {bNG,aNG};
									CharacterAlias = gNG.Name(ccGeneral[5]);
									CharacterName = nGens[ccGeneral[2]].Name(ccGeneral[5]);
									cAge = Random.Range(22,40);
									cCOB = Random.Range(0,Cities.Length);
									int[] monLeng = {32,29,32,31,32,31,32,32,31,32,31,32};
									int mo = Random.Range(0,12);
									cDOB = new System.DateTime(1980-cAge,mo+1,Random.Range(1,monLeng[mo]));
									nGMenuState = SUB.B;
									ColorBackground();
								}
								break;
							}
							case SUB.B: {
								if(Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(",")) {
									selected = 0;
									LoadBackground("Menu Backgrounds/NG");
									nGMenuState = SUB.A;
									ColorBackground();
								}
								else if(Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(".")) {
									confChar = true;
								}
								if(confChar == true && Input.GetKeyDown("c")) {
									selected = 0;
									GameObject p = GameObject.FindWithTag("Player");
									p.GetComponent<Object>().player = new Object.Player(p.GetComponent<Object>(),CharacterAlias,CharacterName,ccGeneral);
									//p.GetComponent<Object>().Coordinates = new Vector3(2,2,0);
									DontDestroyOnLoad(p);
									DontDestroyOnLoad(GameObject.FindWithTag("Manager"));
									state = STATE.Game;
									mMenuState = MENU.MM;
									Menu_BG.Flush();
									int w = UnityEngine.Random.Range(5,20);
									int l = UnityEngine.Random.Range(5,15);
									Debug.Log (w + "x" + l);
									this.gameObject.GetComponent<World_Manager>().world = new World("s",w,l,1,10,6);
									GameObject weapon = GameObject.Instantiate(GameObject.Find("Makarov PM (G)")) as GameObject;
									weapon.tag = "Item";
									weapon.name = weapon.GetComponent<Object>().name;
									weapon.transform.parent = p.transform;
									weapon.GetComponent<Object>().item.curRounds = weapon.GetComponent<Object>().item.capacity;
									DontDestroyOnLoad(weapon);
									p.GetComponent<Object>().player.EquippedWeapon = weapon;
									for(int i = 0; i < 2; i++) {
										GameObject magazine = GameObject.Instantiate(GameObject.Find("9x18mm Makarov Magazine (G)")) as GameObject;
										DontDestroyOnLoad(magazine);
										Object o = magazine.GetComponent<Object>();
										magazine.name = o.name;
										o.item.capacity = 10;
										magazine.transform.parent = p.transform;
									}
									this.gameObject.GetComponent<World_Manager>().world.IndexContents();
									Application.LoadLevel("HUMINT_Game");
									GameManager.FOV();
									this.gameObject.GetComponent<World_Manager>().world.UpdateContents();
								}
								break;
							}
						}
						break;
					}
					case MENU.LG: {//UNFINISHED
						break;
					}
					case MENU.AM: {//UNFINISHED
						break;
					}
					case MENU.AA: {//UNFINISHED
						break;
					}
					case MENU.GO: {//UNFINISHED
						break;
					}
					case MENU.AH: {//UNFINISHED
						break;
					}
					case MENU.RE: {//UNFINISHED
						break;
					}
				}
				if(Input.anyKeyDown)
					Refresh();
				break;
			}
		}
	}
	string formatField(string s) {
		char prevChar = new char();
		int trimEnd = 0;
		foreach(char c in s.ToCharArray()) {
			if(c == '=') {
				trimEnd = System.Array.IndexOf(s.ToCharArray(),c);
				break;
			}
			prevChar = c;
		}
		s = s.Remove(0,trimEnd).TrimStart('=',' ');
		return s;
	}
}