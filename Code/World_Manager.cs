using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using libhumint;

public class World_Manager : MonoBehaviour {
	
	NWorld world = new NWorld("Map Name");
	
	void Awake() {
		
	}
}

public class NWorld {
	public Tile[,,] map;
	public int w,l,h;
	
	public NWorld(string filename) {
		
	}
	
	void Load(string fn) {
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
		this.w = xLargest+1;
		this.l = yLargest+1;
		this.h = zLargest+1;
		map = new Tile[xLargest+1,yLargest+1,zLargest+1];
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
			map[_x,_y,_z] = new Tile(_x,_y,_z,_character,_unblocked,_transparent,_color);
			map[_x,_y,_z].isDoor = _isDoor[0];
			map[_x,_y,_z].isOpen = _isDoor[1];
		}
	}
}