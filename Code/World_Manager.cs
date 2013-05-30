using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using libhumint;
//Rewrite all of this. It kind of blows.
public class World_Manager : MonoBehaviour {
	public World world;

	void Awake() {
		world = new World();
		world.IndexContents();
		world.UpdateContents();
	}
	
	void Start() {
		
	}
	
	public Vector2 GetCoords(int num,int w) {
		int y = (int)num/w;
		int x = num-(y*w);
		Vector2 loc = new Vector2(x,y);
		return loc;
	}
}
public class World {
	public Tile[,,] map;
	public byte[,] costs;
	public List<GameObject> Contents = new List<GameObject>();
	
	//Map files now have 106x36 maps. This is to reduce bloat.
	public World(string filename = "Map Name") {
		ReadMap(filename);
	}
	
	void Populate() {
		
	}
	
	public void IndexContents() {//Initially indexes all NPCs and items to add to the map.
		string[] tagsToExclude = {"Generic Item","Manager","MainCamera","Player"};
		object [] allObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));
		foreach(object thisObject in allObjects) {
			GameObject processed = (GameObject)thisObject;
			if(processed.activeInHierarchy && tagsToExclude.Contains(processed.tag) == false) {
				Contents.Add(processed);
			}
		}
	}
	
	public void UpdateContents() {
		List<Vector3> locs = new List<Vector3>();
		foreach(GameObject obj in Contents) {
			Object o = obj.GetComponent<Object>();
			Vector3 location = o.Coordinates;
			locs.Add(location);
			int x = (int)location.x;
			int y = (int)location.y;
			int z = (int)location.z;
			map[x,y,z].tileContents.Add(obj);
		}
		
		int w = map.GetLength(0);
		int l = map.GetLength(1);
		int h = map.GetLength(2);
		costs = new byte[w,l];
		
		for(int x = 0; x < w; x++) {//This updates the tile's display depending on whatever is or isn't present.
			for(int y = 0; y < l; y++) {
				for(int z = 0; z < h; z++) {
					foreach(GameObject obj in map[x,y,z].tileContents) {
						if(obj.tag == "NPC") {
							map[x,y,z].npcPresent = true;
							map[x,y,z].tChar = obj.GetComponent<Object>().display[0];
						}
					}
					
					
					if(locs.Contains(map[x,y,z].loc) == false) {
						map[x,y,z].tileContents.Clear();
						map[x,y,z].npcPresent = false;
						map[x,y,z].tChar = map[x,y,z].tDefault;
						//map[x,y,z].tColor = map[x,y,z].tCDefault;
					}
				}
			}
		}
	}
	
	public void ReadMap(string filename) {
		TextAsset source = Resources.Load("Maps/"+filename) as TextAsset;
		string[] lines = source.text.Split('\n');
		int xLargest = 0;
		int yLargest = 0;
		int zLargest = 0;
		foreach(string line in lines) { //First pass to determine the size of map
			string coordinates = line.Split(';')[0];
			if(System.Int32.Parse(coordinates.Trim('(',')').Split(',')[0]) > xLargest)
				xLargest = System.Int32.Parse(coordinates.Trim('(',')').Split(',')[0]);
			if(System.Int32.Parse(coordinates.Trim('(',')').Split(',')[1]) > yLargest)
				yLargest = System.Int32.Parse(coordinates.Trim('(',')').Split(',')[1]);
			if(System.Int32.Parse(coordinates.Trim('(',')').Split(',')[2]) > zLargest)
				zLargest = System.Int32.Parse(coordinates.Trim('(',')').Split(',')[2]);
		}
		map = new Tile[xLargest+1,yLargest+1,zLargest+1];
		foreach(string line in lines) {//Second pass to determine contents.
			string _coordinates = line.Split(';')[0];
			string _rgb = line.Split(';')[1];
			string _char = line.Split(';')[2];
			string _moveable = line.Split(';')[3];
			string _seeThrough = line.Split(';')[4];
			string _door = line.Split(';')[5];
			int _x = System.Int32.Parse(_coordinates.Trim('(',')').Split(',')[0]);
			int _y = System.Int32.Parse(_coordinates.Trim('(',')').Split(',')[1]);
			int _z = System.Int32.Parse(_coordinates.Trim('(',')').Split(',')[2]);
			int _r = System.Int32.Parse(_rgb.Trim('(',')').Split(',')[0]);
			int _g = System.Int32.Parse(_rgb.Trim('(',')').Split(',')[1]);
			int _b = System.Int32.Parse(_rgb.Trim('(',')').Split(',')[2]);
			Color32 _color = new Color32((byte)_r,(byte)_g,(byte)_b,255);
			char _character = CharOps.ToASCII(_char.ToCharArray()[1]);
			bool _unblocked = (System.Int32.Parse(_moveable.ToCharArray()[1].ToString()) != 0);
			bool _transparent = (System.Int32.Parse(_seeThrough.ToCharArray()[1].ToString()) != 0);
			bool[] _isDoor = {(System.Int32.Parse(_door.ToCharArray()[1].ToString()) != 0),(System.Int32.Parse(_door.ToCharArray()[3].ToString()) != 0)};
			map[_x,_y,_z] = new Tile(_x,_y,_z,_character,_unblocked,_transparent,_color);
			map[_x,_y,_z].isDoor = _isDoor[0];
			map[_x,_y,_z].isOpen = _isDoor[1];
		}
	}
	public void WriteMapToXML() {
		string filename = "Map";
	}
	bool DetermineCollision(char c) {
		bool val = true;
		if(c != ' ')
			val = false;
		return val;
	}
	//Can remove DetermineColor whenever.
	public Color32 DetermineColor(char c) {
		Color32 col = new Color32();
		if(c == 'A')
			col = ColorLib.Blue("natural");
		else if(c == 'B')
			col = ColorLib.Green("natural");
		else if(c == 'C')
			ColorLib.Cyan("natural");
		else if(c == 'D')
			col = ColorLib.Red("natural");
		else if(c == 'E')
			col = ColorLib.Magenta("natural");
		else if(c == 'F')
			col = ColorLib.Brown("natural");
		else if(c == 'G')
			col = ColorLib.DarkGray("natural");
		else if(c == 'H')
			col = ColorLib.Blue("cga");
		else if(c == 'I')
			col = ColorLib.Green("cga");
		else if(c == 'J')
			col = ColorLib.Cyan("cga");
		else if(c == 'K')
			col = ColorLib.Red("cga");
		else if(c == 'L')
			col = ColorLib.Magenta("cga");
		else if(c == 'M')
			col = ColorLib.Brown("cga");
		else if(c == 'N')
			col = ColorLib.DarkGray("cga");
		else if(c == 'O')
			col = ColorLib.Blue();
		else if(c == 'P')
			col = ColorLib.Green();
		else if(c == 'Q')
			col = ColorLib.Cyan();
		else if(c == 'R')
			col = ColorLib.Red();
		else if(c == 'S')
			col = ColorLib.Magenta();
		else if(c == 'T')
			col = ColorLib.Brown();
		else if(c == 'U')
			col = ColorLib.DarkGray();
		else if(c == 'a')
			col = ColorLib.LightBlue("natural");
		else if(c == 'b')
			col = ColorLib.LightGreen("natural");
		else if(c == 'c')
			col = ColorLib.LightCyan("natural");
		else if(c == 'd')
			col = ColorLib.LightRed();
		else if(c == 'e')
			col = ColorLib.LightMagenta("natural");
		else if(c == 'f')
			col = ColorLib.Yellow("natural");
		else if(c == 'g')
			col = ColorLib.LightGray("natural");
		else if(c == 'h')
			col = ColorLib.LightBlue("cga");
		else if(c == 'i')
			col = ColorLib.LightGreen("cga");
		else if(c == 'j')
			col = ColorLib.LightCyan("cga");
		else if(c == 'k')
			col = ColorLib.LightRed("cga");
		else if(c == 'l')
			col = ColorLib.LightMagenta("cga");
		else if(c == 'm')
			col = ColorLib.Yellow("cga");
		else if(c == 'n')
			col = ColorLib.LightGray("cga");
		else if(c == 'o')
			col = ColorLib.LightBlue();
		else if(c == 'p')
			col = ColorLib.LightGreen();
		else if(c == 'q')
			col = ColorLib.LightCyan();
		else if(c == 'r')
			col = ColorLib.LightRed();
		else if(c == 's')
			col = ColorLib.LightMagenta();
		else if(c == 't')
			col = ColorLib.Yellow();
		else if(c == 'u')
			col = ColorLib.LightGray();
		return col;
	}
	bool DetermineFOV(char c) {
		bool val = true;
		if(c != ' ')
			val = false;
		return val;
	}
}
public struct Tile {
	public char tChar,tDefault;
	public bool hasBeenSeen;
	public Vector3 loc;
	public bool isDoor,isOpen,canMoveTo,canSeeThrough;
	public int cost;
	public Color32 tColor,tCDefault;
	public bool npcPresent;
	public List<GameObject> tileContents;
	
	public Tile(int x, int y, int z,char c = '.', bool canM = true, bool canS = true,Color32 tileC = new Color32()) {
		loc = new Vector3(x,y,z);
		hasBeenSeen = false;
		tDefault = c;
		tChar = c;
		tColor = tileC;
		tCDefault = tileC;
		cost = 0;
		canMoveTo = canM;
		canSeeThrough = canS;
		isDoor = false;
		isOpen = false;
		npcPresent = false;
		tileContents = new List<GameObject>();
	}
	
	public void Open() {
		tChar = '.';
		tDefault = tChar;
		canMoveTo = true;
		canSeeThrough = true;
		isOpen = true;
	}
	public void Close() {
		tDefault = (char)219;
		tChar = tDefault;
		canMoveTo = false;
		canSeeThrough = false;
		isOpen = false;
	}
}