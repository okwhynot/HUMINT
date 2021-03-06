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
	bool multSel = false;
	
	[MenuItem ("HUMINT/Map Editor")]
	static void Init () {
		//Find an open window if one exists.
		MapEditor editor = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor));
		editor.title = "Map Editor";
		editor.LoadMap("Map Name",editor.map);
		editor.ReadRecents();
		//editor.LoadMap(editor.filename,editor.map);
	}
	
	void OnFocus () {
		ReadRecents();
		LoadMap(filename, map);
	}
	
	void OnLostFocus () {
		SaveMap(filename);
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
			Color def = GUI.color;
			GUI.color = ColorLib.Black();
			GUILayout.Label(selTile);
			GUI.color = def;
			tileCharacter = GUILayout.TextArea(tileCharacter,1,GUILayout.Width(17));
			tileCol = EditorGUILayout.ColorField("",tileCol,GUILayout.Width(50));
			GUI.color = ColorLib.Black();
			GUILayout.Label("Movable");
			GUI.color = def;
			tileMovable = EditorGUILayout.Toggle("",tileMovable,GUILayout.Width(17));
			GUI.color = ColorLib.Black();
			GUILayout.Label("Transparent");
			GUI.color = def;
			tileTransparent = EditorGUILayout.Toggle("",tileTransparent,GUILayout.Width(17));
			GUI.color = ColorLib.Black();
			GUILayout.Label("Door");
			GUI.color = def;
			tileIsDoor = EditorGUILayout.Toggle("",tileIsDoor,GUILayout.Width(17));
			GUI.color = ColorLib.Black();
			GUILayout.Label("Open");
			GUI.color = def;
			tileIsOpen = EditorGUILayout.Toggle("",tileIsOpen,GUILayout.Width(17));
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("^",GUILayout.ExpandWidth(false)) && dispY > 0) {
				dispY-=1;
			}
			if(GUILayout.Button("<",GUILayout.ExpandWidth(false)) && dispX > 0) {
				dispX-=1;
			}
			GUI.color = ColorLib.Black();
			GUILayout.Label("("+dispX+","+dispY+","+dispZ+")");
			GUI.color = def;
			if(GUILayout.Button(">",GUILayout.ExpandWidth(false)) && dispX < 13) {
				dispX+=1;
			}
			if(GUILayout.Button("v",GUILayout.ExpandWidth(false)) && dispY < 13) {
				dispY+=1;
			}
			
			GUILayout.FlexibleSpace();
			GUI.color = ColorLib.Black();
			GUILayout.Label("Multi-select");
			GUI.color = def;
			multSel = EditorGUILayout.Toggle("",multSel,GUILayout.Width(17));
			if(GUILayout.Button("Apply",GUILayout.ExpandWidth(false))) {
				for(int i = 0; i < selectedTiles.Count - 1; i++) {
					int x = (int)selectedTiles[i].loc.x;
					int y = (int)selectedTiles[i].loc.y;
					int z = (int)selectedTiles[i].loc.z;
					map[x,y,z] = new Tile(x,y,z,tileCharacter[0],tileMovable,tileTransparent,tileCol);
					
				}
				selectedTiles.Clear();
				multSel = false;
			}
			if(GUILayout.Button("Clear",GUILayout.ExpandWidth(false))) {
				selectedTiles.Clear();
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
					//if(selectedTiles.Contains(map[w + wAdd,l + lAdd,0]) == false)
					//GUI.color = map[w + wAdd, l + lAdd, 0].tColor;
					//else
					//	GUI.color = ColorLib.Black();
					if(selectedTiles.Contains(map[w + wAdd,l + lAdd,0]) == false)
						s.normal.textColor = map[w + wAdd, l + lAdd, 0].tColor;
					else if(selectedTiles.Contains(map[w + wAdd,l + lAdd,0]) == true)
						s.normal.textColor = ColorLib.Black();
					if(GUILayout.Button(map[w + wAdd,l + lAdd,0].tChar.ToString(),s,GUILayout.Width(13))) {
						if(multSel == false) {
							map[sX,sY,sZ] = new Tile(sX,sY,sZ,tileCharacter[0],tileMovable,tileTransparent,tileCol);
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
						else {
							if(selectedTiles.Contains(map[w + wAdd,l + lAdd,0]) == false)
								selectedTiles.Add(map[w + wAdd,l + lAdd,0]);
							else
								selectedTiles.Remove(map[w + wAdd,l + lAdd,0]);
						}
					}
				}
			GUILayout.EndVertical();
		}
		GUILayout.EndHorizontal();
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