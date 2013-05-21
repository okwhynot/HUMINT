using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using libhumint;

//Somewhere in loading the map, (0,0,0)'s character is deleted.

public class MapEditor : EditorWindow {
	public string filename;
	public Tile[,,] map;
	public int x,y,z;
	int sX,sY,sZ;
	int dispX,dispY,dispZ;
	Vector2 _scrollPos;
	string tileCharacter = ".",selTile = "(?,?,?)";
	Color32 tileCol;
	List<Tile> selectedTiles = new List<Tile>();
	bool tileMovable = true,tileTransparent = true,tileIsDoor,tileIsOpen;
	
	[MenuItem ("HUMINT/Map Editor")]
	static void Init () {
		//Find an open window if one exists.
		MapEditor editor = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor));
		editor.title = "Map Editor";
		editor.ReadRecents();
		editor.LoadMap(editor.filename,editor.map);
	}
	
	void OnFocus () {
		ReadRecents();
		//LoadMap(filename);
		LoadMap(filename, map);
	}
	
	void OnLostFocus () {
		//SaveMap(filename);
		SetRecents();
	}
	
	void OnGUI () {
		GUIStyle s = new GUIStyle();
		s = GUI.skin.label;
		s.margin.left = -10;
		s.margin.right = -10;
		s.margin.top = 0;
		s.margin.bottom = 0;
		EditorGUILayout.BeginHorizontal("Box");
			GUILayout.Label(selTile);
			tileCharacter = GUILayout.TextArea(tileCharacter,1,GUILayout.Width(17));
			tileCol = EditorGUILayout.ColorField("",tileCol,GUILayout.Width(50));
			GUILayout.Label("Movable");
			tileMovable = EditorGUILayout.Toggle("",tileMovable,GUILayout.Width(17));
			GUILayout.Label("Transparent");
			tileTransparent = EditorGUILayout.Toggle("",tileTransparent,GUILayout.Width(17));
			GUILayout.Label("Door");
			tileIsDoor = EditorGUILayout.Toggle("",tileIsDoor,GUILayout.Width(17));
			GUILayout.Label("Open");
			tileIsOpen = EditorGUILayout.Toggle("",tileIsOpen,GUILayout.Width(17));
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("^",GUILayout.ExpandWidth(false)) && dispY > 0) {
				dispY-=1;
			}
			if(GUILayout.Button("<",GUILayout.ExpandWidth(false)) && dispX > 0) {
				dispX-=1;
			}
			GUILayout.Label("("+dispX+","+dispY+","+dispZ+")");
			if(GUILayout.Button(">",GUILayout.ExpandWidth(false)) && dispX < 13) {
				dispX+=1;
			}
			if(GUILayout.Button("v",GUILayout.ExpandWidth(false)) && dispY < 13) {
				dispY+=1;
			}
			
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Apply",GUILayout.ExpandWidth(false))) {
				map[sX,sY,sZ] = new Tile(sX,sY,sZ,tileCharacter.ToCharArray()[0],tileMovable,tileTransparent,tileCol);
			}
			
			if(GUILayout.Button("Load",GUILayout.ExpandWidth(false))) {
				LoadMap("Map Name",map);
			}
			if(GUILayout.Button("Save "+filename,GUILayout.ExpandWidth(false))) {
				SaveMap(filename);
			}
		EditorGUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		for(int w = 0;w < 75;w++) {//Actual tile display area
			GUILayout.BeginVertical();
				for(int l = 0;l < 25;l++) {
					int wAdd = 75*dispX;
					int lAdd = 25*dispY;
					//s.normal.textColor = map[w + wAdd,l + lAdd,0].tColor;
					if(GUILayout.Button(map[w + wAdd,l + lAdd,0].tChar.ToString(),s,GUILayout.Width(13))) {
						map[sX,sY,sZ] = new Tile(sX,sY,sZ,tileCharacter.ToCharArray()[0],tileMovable,tileTransparent,tileCol);
						map[sX,sY,sZ].isDoor = tileIsDoor;
						map[sX,sY,sZ].isOpen = tileIsOpen;
						sX = w + wAdd;
						sY = l + lAdd;
						sZ = 0;
						selTile = "("+sX+","+sY+",0)";
						tileMovable = map[sX,sY,0].canMoveTo;
						tileCharacter = map[sX,sY,0].tChar.ToString();
						tileCol = map[sX,sY,0].tColor;
						tileTransparent = map[sX,sY,0].canSeeThrough;
						tileIsDoor = map[sX,sY,0].isDoor;
						tileIsOpen = map[sX,sY,0].isOpen;
					}
				}
			GUILayout.EndVertical();
		}
		GUILayout.EndHorizontal();
		
		//_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos,"Box");
		//	GUILayout.BeginHorizontal();
		//	for(int w = 0;w < x;w++) {//Actual tile display area. Needs a GUIStyle to accompany it. Also needs to be optimized to hell & back.
		//		GUILayout.BeginVertical();
		//		for(int l = 0;l < y;l++) {
		//			s.normal.textColor = map[w,l,0].tColor;
		//			if(GUILayout.Button(map[w,l,0].tChar.ToString(),s,GUILayout.Width(13))) {
		//				//(int x, int y, int z,char c = '.', bool canM = true, bool canS = true,Color32 tileC = new Color32())
		//				map[sX,sY,sZ] = new Tile(sX,sY,sZ,tileCharacter.ToCharArray()[0],tileMovable,tileTransparent,tileCol);
		//				map[sX,sY,sZ].isDoor = tileIsDoor;
		//				map[sX,sY,sZ].isOpen = tileIsOpen;
		//				sX = w;
		//				sY = l;
		//				sZ = 0;
		//				selTile = "("+w+","+l+",0)";
		//				tileMovable = map[w,l,0].canMoveTo;
		//				tileCharacter = map[w,l,0].tChar.ToString();
		//				tileCol = map[w,l,0].tColor;
		//				tileTransparent = map[w,l,0].canSeeThrough;
		//				tileIsDoor = map[w,l,0].isDoor;
		//				tileIsOpen = map[w,l,0].isOpen;
		//			}
		//		}
		//		GUILayout.EndVertical();
		//	}
		//	GUILayout.EndHorizontal();
		//EditorGUILayout.EndScrollView();
	}
	
	void Update() {
		
	}
	
	public void LoadMap (string fn, Tile[,,] _map) {
		filename = fn;
		List<string> lines = new List<string>();
		using(StreamReader mapReader = new StreamReader(@"Assets/Resources/Maps/"+fn+".txt")) {//Initial pass to generate list.
			while(mapReader.Peek() >= 0)
				lines.Add(mapReader.ReadLine());
			mapReader.Close();
		}
		lines.RemoveAt(0);
		int xLargest = 0,yLargest = 0,zLargest = 0;
		foreach(string line in lines) { //First pass to determine the size of map
			string coordinates = line.Split(';')[0];
			if(System.Int32.Parse(coordinates.Trim('(',')').Split(',')[0]) > xLargest)
				xLargest = System.Int32.Parse(coordinates.Trim('(',')').Split(',')[0]);
			if(System.Int32.Parse(coordinates.Trim('(',')').Split(',')[1]) > yLargest)
				yLargest = System.Int32.Parse(coordinates.Trim('(',')').Split(',')[1]);
			if(System.Int32.Parse(coordinates.Trim('(',')').Split(',')[2]) > zLargest)
				zLargest = System.Int32.Parse(coordinates.Trim('(',')').Split(',')[2]);
		}
		this.x = xLargest+1;
		this.y = yLargest+1;
		this.z = zLargest+1;
		_map = new Tile[xLargest+1,yLargest+1,zLargest+1];
		foreach(string line in lines) { //Second pass to determine attributes of each tile
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
			char _character = _char.ToCharArray()[1];
			bool _unblocked = (System.Int32.Parse(_moveable.ToCharArray()[1].ToString()) != 0);
			bool _transparent = (System.Int32.Parse(_seeThrough.ToCharArray()[1].ToString()) != 0);
			bool[] _isDoor = {(System.Int32.Parse(_door.ToCharArray()[1].ToString()) != 0),(System.Int32.Parse(_door.ToCharArray()[3].ToString()) != 0)};
			_map[_x,_y,_z] = new Tile(_x,_y,_z,_character,_unblocked,_transparent,_color);
			_map[_x,_y,_z].isDoor = _isDoor[0];
			_map[_x,_y,_z].isOpen = _isDoor[1];
		}
		map = _map;
	}

	public void SaveMap (string fn) {
		File.Delete(@"Assets/Resources/Maps/"+fn+".txt");
		string tW = null;
		foreach(Tile tile in map) {
			int _x = (int)tile.loc.x;
			int _y = (int)tile.loc.y;
			int _z = (int)tile.loc.z;
			int _r = (int)tile.tColor.r;
			int _g = (int)tile.tColor.g;
			int _b = (int)tile.tColor.b;
			char c = tile.tChar;
			int passable = System.Convert.ToInt32(tile.canMoveTo);
			int transparent = System.Convert.ToInt32(tile.canSeeThrough);
			int isDoor = System.Convert.ToInt32(tile.isDoor);
			int isOpen = System.Convert.ToInt32(tile.isOpen);
			string toWrite = "("+_x+","+_y+","+_z+");("+_r+","+_g+","+_b+");("+c+");";
			toWrite+="("+passable+");("+transparent+");("+isDoor+","+isOpen+")";
			tW += toWrite + "\n";
		}
		File.AppendAllText(@"Assets/Resources/Maps/"+fn+".txt",tW.TrimEnd('\n'));
	}
	public void SetRecents () {
		File.Delete(@"Assets/Editor/Config/mapeditor.txt");
		File.AppendAllText(@"Assets/Editor/Config/mapeditor.txt",filename+"\n");
		File.AppendAllText(@"Assets/Editor/Config/mapeditor.txt",dispX+","+dispY+"\n");
	}
	public void ReadRecents () {
		List<string> lines = new List<string>();
		using(StreamReader recentReader = new StreamReader(@"Assets/Editor/Config/mapeditor.txt")) {
			while(recentReader.Peek() >= 0)
				lines.Add(recentReader.ReadLine());
			recentReader.Close();
		}
		lines[0] = filename;
		dispX = System.Int32.Parse(lines[1].Split(',')[0]);
		dispY = System.Int32.Parse(lines[1].Split(',')[1]);
	}
}