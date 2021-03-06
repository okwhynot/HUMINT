using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using libhumint;
//Rewrite all of this. It kind of blows.
public class World_Manager : MonoBehaviour {
	public World world;
	public Color wallColor = ColorLib.Brown("natural");
	public Color floorColor = ColorLib.Brown();
	
	void Awake() {
		//world = new World();
		//world = new World(150,100,1,5, 10,true);
		int w = UnityEngine.Random.Range(5,20);
		int l = UnityEngine.Random.Range(5,15);
		//Debug.Log (w + "x" + l);
		//world = new World("s",w,l,1,10,6);
		//world.IndexContents();
		//world.UpdateContents();
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
	#region Variables
	public Tile[,,] map;
	public List<GameObject> Contents = new List<GameObject>();
	public List<Line.Point> Street = new List<Line.Point>();
	public Color wallColor = ColorLib.DarkGray("cga");
	public Color floorColor = ColorLib.LightGray("cga");
	#endregion
	#region Constructors
	public World(string filename = "Map Name") {
		ReadMap(filename);
	}
	/// <summary>
	/// Initializes a new randomly generated dungeon.
	/// </summary>
	public World(string s, int width, int length, int depth, int roomW = 10, int roomH = 6) {
		map = new Tile[1+width * roomW,1+length*roomH,depth];
		
		for(int x = 0; x < width*roomW; x++) {
			for(int y = 0; y < length*roomH; y++) {
				for(int z = 0; z < depth; z++) {
					map[x,y,z] = new Tile(x,y,z,' ',false,true,ColorLib.White());
				}
			}
		}
		
		GenerateDungeon(width, length, roomW, roomH);
		//GenerateCave();
	}
	/// <summary>
	/// Initializes a randomly generated world, the minimum size of which is 31x25x1.
	/// </summary>
	public World(int width, int length, int height, int streetWidth = 5,int buildingSize = 10,bool centerline = false, bool sidewalk = false, bool crosswalk = false) {
		#region Exceptions
		if(width <= 30)
			throw new InvalidOperationException("Width must be greater than 30.");
		if(length <= 24)
			throw new InvalidOperationException("Length must be greater than 24.");
		if(height < 1)
			throw new InvalidOperationException("Height must be greater than or equal to 1.");
		if(width <= 30 || length <= 24 || height < 1)
			return;
		#endregion
		#region Initialization
		this.map = new Tile[width, length, height];
		for(int x = 0; x < width; x++) {
			for(int y = 0; y < length; y++) {
				for(int z = 0; z < height; z++) {
					map[x,y,z] = new Tile(x,y,z,'.',true,true,ColorLib.White());
				}
			}
		}
		#endregion
		#region Generation
		GenerateWorld(streetWidth, buildingSize, centerline, sidewalk,crosswalk);
		
		#endregion
	}
	#endregion
	#region Cave Generation
	void GenerateCave() {
		map = new Tile[100,100,1];
		for(int x = 0; x < 100; x++) {
			for(int y = 0; y < 100; y++) {
				map[x,y,0] = new Tile(x,y,0,'.',true,true,ColorLib.LightGray("cga"));
			}
		}
		List<Line.Point> points = new List<Line.Point>();
		//Rect A's corners are !(20,10)!, (20,20), (80,10), and !(80,20)!
		for(int i = 0; i < UnityEngine.Random.Range(1,4); i++) {
			int x = UnityEngine.Random.Range(25,75);
			int y = UnityEngine.Random.Range(12,28);
			points.Add(new Line.Point(x,y,0));
		}
		//Rect B's corners are !(80,20)!, (90,20), (80,80), and !(90,80)!
		for(int i = 0; i < UnityEngine.Random.Range(1,4); i++) {
			int x = UnityEngine.Random.Range(81,90);
			int y = UnityEngine.Random.Range(25,75);
			points.Add(new Line.Point(x,y,0));
		}
		//Rect C's corners are !(20,80)!,(20,90),(90,80), and !(90,90)!
		for(int i = 0; i < UnityEngine.Random.Range(1,4); i++) {
			int x = UnityEngine.Random.Range(25,85);
			int y = UnityEngine.Random.Range(81,90);
			points.Add(new Line.Point(x,y,0));
		}
		//Rect D's corners are !(10,20)!, (20,20), (10,80), and !(20,80)!
		for(int i = 0; i < UnityEngine.Random.Range(1,4); i++) {
			int x = UnityEngine.Random.Range(11,20);
			int y = UnityEngine.Random.Range(25,75);
			points.Add(new Line.Point(x,y,0));
		}
		List<Line> edges = new List<Line>();
		for(int i = 0; i < points.Count; i++) {
			Line l = new Line(0,0,0,0);
			if(i != points.Count - 1) {
				l = new Line(points[i].x,points[i].y,points[i+1].x,points[i+1].y);
			}
			else {
				l = new Line(points[i].x,points[i].y,points[0].x,points[0].y);
			}
			edges.Add(l);
		}
		foreach(Line l in edges) {
			foreach(Line.Point p in l.getPoints()) {
				map[p.x,p.y,p.z] = new Tile(p.x,p.y,p.z,(char)219,false,false,ColorLib.DarkGray("cga"));
			}
		}
	}
	#endregion
	#region Dungeon Generation
	/// <summary>
	/// Generates the dungeon.
	/// </summary>
	public void GenerateDungeon(int xBound, int yBound, int roomW, int roomH) {
		#region Initialize
		List<Line.Point> potentialDoors = new List<Line.Point>();
		Room[,] grid = new Room[xBound,yBound];
		for(int o = 0; o < xBound; o++) {
			for(int q = 0; q < yBound; q++) {
				grid[o,q] = new Room(o, q, false, new Vector2[0]);
			}
		}
		#endregion
		#region First room
		int x = UnityEngine.Random.Range(0,xBound);
		int y = UnityEngine.Random.Range(0,yBound);
		Vector2 first = new Vector2(x, y);
		int a = (x > 0 && x < xBound - 1) ? 2 : 1;
		int b = (y > 0 && y < yBound - 1) ? 2 : 1;
		
		List<Vector2> adjacent = new List<Vector2>();
		
		int[] xAdj = (x > 0 && x < xBound - 1) ? new int[2] : new int[1];
		if(xAdj.Length == 1 && x == xBound - 1)
			xAdj[0] = x - 1;
		else if(xAdj.Length == 1 && x == 0)
			xAdj[0] = x + 1;
		
		if(xAdj.Length == 2)
			xAdj = new int[2] {x - 1, x + 1};
	
		foreach(int o in xAdj) {
			adjacent.Add(new Vector2(o,y));
		}			
		int[] yAdj = (y > 0 && y < yBound - 1) ? new int[2] : new int[1];
		if(yAdj.Length == 1 && y == yBound - 1)
			yAdj[0] = y - 1;
		else if(yAdj.Length == 1 && y == 0)
			yAdj[0] = y + 1;
		
		if(yAdj.Length == 2) {
			yAdj = new int[2] {y - 1, y + 1};
		}
		
		foreach(int o in yAdj) {
			adjacent.Add(new Vector2(x,o));
		}
		
		grid[x,y] = new Room(x, y, true, adjacent.ToArray());
		#endregion
		#region Set player location
		GameObject p = GameObject.FindWithTag("Player");	
		float z = p.GetComponent<Object>().Coordinates.z;
		p.GetComponent<Object>().Coordinates = new Vector3(grid[x,y].center.x,grid[x,y].center.y,z);
		#endregion
		#region Connected to initial
		foreach(Vector2 v2 in grid[x,y].connectedTo) {
			adjacent.Clear();
			xAdj = ((int)v2.x > 0 && (int)v2.x < xBound - 1) ? new int[2] : new int[1];
			yAdj = (y > 0 && y < yBound - 1) ? new int[2] : new int[1];
			
			if(xAdj.Length == 1 && x == xBound - 1)
				xAdj[0] = x - 1;
			else if(xAdj.Length == 1 && x == 0)
				xAdj[0] = x + 1;
			if(xAdj.Length == 2)
				xAdj = new int[2] {x - 1, x + 1};
		
			foreach(int o in xAdj)
				adjacent.Add(new Vector2(o,y));
			
			if(yAdj.Length == 1 && y == yBound - 1)
				yAdj[0] = y - 1;
			else if(yAdj.Length == 1 && y == 0)
				yAdj[0] = y + 1;
			if(yAdj.Length == 2)
				yAdj = new int[2] {y - 1, y + 1};
			
			foreach(int o in yAdj)
				adjacent.Add(new Vector2(x,o));
				
			grid[(int)v2.x,(int)v2.y] = new Room((int)v2.x,(int)v2.y,true,adjacent.ToArray());
			grid[(int)v2.x,(int)v2.y].gone = (UnityEngine.Random.Range(0,2) == 1);
		}
		#endregion
		#region While unconnected still exists
		for(x = 0; x < xBound; x++) {
			for(y = 0; y < yBound; y++) {
				if(grid[x,y].isConnected == false || grid[x,y].isConnected == false) {
					adjacent.Clear();
					xAdj = (x > 0 && x < xBound - 1) ? new int[2] : new int[1];
					yAdj = (y > 0 && y < yBound - 1) ? new int[2] : new int[1];
					
					if(xAdj.Length == 1 && x == xBound - 1)
						xAdj[0] = x - 1;
					else if(xAdj.Length == 1 && x == 0)
						xAdj[0] = x + 1;
					if(xAdj.Length == 2 && x != xBound - 1 && x != 0)
						xAdj = new int[2] {x - 1, x + 1};
				
					foreach(int o in xAdj)
						adjacent.Add(new Vector2(o,y));
					
					if(yAdj.Length == 1 && y == yBound - 1)
						yAdj[0] = y - 1;
					else if(yAdj.Length == 1 && y == 0)
						yAdj[0] = y + 1;
					if(yAdj.Length == 2 && y != yBound - 1 && y != 0)
						yAdj = new int[2] {y - 1, y + 1};
					
					foreach(int o in yAdj)
						adjacent.Add(new Vector2(x,o));
						
					grid[x,y] = new Room(x,y,true,adjacent.ToArray());
					grid[x,y].gone = (UnityEngine.Random.Range(0,2) == 1);
				}
			}
		}
		#endregion
		
		#region Draw rooms
		foreach(Room r in grid) {
			if(r.gone == true) {
				for(x = (int)r.center.x - 1; x <= (int)r.center.x + 1; x++) {
					for(y = (int)r.center.y - 1; y <= (int)r.center.y + 1; y++) {
						map[x,y,(int)z] = new Tile(x,y,(int)z,(char)219,false,false,wallColor);
					}
				}
				continue;
			}
			
			for(x = roomW * r.x + r.xOff; x <= roomW * r.x + r.xOff + roomW; x++) {
				for(y = roomH * r.y; y <= roomH * r.y + roomH; y++) {
					if(x == roomW * r.x + r.xOff || x == roomW * r.x + r.xOff + roomW) {
						map[x,y,(int)z] = new Tile(x,y,(int)z,(char)219,false,false,wallColor);
					}
					else {
						map[x,y,(int)z] = new Tile(x,y,(int)z,'.',true,true,floorColor);
					}
					if(y == roomH * r.y || y == roomH * r.y + roomH) {
						map[x,y,(int)z] = new Tile(x,y,(int)z,(char)219,false,false,wallColor);
					}
				}
			}
		}
		#endregion
		
		#region Draw corridors
		//return;
		foreach(Room r in grid) {
			if(r.connectedTo != null && r.connectedTo.Count() > 0) {
				bool skipped = false;
				foreach(Vector2 v in r.connectedTo) {
					if(skipped == false) {
						if(UnityEngine.Random.Range(0,2) == 0) {
							skipped = true;
							continue;
						}
					}
					if((int)v.x >= grid.GetLength(0) || (int)v.y >= grid.GetLength(1) || v.y < 0 || v.x < 0) {
						Debug.Log("Adjacent room out of range, origin is "+r.x+","+r.y+".");
						continue;
					}
					int gY = (int)grid[(int)v.x,(int)v.y].center.y;
					int gX = (int)grid[(int)v.x,(int)v.y].center.x;
					int rise = Math.Abs(gY - (int)r.center.y);
					int run = Math.Abs(gX - (int)r.center.x);
					if(rise != 0 && run != 0 && rise == run) {
						Debug.Log(rise / run);
						continue;
					}
					Line cor = new Line((int)r.center.x, (int)r.center.y, (int)grid[(int)v.x,(int)v.y].center.x, (int)grid[(int)v.x,(int)v.y].center.y);
					foreach(Line.Point point in cor.getPoints()) {
						if(run == 0) {
							if(map[point.x - 1, point.y,(int)z].tDefault != '.')
								map[point.x - 1, point.y,(int)z] = new Tile(point.x - 1, point.y, (int)z,(char)219,false,false,wallColor);
							
							if(map[point.x,point.y,(int)z].tDefault == (char)219)
								potentialDoors.Add(point);
								
							map[point.x,point.y,(int)z] = new Tile(point.x,point.y,(int)z,'.',true,true,floorColor);
							
							if(map[point.x + 1, point.y,(int)z].tDefault != '.')
								map[point.x + 1, point.y,(int)z] = new Tile(point.x + 1, point.y, (int)z,(char)219,false,false,wallColor);
						}
						else if(rise == 0) {
							if(map[point.x, point.y - 1,(int)z].tDefault != '.' && map[point.x, point.y - 1,(int)z].tDefault != (char)186)
								map[point.x, point.y - 1, (int)z] = new Tile(point.x, point.y - 1, (int)z, (char)219,false,false,wallColor);
							
							if(map[point.x,point.y,(int)z].tDefault == (char)219)
								potentialDoors.Add(point);
							
							map[point.x,point.y,(int)z] = new Tile(point.x,point.y,(int)z,'.',true,true,floorColor);
							
							if(map[point.x, point.y + 1,(int)z].tDefault != '.' && map[point.x, point.y + 1,(int)z].tDefault != (char)186)
								map[point.x, point.y + 1, (int)z] = new Tile(point.x, point.y + 1, (int)z, (char)219,false,false,wallColor);
						}
					}
				}
			}
		}
		#endregion
		#region Draw doors -- NOT FINISHED
		foreach(Line.Point point in potentialDoors) {
			//map[point.x, point.y, (int)z].tDefault = 'P';
			if(map[point.x - 1, point.y, (int)z].canMoveTo == true && map[point.x + 1, point.y, (int)z].canMoveTo == true
			&& map[point.x, point.y - 1, (int)z].canMoveTo == false && map[point.x, point.y + 1, (int)z].canMoveTo == false
			&& map[point.x + 1, point.y + 1, (int)z].canMoveTo == true && map[point.x + 1, point.y - 1, (int)z].canMoveTo == true
			&& map[point.x - 1, point.y - 1, (int)z].canMoveTo == true && map[point.x - 1, point.y + 1, (int)z].canMoveTo == true ||
			map[point.x, point.y - 1, (int)z].canMoveTo == true && map[point.x, point.y + 1, (int)z].canMoveTo == true
			&& map[point.x - 1,point.y,(int)z].canMoveTo == false && map[point.x + 1, point.y,(int)z].canMoveTo == false
			&& map[point.x - 1, point.y - 1, (int)z].canMoveTo == true && map[point.x + 1, point.y + 1, (int)z].canMoveTo == true
			&& map[point.x + 1, point.y + 1, (int)z].canMoveTo == true && map[point.x + 1, point.y - 1, (int)z].canMoveTo == true) {
				map[point.x,point.y,(int)z].tDefault = (char)219;
				map[point.x,point.y,(int)z].tCDefault = ColorLib.LightGray("cga");
				map[point.x,point.y,(int)z].tDoorTile = (char)219;
				map[point.x,point.y,(int)z].isDoor = true;
				map[point.x,point.y,(int)z].isOpen = false;
				map[point.x,point.y,(int)z].canSeeThrough = false;
			}
		}
		#endregion
		#region Decorate -- UNFINISHED
		DecorateDungeon(grid, roomW, roomH);
		PopulateDungeon(grid, roomW, roomH);
		PopulateItems(grid, roomW, roomH);
		#endregion
	}
	
	void DecorateDungeon(Room[,] grid, int roomW, int roomH) {
		#region Variables
		GameObject p = GameObject.FindWithTag("Player");	
		int z = (int)p.GetComponent<Object>().Coordinates.z;
		#endregion
		#region Combine Rooms
		for(int x = 0; x < grid.GetLength(0) - 1; x++) {
			for(int y = 0; y < grid.GetLength(1) - 1; y++) {
				if(grid[x,y].gone == false && grid[x + 1, y].gone == false) {
					bool go = (UnityEngine.Random.Range(0,4) == 0);
					if(go) {
						int x0 = (int)grid[x,y].center.x;
						int y0 = (int)grid[x,y].center.y;
						int x1 = (int)grid[x + 1, y].center.x;
						int y1 = (int)grid[x + 1, y].center.y;
						Line l = new Line(x0,y0,x1,y1);
						grid[x,y].combined = true;
						grid[x + 1,y].combined = true;
						foreach(Line.Point point in l.getPoints()) {
							if(map[point.x,point.y,point.z].isDoor) {
								Line wall = new Line(point.x,point.y - (roomH - 1)/2,point.x,point.y + (roomH - 1)/2);
								foreach(Line.Point poi in wall.getPoints()) {
									map[poi.x,poi.y,poi.z] = new Tile(poi.x, poi.y, poi.z, '.', true, true, floorColor);
								}
								break;
							}
						}
						continue;
					}
				}
				if(grid[x,y].gone == false && grid[x,y + 1].gone == false) {
					bool go = (UnityEngine.Random.Range(0,4) == 0);
					if(go) {
						int x0 = (int)grid[x,y].center.x;
						int y0 = (int)grid[x,y].center.y;
						int x1 = (int)grid[x,y + 1].center.x;
						int y1 = (int)grid[x,y + 1].center.y;
						Line l = new Line(x0,y0,x1,y1);
						grid[x,y].combined = true;
						grid[x,y + 1].combined = true;
						foreach(Line.Point point in l.getPoints()) {
							if(map[point.x, point.y, point.z].isDoor) {
								Line wall = new Line(point.x - (roomW - 1)/2,point.y, point.x + (roomW - 1)/2, point.y);
								foreach(Line.Point poi in wall.getPoints()) {
									map[poi.x,poi.y,z] = new Tile(poi.x, poi.y, poi.z, '.', true, true, floorColor);
								}
								break;
							}
						}
						continue;
					}
				}
			}
		}
		#endregion
		#region Decorate rooms
		bool upStairsPlaced = false;
		bool downStairsPlaced = false;
		bool armoryPlaced = false;
		bool commandPlaced = false;
		bool barracksPlaced = false;
		bool radioRoomPlaced = false;
		bool infirmaryPlaced = false;
		bool labPlaced = false;
		bool officeLimit = false;
		bool prisonPlaced = false;
		bool gymPlaced = false;
		bool messPlaced = false;
		foreach(Room r in grid) {
			if(r.gone)
				continue;
			start:
			int room = UnityEngine.Random.Range(0,5);
			if(r.combined) {
				#region Barracks
				
				#endregion
				#region Command Center
				
				#endregion
				#region Lab
				
				#endregion
			}
			else {
				#region Armory
				if(room == 0) {
					if(armoryPlaced)
						goto start;
				
					int x = (int)r.center.x + 1 - (roomW - 1) / 2;
					int x1 = (int)r.center.x + (roomW - 1) / 2;
					int y = (int)r.center.y + 1 - (roomH - 1) / 2;
					int y1 = (int)r.center.y - 1 + (roomH - 1) / 2;
					int ind = 0;
					GameObject armoryParent = new GameObject("Armory Parent");
					GameObject.DontDestroyOnLoad(armoryParent);
					for(x = (int)r.center.x - (roomW - 1) / 2; x <= x1; x++) {
						for(y = (int)r.center.y + 1 - (roomH - 1) / 2; y <= y1; y++) {
							if(ind % 2 == 1) {
								//map[x,y,z] = new Tile(x,y,0,(char)14,false,true,ColorLib.DarkGray("cga"));
								GameObject q = GameObject.Instantiate(Resources.Load("Object")) as GameObject;
								q.name = "Gun Rack";
								q.GetComponent<Object>().Coordinates = new Vector3(x,y,z);
								q.transform.parent = armoryParent.transform;
								q.GetComponent<Object>().name = "Gun Rack";
								q.GetComponent<Object>().item.canPickUp = false;
								q.GetComponent<Object>().item.doesStack = false;
								q.GetComponent<Object>().doesBlockMovement = true;
								q.GetComponent<Object>().display += (char)14;
								map[x,y,z].tileContents.Add(q);
								GameObject.DontDestroyOnLoad(q);
							}
						}
						ind++;
					}
					#region Armory Doors
					y = (int)r.center.y;
					ind = 0;
					for(x = (int)r.center.x - 1 - (roomW - 1) / 2; x <= x1 + 1; x++) {
						if(map[x,y,z].isDoor) {
							map[x,y,z] = new Tile(x,y,z,(char)219,true,false,ColorLib.LightGray("cga"));
							map[x,y,z].isDoor = true;
							map[x,y,z].isOpen = false;
						}
						else if(map[x,y,z].isDoor == false && map[x,y,0].tDefault == '.' && (ind == 0 || ind == roomW)) {
							map[x,y,z] = new Tile(x,y,z,(char)219,true,false,ColorLib.LightGray("cga"));
							map[x,y,z].isDoor = true;
							map[x,y,z].isOpen = false;
						}
						ind++;
					}
					x = (int)r.center.x;
					ind = 0;
					for(y = (int)r.center.y - 1 - (roomH - 1) / 2; y <= y1 + 2; y++) {
						if(map[x,y,z].isDoor) {
							map[x,y,z] = new Tile(x,y,z,(char)219,true,false,ColorLib.LightGray("cga"));
							map[x,y,z].isDoor = true;
							map[x,y,z].isOpen = false;
						}
						else if(map[x,y,z].isDoor == false && map[x,y,z].tDefault == '.' && (ind == 0 || ind == roomH)) {
							map[x,y,z] = new Tile(x,y,z,(char)219,true,false,ColorLib.LightGray("cga"));
							map[x,y,z].isDoor = true;
							map[x,y,z].isOpen = false;
						}
						ind++;
					}
					#endregion
					armoryPlaced = true;
					Debug.Log (r.x + " , " + r.y);
				}
				#endregion
				#region Radio Room
				if(room == 1) {
					if(radioRoomPlaced)
						goto start;
					
					int x = (int)r.center.x - (roomW - 1) / 2;
					int y = (int)r.center.y - (roomH - 1) / 2;
					int x1 = (int)r.center.x + (roomW - 1) / 2;
					int y1 = (int)r.center.y + (roomH - 1) / 2;
					#region Radio consoles
					for(x = (int)r.center.x - (roomW - 1) / 2; x <= x1; x++) {
						for(y = (int)r.center.y - (roomH - 1) / 2; y <= y1; y++) {
							if(map[x,y+1,0].canMoveTo == false && map[x,y+1,0].tDefault != '8' && map[x,y+1,0].tDefault != '!' || map[x,y-1,0].canMoveTo == false && map[x,y-1,0].tDefault != '8' && map[x,y-1,0].tDefault != '!') {
								map[x,y,0] = new Tile(x,y,0,'8',false,false,ColorLib.Brown("cga"));
							}
						}
					}
					#endregion
					#region Chairs
					for(x = (int)r.center.x - (roomW - 1) / 2; x <= x1; x++) {
						for(y = (int)r.center.y - (roomH - 1) / 2; y <= y1; y++) {
							if(map[x,y+1,z].tDefault == '8' && map[x,y+1,0].canMoveTo == false|| map[x,y-1,z].tDefault == '8' && map[x,y-1,0].canMoveTo == false) {
								map[x,y,z] = new Tile(x,y,z,'o',true,true,ColorLib.LightGray("cga"));
							}
						}
					}
					#endregion
					#region Antenna
					map[(int)r.center.x,(int)r.center.y,0] = new Tile((int)r.center.x,(int)r.center.y,z,'|',false,true,ColorLib.LightGray("cga"));
					#endregion
					radioRoomPlaced = true;	
					//Debug.Log(r.center.x + "," + r.center.y);
				}
				#endregion
				#region Storage
				if(room == 1) {
					
				}
				#endregion
				#region Office
				if(room == 2) {
					
				}
				#endregion
				#region Entrance -- IMPORTANT!
				if(room == 3) {
					if(upStairsPlaced)
						goto start;
					
					int x = (int)r.center.x;
					int y = (int)r.center.y;
					map[x,y,z] = new Tile(x,y,z,'<',true,true,ColorLib.White());
					Vector3 loc = new Vector3(x,y,z);
					p.GetComponent<Object>().Coordinates = loc;
					char[] wall = new char[] {(char)201,(char)186,(char)200,(char)205,(char)240,(char)187,(char)187,(char)186,(char)188};
					int xind = 0;
					for(x = (int)r.center.x - 1; x <= (int)r.center.x + 1; x++) {
						int yind = 0;
						for(y = (int)r.center.y - 1; y <= (int)r.center.y + 1; y++) {
							if(x == (int)r.center.x && y == (int)r.center.y)
								continue;
							map[x,y,z] = new Tile(x,y,z,wall[xind * 3 + yind],false,true,ColorLib.LightGray("cga"));
								
							yind++;
						}
						xind++;
					}
					#region Elevator Doors
					
					y = (int)r.center.y;
					int ind = 0;
					int x1 = (int)r.center.x + 1 + (roomW - 1) / 2;
					int y1 = (int)r.center.y + 1 + (roomH - 1) / 2;
					for(x = (int)r.center.x - 1 - (roomW - 1) / 2; x <= x1 + 1; x++) {
						if(map[x,y,z].isDoor && y != (int)r.center.y + 1) {
							map[x,y,z] = new Tile(x,y,z,(char)219,true,false,ColorLib.LightGray("cga"));
							map[x,y,z].isDoor = true;
							map[x,y,z].isOpen = false;
						}
						else if(map[x,y,z].isDoor == false && map[x,y,0].tDefault == '.' && (ind == 0 || ind == roomW)) {
							map[x,y,z] = new Tile(x,y,z,(char)219,true,false,ColorLib.LightGray("cga"));
							map[x,y,z].isDoor = true;
							map[x,y,z].isOpen = false;
						}
						ind++;
					}
					x = (int)r.center.x;
					ind = 0;
					for(y = (int)r.center.y - 1 - (roomH - 1) / 2; y <= y1 + 2; y++) {
						if(x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1))
							continue;
						if(map[x,y,z].isDoor) {
							map[x,y,z] = new Tile(x,y,z,(char)219,true,false,ColorLib.LightGray("cga"));
							map[x,y,z].isDoor = true;
							map[x,y,z].isOpen = false;
						}
						else if(map[x,y,z].isDoor == false && map[x,y,z].tDefault == '.' && (ind == 0 || ind == roomH)) {
							map[x,y,z] = new Tile(x,y,z,(char)219,true,false,ColorLib.LightGray("cga"));
							map[x,y,z].isDoor = true;
							map[x,y,z].isOpen = false;
						}
						ind++;
					}
					
					#endregion
					map[(int)r.center.x,(int)r.center.y + 1, z].isDoor = true;
					map[(int)r.center.x,(int)r.center.y + 1, z].isOpen = false;
					upStairsPlaced = true;
					Debug.Log(r.x + "," + r.y + " is entrance.");
				}
				#endregion
				#region Exit
				if(room == 4) {
					if(downStairsPlaced)
						goto start;
					int x = (int)r.center.x;
					int y = (int)r.center.y;
					map[x,y,z] = new Tile(x,y,z,'>',true,true,ColorLib.White());
					
					Debug.Log(r.x + "," + r.y + " is exit.");
					downStairsPlaced = true;
				}
				#endregion
			}
		}
		#endregion
	}
	
	void PlaceWall(int x, int y, int z, Color col, bool sight) {
		int[] AdaptableCharacters = new int[] {207,206,205,204,203,202,201,188,187,186,185};
		Line.Point[] adjacent = new Line.Point[] {
			new Line.Point(x-1,y,z),	//0
			new Line.Point(x+1,y,z),	//1
			new Line.Point(x,y-1,z),	//2
			new Line.Point(x,y+1,z),	//3
			new Line.Point(x-1,y-1,z),	//4
			new Line.Point(x-1,y+1,z),	//5
			new Line.Point(x+1,y-1,z),	//6
			new Line.Point(x+1,y+1,z)	//7
		};
		bool[] adjWal = new bool[8];
		foreach(Line.Point point in adjacent) {
			if(AdaptableCharacters.Contains((int)map[point.x,point.y,point.z].tDefault)) {
				adjWal[Array.IndexOf(adjacent, point)] = true;
			}
		}
		//Horizontal
		if((adjWal[0] || adjWal[1]) && adjWal[2] == false && adjWal[3] == false) {
			map[x,y,z] = new Tile(x,y,z,(char)205,false,sight,col);
		}
		//Vertical
		else if((adjWal[2] || adjWal[3]) && adjWal[0] == false && adjWal[1] == false) {
			map[x,y,z] = new Tile(x,y,z,(char)186,false,sight,col);
			if(adjWal[4] == false && adjWal[6] == false) {
				
			}
		}
		else
			map[x,y,z] = new Tile(x,y,z,(char)204,false,sight,col);
	}
	
	void PopulateDungeon(Room[,] grid, int roomW, int roomH) {
		GameObject npcParent = new GameObject("NPC Parent");
		GameObject.DontDestroyOnLoad(npcParent);
		foreach(Room r in grid) {
			if(r.gone == true)
				continue;
			int numberOfNPCs = UnityEngine.Random.Range(0,3);
			for(int i = 0; i < numberOfNPCs; i++) {
				string name = GameObject.Find("Logic Manager").GetComponent<Menu_Manager>().gNG.Name(0);
				GameObject enemy = GameObject.Instantiate(Resources.Load("NPC")) as GameObject;
				enemy.name = name;
				enemy.GetComponent<Object>().Coordinates = new Vector3(r.center.x - (UnityEngine.Random.Range(0,(roomW - 1) / 2)),r.center.y,0);
				enemy.GetComponent<Object>().name = name;
				enemy.transform.parent = npcParent.transform;
				char disp = (char)1;
				enemy.GetComponent<Object>().display = disp.ToString();
				GameObject.DontDestroyOnLoad(enemy);
			}
		}
	}
	
	void PopulateItems(Room[,] grid, int roomW, int roomH) {
		foreach(Transform child in GameObject.Find("Armory Parent").transform) {
			Object o = child.gameObject.GetComponent<Object>();
			float x = o.Coordinates.x + 1;
			float y = o.Coordinates.y;
			GameObject q = GameObject.Instantiate(GameObject.Find("9x18mm Makarov Magazine (G)")) as GameObject;
			MonoBehaviour.DontDestroyOnLoad(q);
			q.tag = "Item";
			Object r = q.GetComponent<Object>();
			q.name = r.name;
			r.item.capacity = 10;
			r.Coordinates = new Vector3(x,y,0);
		}
	}
	#endregion
	#region Overly ambitious world generator.
	/// <summary>
	/// Generate the world.
	/// </summary>
	void GenerateWorld(int streetWidth = 5, int buildingSize = 10, bool centerLine = false, bool sidewalk = false, bool crosswalk = false) {
		List<Block> blocks = new List<Block>();
		//During generation, places are painted colors.
		//Streets are red, sidewalks are cyan, centerlines are yellow, intersections are green., 
		#region Grid
		#region Variables
		Color _streetCol = ColorLib.Red();
		Color _divCol = ColorLib.Yellow();
		Color _walkCol = ColorLib.Cyan();
		Color _interCol = ColorLib.Green();
		int _width = map.GetLength(0);
		int _length = map.GetLength(1);
		int _height = map.GetLength(2);
		int _streetwidth = streetWidth;
		int _first = 0;
		int _y0 = 0;
		int _y1 = 0;
		int _x0 = 0;
		int _x1 = 0;
		#endregion
		#region Streets
		//Initial street's coordinates.
		_first = UnityEngine.Random.Range (5, 10);
		_y0 = _first;
		_y1 = _y0 + _streetwidth;
		_x1 = _width;
		//Generate our first street.
		for(int x = _x0; x < _x1; x++) {
			for(int y = _y0; y < _y1; y++) {
				map[x,y,0].tColor = _streetCol;
				map[x,y,0].tCDefault = _streetCol;
				Street.Add(new Line.Point(x,y,0));
			}
		}
		#region Horizontal
		_y0 = _y1 + 10;
		_y1 = _y0 + _streetwidth;
		_x1 = UnityEngine.Random.Range((int)_width/2, _width);
		while(_y1 < _length) {
			for(int x = _x0; x < _x1; x++) {
				for(int y = _y0; y < _y1; y++) {
					map[x,y,0].tColor = _streetCol;
					map[x,y,0].tCDefault = _streetCol;
					Street.Add(new Line.Point(x,y,0));
				}
			}
			_y0 = _y1 + 10;
			_y1 = _y0 + _streetwidth;
		}
		#endregion
		#region Vertical
		_x0 = 5;
		_x1 = _x0 + _streetwidth;
		_y0 = 0;
		_y1 = _length;
		while(_x1 < _width) {
			for(int x = _x0; x < _x1; x++) {
				for(int y = _y0; y < _y1; y++) {
					map[x,y,0].tColor = _streetCol;
					map[x,y,0].tCDefault = _streetCol;
					Street.Add(new Line.Point(x,y,0));
				}
			}
			_x0 = _x1 + 20;
			_x1 = _x0 + _streetwidth;
		}
		#endregion
		#endregion
		//Next, if desired, center lines are generated.
		#region Center Lines
		if(centerLine) {
			#region Horizontal
			_y0 = _first;
			_y1 = _y0 + _streetwidth;
			_x0 = 0;
			_x1 = _width;
			while(_y1 < _length) {
				int y = _y0 + _streetwidth/2;
				for(int x = _x0; x < _x1; x++) {
					map[x,y,0].tColor = _divCol;
					map[x,y,0].tCDefault = _divCol;
				}
				_y0 = _y1 + 10;
				_y1 = _y0 + _streetwidth;
			}
			#endregion
			#region Vertical
			_x0 = 5;
			_x1 = _x0 + _streetwidth;
			_y0 = 0;
			_y1 = _length;
			while(_x1 < _width) {
				int x = _x0 + _streetwidth/2;
				for(int y = _y0; y < _y1; y++) {
					if(map[x,y,0].tColor != _divCol) {
						map[x,y,0].tColor = _divCol;
						map[x,y,0].tCDefault = _divCol;
					}
					else {
						map[x,y,0].tColor = _interCol;
						map[x,y,0].tCDefault = _interCol;
					}
				}
				_x0 = _x1 + 20;
				_x1 = _x0 + _streetwidth;
			}
			#endregion
		}
		#endregion
		//Then, if desired, sidewalks.
		#region Sidewalk
		if(sidewalk) {
			#region Horizontal
			_y0 = _first;
			_y1 = _y0 + _streetwidth;
			_x0 = 0;
			_x1 = _width;
			while(_y1 < _length) {
				int _ya = _y0 - 1;
				int _yb = _y1;
				for(int x = _x0; x < _x1; x++) {
					if(map[x,_ya,0].tColor != _streetCol && map[x,_ya,0].tColor != _divCol) {
						map[x,_ya,0].tColor = _walkCol;
						map[x,_ya,0].tCDefault = _walkCol;
					}
					if(map[x,_yb,0].tColor != _streetCol && map[x,_yb,0].tColor != _divCol) {
						map[x,_yb,0].tColor = _walkCol;
						map[x,_yb,0].tCDefault = _walkCol;
					}
				}
				_y0 = _y1 + 10;
				_y1 = _y0 + _streetwidth;
			}
			#endregion
			#region Vertical
			_x0 = 5;
			_x1 = _x0 + _streetwidth;
			_y0 = 0;
			_y1 = _length;
			while(_x1 < _width) {
				int _xa = _x0 - 1;
				int _xb = _x1;
				for(int y = _y0; y < _y1; y++) {
					if(map[_xa,y,0].tColor != ColorLib.Red() && map[_xa,y,0].tColor != ColorLib.Yellow()) {
						map[_xa,y,0].tColor = ColorLib.Cyan();
						map[_xa,y,0].tCDefault = ColorLib.Cyan();
					}
					if(map[_xb,y,0].tColor != ColorLib.Red() && map[_xb,y,0].tColor != ColorLib.Yellow()) {
						map[_xb,y,0].tColor = ColorLib.Cyan();
						map[_xb,y,0].tCDefault = ColorLib.Cyan();
					}
				}
				_x0 = _x1 + 20;
				_x1 = _x0 + _streetwidth;
			}
			#endregion
		}
		#endregion
		#region Crosswalk
		if(crosswalk) {
			
		}
		#endregion
		#endregion
		//After the street is generated, blocks must be defined.
		#region Blocks
		Line _horizontal = new Line(0, 0, _width - 1, 0);
		Line _vertical = new Line(0, 0, 0, _length - 1);
		int xavoid = 4;
		int yavoid = _first - 1;
		List<int> _xs = new List<int>();
		List<int> _xe = new List<int>();
		List<int> _ys = new List<int>();
		List<int> _ye = new List<int>();
		#region Horizontal
		if(sidewalk) {
			xavoid = 3;
			yavoid = _first - 2;
		}
		#endregion
		#endregion
		#region Buildings
		#region Variables
		int _size = buildingSize;
		_x0 = -1;
		_y0 = -1;
		_x1 = -1;
		_y1 = -1;
		#endregion
		#region Test Buildng
		for(_x0 = 0; _x0 < _width - 1; _x0++) {
			for(_y0 = 0; _y0 < _length - 1; _y0++) {
				#region Variables
				Color tc0 = map[_x0,_y0,0].tColor;
				Color tc1 = new Color(); // x - 1, y, 0
				Color tc2 = new Color(); // x + 1, y, 0
				Color tc3 = new Color(); // x - 1, y - 1, 0
				Color tc4 = new Color(); // x, y - 1, 0
				Color tc5 = new Color(); // x, y + 1, 0
				Color tc6 = new Color(); // x + 1, y + 1, 0
				Color tc7 = new Color(); // x + 1, y - 1, 0
				Color tc8 = new Color(); // x - 1, y + 1, 0
				#endregion
				#region Color Assignment
				
				#endregion
			}
		}
		Debug.Log(_x0 +","+_y0);
		map[_x0,_y0,0].tColor = ColorLib.LightBlue("cga");
		#endregion
		#endregion
	}
	#endregion
	#region Maintenance
	public void IndexContents() {//Initially indexes all NPCs and items to add to the map.
		//string[] tagsToExclude = {"Generic Item","Manager","MainCamera","Player"};
		//object [] allObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));
		//foreach(object thisObject in allObjects) {
		//	GameObject processed = (GameObject)thisObject;
		//	if(processed.activeInHierarchy && tagsToExclude.Contains(processed.tag) == false) {
		//		Contents.Add(processed);
		//	}
		//}
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("NPC")) {
			Contents.Add(obj);
		}
	}
	
	public void UpdateContents() {	
		List<Vector3> locs = new List<Vector3>();
		//foreach(GameObject obj in Contents) {
		//	if(obj == null)
		//		continue;
		//	Object o = obj.GetComponent<Object>();
		//	Vector3 location = o.Coordinates;
		//	locs.Add(location);
		//	int x = (int)location.x;
		//	int y = (int)location.y;
		//	int z = (int)location.z;
		//	map[x,y,z].tileContents.Add(obj);
		//}
		
		int w = map.GetLength(0);
		int l = map.GetLength(1);
		int h = map.GetLength(2);
		//Set this to only update the player's line of sight.
		Menu_Manager m = GameObject.Find("Logic Manager").GetComponent<Menu_Manager>();
		Action_Manager actn = GameObject.Find("Logic Manager").GetComponent<Action_Manager>();
		
		for(int x = 0; x < w - 1; x++) {//This updates the tile's display depending on whatever is or isn't present.
			for(int y = 0; y < l - 1; y++) {
				map[x,y,0].tChar = map[x,y,0].tDefault;
				map[x,y,0].npcPresent = false;
				map[x,y,0].tileContents.Clear();
			}
		}
		
		foreach(GameObject g in GameObject.FindGameObjectsWithTag("NPC")) {
			Object o = g.GetComponent<Object>();
			if(o.npc.health <= 0) {
				actn.Kill(g);
				continue;
			}
			int x = (int)o.Coordinates.x;
			int y = (int)o.Coordinates.y;
			map[x,y,0].tileContents.Add(g);
			map[x,y,0].npcPresent = true;
		}
		
		foreach(GameObject g in GameObject.FindGameObjectsWithTag("Item")) {
			Object o = g.GetComponent<Object>();
			int x = (int)o.Coordinates.x;
			int y = (int)o.Coordinates.y;
				
			if(o.name.Contains("(d)") && map[x,y,0].tileContents.Count > 0) {
				if(map[x,y,0].tileContents[0].GetComponent<Object>().objectType != Object.Type.NPC) {
					map[x,y,0].tileContents.Insert(0,g);
				}
				else {
					map[x,y,0].tileContents.Insert(1,g);
				}
				map[x,y,0].canMoveTo = !o.doesBlockMovement;
				continue;
			}
			if(g.transform.parent != null && (g.transform.parent.gameObject.tag == "Player" || g.transform.parent.gameObject.tag == "NPC"))
				continue;	
			map[x,y,0].tileContents.Add(g);
			map[x,y,0].canMoveTo = !o.doesBlockMovement;
		}
		
		for(int x = 0; x < w - 1; x++) {//This updates the tile's display depending on whatever is or isn't present.
			for(int y = 0; y < l - 1; y++) {
				if(map[x,y,0].tileContents.Count > 0) {
					map[x,y,0].tChar = map[x,y,0].tileContents[0].GetComponent<Object>().display[0];
					map[x,y,0].tColor = map[x,y,0].tileContents[0].GetComponent<Object>().objectColor;
				}
			}
		}
	}
	#endregion
	#region Loading & Saving of maps
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
	#endregion
}

public struct Room {
	public int x, y, xOff, yOff;
	public bool isConnected, gone;
	public Vector2[] connectedTo;
	public Vector2 center;
	public bool combined;
	public bool decorated;
	
	public Room(int x0, int y0, bool isCon, Vector2[] cTo) {
		decorated = false;
		combined = false;
		xOff = 0;
		yOff = 0;
		gone = false;
		x = x0;
		y = y0;
		center = new Vector2(10 * x + xOff + 5, 6 * y + yOff + 3);
		isConnected = isCon;
		connectedTo = cTo;
	}
}

public struct Block {
	public int x0,y0,x1,y1;
	public List<Line.Point> border;
	
	public Block(int _x0, int _y0, int _x1, int _y1) {
		x0 = _x0;
		y0 = _y0;
		x1 = _x1;
		y1 = _y1;
		border = new List<Line.Point>();
		//Horizontal
		for(int x = 0; x <= Math.Abs(x0 - x1); x++) {
			border.Add(new Line.Point(x0 + x, y0, 0));
			border.Add(new Line.Point(x0 + x, y1, 0));
		}
		Debug.Log ("Yes");
	}
}

public struct Tile {
	public char tChar,tDefault;
	public char tDoorTile;
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
		tDoorTile = c;
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
		tDefault = tDoorTile;
		tChar = tDoorTile;
		//tChar = tDefault;
		canMoveTo = false;
		canSeeThrough = false;
		isOpen = false;
	}
}