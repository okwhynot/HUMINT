using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using libhumint;
//Rewrite all of this. It kind of blows.
public class HUMINT_World : MonoBehaviour {
	public World world;

	void Awake() {
		world = new World(3,3);
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
	public Tile[,,] contents;
	public Tile[,] z1;
	public Tile[,] z2;
	public int w,h;
	public List<GameObject> Items = new List<GameObject>();
	public List<GameObject> Entities = new List<GameObject>();
	
	//Map files now have 106x36 maps. This is to reduce bloat.
	public World(int aW, int aH) {
		w = aW*106;
		h = aH*36;
		z1 = new Tile[aW*106,aH*36];
		z2 = new Tile[aW*106,aH*36];
		ReadMapFromFile(1,1,1);
		//ReadMapFromFile(2,1,1);
		//ReadMapFromFile(1,2,1);
		ReadMap("Map Name");
	}
	public void ReadMap(string filename) {
		TextAsset source = Resources.Load("Maps/"+filename) as TextAsset;
		string[] lines = source.text.Split('\n');
		foreach(string tile in lines) {
			string[] contents = tile.Split(';');
			foreach(string cont in contents) {
				Debug.Log (cont);
			}
		}
	}	
	public void ReadMapFromFile(int x, int y, int z) {
		string filename = x + "," + y + "," + z;
		TextAsset source = Resources.Load(filename) as TextAsset;
		string[] sourceRows = source.text.Split('\n');
		//1-36 (array values) = characters
		for(int index = 1; index <= 36; index++) {
			char[] contents = sourceRows[index].ToCharArray();
			char[] collision = sourceRows[index+37].ToCharArray();
			char[] color = sourceRows[index+74].ToCharArray();
			char[] fov = sourceRows[index+111].ToCharArray();
			for(int hIndex = 0;hIndex<106;hIndex++) {
				if(z == 1)
				{
					z1[hIndex+106*(x-1),index+36*(y-1)].tChar = CharOps.ToASCII(contents[hIndex]);
					z1[hIndex+106*(x-1),index+36*(y-1)].tDefault = CharOps.ToASCII(contents[hIndex]);
					z1[hIndex+106*(x-1),index+36*(y-1)].canMoveTo = DetermineCollision(collision[hIndex]);
					z1[hIndex+106*(x-1),index+36*(y-1)].tColor = DetermineColor(color[hIndex]);
					z1[hIndex+106*(x-1),index+36*(y-1)].tCDefault = DetermineColor(color[hIndex]);
					z1[hIndex+106*(x-1),index+36*(y-1)].canSeeThrough = DetermineFOV(fov[hIndex]);
					if(CharOps.ToASCII(contents[hIndex]) == (char)219)
						z1[hIndex+106*(x-1),index+36*(y-1)].isDoor = true;
				}
				else if(z == 2)
				{
					z2[hIndex+106*(x-1),index+36*(y-1)].tChar = CharOps.ToASCII(contents[hIndex]);
					z2[hIndex+106*(x-1),index+36*(y-1)].canMoveTo = DetermineCollision(collision[hIndex]);
					z2[hIndex+106*(x-1),index+36*(y-1)].tColor = DetermineColor(color[hIndex]);
					z2[hIndex+106*(x-1),index+36*(y-1)].canSeeThrough = DetermineFOV(fov[hIndex]);
					if(CharOps.ToASCII(contents[hIndex]) == (char)219)
						z2[hIndex+106*(x-1),index+36*(y-1)].isDoor = true;
				}
			}
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
		if((int)c == 219) {
			isDoor = true;
			isOpen = false;
		}
		else {
			isDoor = false;
			isOpen = true;
		}
	}
	public void Open() {
		tChar = '.';
		tDefault = '.';
		canMoveTo = true;
		canSeeThrough = true;
		isOpen = true;
	}
	public void Close() {
		tChar = (char)219;
		tDefault = (char)219;
		canMoveTo = false;
		canSeeThrough = false;
		isOpen = false;
	}
}