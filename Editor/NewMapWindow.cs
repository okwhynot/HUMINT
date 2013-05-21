using UnityEngine;
using UnityEditor;
using System.IO;

public class NewMapWindow : EditorWindow {
	
	int _x = 75,_y = 25,_z = 1;
	Color32 defCol;
	string mapName = "Map Name";
	string defChar = ".";
	string toWrite;
	static NewMapWindow win;
	bool creating = false;
	
	[MenuItem("HUMINT/New Map")]
	static void Init () {
		NewMapWindow window = new NewMapWindow();
		window.ShowUtility();
		window.title = "New Map";
		window.minSize = new Vector2(195,150);
		window.maxSize = new Vector2(195,150);
		win = window;
	}
	
	void OnGUI () {
		mapName = EditorGUILayout.TextField(mapName);
		GUILayout.BeginHorizontal("Box");
			GUILayout.Label("Dimensions:");
			GUILayout.FlexibleSpace();
			_x = EditorGUILayout.IntField("",_x,GUILayout.MaxWidth(30));
			_y = EditorGUILayout.IntField("",_y,GUILayout.MaxWidth(30));
			_z = EditorGUILayout.IntField("",_z,GUILayout.MaxWidth(30));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal("Box");
			GUILayout.Label("Default Color:");
			GUILayout.FlexibleSpace();
			defCol = EditorGUILayout.ColorField("",defCol,GUILayout.MaxWidth(100));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal("Box");
			GUILayout.Label("Default Character:");
			defChar = GUILayout.TextField(defChar,1,GUILayout.MaxWidth(20));
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Create")) {
			if(File.Exists(@"Assets/Resources/Maps/"+mapName+".txt"))
				File.Delete(@"Assets/Resources/Maps/"+mapName+".txt");
			CreateMap();
			MapEditor editor = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor));
			editor.title = "Map Editor";
			editor.filename = mapName;
			editor.x = _x;
			editor.y = _y;
			editor.z = _z;
			//editor.filename = mapName;
			editor.LoadMap(mapName,editor.map);
			Close();
		}
	}
	void CreateMap() {
		for(int x = 0; x < _x; x++) {
			for(int y = 0; y < _y; y++) {
				string toWrite = null;
				for(int z = 0; z < _z; z++) {
					string curLine = "("+x+","+y+","+z+");("+defCol.r+","+defCol.g+","+defCol.b+");("+defChar+");(1);(1);(0,0)"+"\n";
					toWrite+=curLine;
				}
				File.AppendAllText(@"Assets/Resources/Maps/"+mapName+".txt",toWrite);
			}
		}
	}
}