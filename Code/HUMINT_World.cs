using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using libhumint;
//YO DUDE - CREATE A CHART FOR THE CHARACTERS THAT CORRESPOND TO COLORS. YOU CAN EASILY HAVE 52 IF YOU JUST USE LC/UC LETTERS
public class HUMINT_World : MonoBehaviour {
	public Area[,] worldMap;
	public Area map1,map2;

	void Awake() {
		map1 = MapReader("1,1");
		map2 = MapReader("1,2");
	}
	
	public void ChangeMap(char dir) {
		GameObject player = GameObject.FindWithTag("Player");
		if(dir == '^')
		{
			player.GetComponent<HUMINT_Object>().Coordinates.y = 17;
			player.GetComponent<HUMINT_Object>().curMap.y -=1;
		}
		if(dir == 'v')
		{
			player.GetComponent<HUMINT_Object>().Coordinates.y = 0;
			player.GetComponent<HUMINT_Object>().curMap.y +=1;
		}
	}
	
	public Area[,] wMap() {
		Area[,] maps = new Area[10,10];
		maps[0,0] = map1;
		maps[0,1] = map2;
		return maps;
	}
	public Vector2 GetCoords(int num,int w) {
		int y = (int)num/w;
		int x = num-(y*w);
		Vector2 loc = new Vector2(x,y);
		return loc;
	}
	
	Area MapReader(string filename) {
		TextAsset source = Resources.Load(filename) as TextAsset;
		string[] layers = source.text.Split('}');
		string[] mapRows = layers[0].Trim('{').TrimStart().TrimEnd().Split('\n');
		string[] collisionRows = layers[1].TrimStart().Trim('{').TrimStart().TrimEnd().Split('\n');
		string[] colorRows = layers[2].TrimStart().Trim('{').TrimStart().TrimEnd().Split('\n');
		Area mapToSet = new Area(0,0,33,18);
		int y = 0;
		foreach(string s in mapRows)
		{
			int x = 0;
			foreach(string sa in s.Split(']'))
			{
				if(x<33)
				{	
					int ch = 0;
					bool a = System.Int32.TryParse(sa.TrimStart('['),out ch);
					mapToSet.Set(x,y,(char)ch,ColorLib.Red("natural"));
					x+=1;
				}
			}
			y+=1;
		}
		y = 0;
		foreach(string s in collisionRows)
		{
			int x = 0;
			foreach(string sa in s.Split(']'))
			{
				if(x<33)
				{
					foreach(char c in sa.TrimStart('[').TrimEnd().ToCharArray())
					{
						if(c == '-')
							mapToSet.collisionMap[x,y] = false;
						else
							mapToSet.collisionMap[x,y] = true;
					}
					x+=1;
				}
			}
			y+=1;
		}
		y = 0;
		foreach(string s in colorRows)
		{
			int x = 0;
			foreach(string sa in s.Split(']'))
			{
				if(x<33)
				{
					foreach(char c in sa.TrimStart('[').TrimEnd().ToCharArray())
					{
						if(c == 'A')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Blue("natural");
						else if(c == 'B')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Green("natural");
						else if(c == 'C')
							ColorLib.Cyan("natural");
						else if(c == 'D')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Red("natural");
						else if(c == 'E')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Magenta("natural");
						else if(c == 'F')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Brown("natural");
						else if(c == 'G')
							mapToSet.mTiles[x,y].tileColor = ColorLib.DarkGray("natural");
						else if(c == 'H')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Blue("cga");
						else if(c == 'I')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Green("cga");
						else if(c == 'J')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Cyan("cga");
						else if(c == 'K')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Red("cga");
						else if(c == 'L')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Magenta("cga");
						else if(c == 'M')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Brown("cga");
						else if(c == 'N')
							mapToSet.mTiles[x,y].tileColor = ColorLib.DarkGray("cga");
						else if(c == 'O')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Blue();
						else if(c == 'P')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Green();
						else if(c == 'Q')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Cyan();
						else if(c == 'R')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Red();
						else if(c == 'S')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Magenta();
						else if(c == 'T')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Brown();
						else if(c == 'U')
							mapToSet.mTiles[x,y].tileColor = ColorLib.DarkGray();
						else if(c == 'a')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightBlue("natural");
						else if(c == 'b')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightGreen("natural");
						else if(c == 'c')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightCyan("natural");
						else if(c == 'd')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightRed();
						else if(c == 'e')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightMagenta("natural");
						else if(c == 'f')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Yellow("natural");
						else if(c == 'g')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightGray("natural");
						else if(c == 'h')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightBlue("cga");
						else if(c == 'i')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightGreen("cga");
						else if(c == 'j')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightCyan("cga");
						else if(c == 'k')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightRed("cga");
						else if(c == 'l')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightMagenta("cga");
						else if(c == 'm')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Yellow("cga");
						else if(c == 'n')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightGray("cga");
						else if(c == 'o')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightBlue();
						else if(c == 'p')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightGreen();
						else if(c == 'q')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightCyan();
						else if(c == 'r')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightRed();
						else if(c == 's')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightMagenta();
						else if(c == 't')
							mapToSet.mTiles[x,y].tileColor = ColorLib.Yellow();
						else if(c == 'u')
							mapToSet.mTiles[x,y].tileColor = ColorLib.LightGray();
					}
					x+=1;
				}
			}
			y+=1;
		}
		return mapToSet;
	}
}

public class Area {
	public Vector2 location;
	public Tile[,] mTiles;
	public bool[,] collisionMap;
	public bool[,] visionMap;
	public int width,height,index;
	public bool isActive;
	public List<GameObject> objectList = new List<GameObject>();
	int n,e,s,w;
	
	public Area(int a, int b,int w, int h) {
		width = w;
		height = h;
		mTiles = new Tile[w,h];
		collisionMap = new bool[w,h];
		for(int i=0;i<(w*h);i++)
		{
			for(int x=0;x<w;x++)
			{
				for(int y=0;y<h;y++)
				{
					collisionMap[x,y] = true;
					mTiles[x,y] = new Tile(x,y,'v',ColorLib.Red("natural"));
				}
			}
		}
	}
	public void Set(int x, int y, char c, Color32 col = new Color32(),bool canMoveTo = true,bool canSeeThrough = true) {
		mTiles[x,y] = new Tile(x,y,c,col);
	}
}

public class Tile {
	public char tileChar;
	public Vector2 loc;
	public bool isDoor;
	public bool isOpen;
	public int cost;
	public Color32 tileColor;
	public Tile(int x,int y,char c,Color32 tileC = new Color32()) {
		loc = new Vector2(x,y);
		tileChar = c;
		tileColor = tileC;
	}
}