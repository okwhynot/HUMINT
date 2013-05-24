using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using libhumint;

public class AI : MonoBehaviour {
	
	Game_Manager GameManager;
	Object thisObject,playerObject;
	List<Line.Point> path = new List<Line.Point>();
	
	int ind = 0;
	
	
	void Awake() {
		GameManager = GameObject.FindWithTag("Manager").GetComponent<Game_Manager>();
		thisObject = this.gameObject.GetComponent<Object>();
		playerObject = GameObject.Find("Player").GetComponent<Object>();
	}
	
	void Update() {
		
	}
	
	//Horizontal & vertical movement costs 10 points, diag costs 14.
	//F = G + H
	//H = the estimated movement cost to move from that given square to the end.
	//OH, don't forget to add the whole "open" list and stuff. That's super important!
	public void Pathfind() {
		path = new List<Line.Point>();
		ind = 0;
		Vector3 start = thisObject.Coordinates;
		Vector3 end = playerObject.Coordinates;

		
		int startDist = (int)System.Math.Sqrt(System.Math.Pow((end.x - start.x), 2) + System.Math.Pow((end.y - start.y), 2));
		
		int x = (int)start.x;
		int y = (int)start.y;
		int z = (int)start.z;
		int ex = (int)end.x;
		int ey = (int)end.y;
		int ez = (int)end.z;
		
		List<Line.Point> open = new List<Line.Point>();
		
		Line.Point endP = new Line.Point(ex,ey,ez);
		
		Line.Point[] _movements = {
			new Line.Point(x-1,y-1,z),
			new Line.Point(x,y-1,z),
			new Line.Point(x+1,y-1,z),
			new Line.Point(x+1,y,z),
			new Line.Point(x+1,y+1,z),
			new Line.Point(x,y+1,z),
			new Line.Point(x-1,y+1,z),
			new Line.Point(x-1,y,z)
		};
		
		Line.Point next = new Line.Point(x,y,z);
		
		repeat:
		_movements = FindAvailable(next);
		List<Line.Point> toExamine = new List<Line.Point>();
		List<Line.Point> invalid = new List<Line.Point>();
		
		foreach(Line.Point p in _movements) {//Cut out the invalid points.
			//GameManager.WorldManager.world.map[p.x,p.y,p.z].tColor = ColorLib.Green("natural"); //**DEBUG**
			if(invalid.Contains(p))
				continue;
			if(ValidCoordinates(p)) {
				toExamine.Add(p);
			}
			else if(ValidCoordinates(p) == false)
				invalid.Add(p);
		}
		int[] fcost = new int[toExamine.Count];
		//Calculate H via manhattan method, using cur point & end point (run + rise * 10)
		//Can't move to the other side of a walled structure??
		foreach(Line.Point p in toExamine) {//Determine the lowest costing move.
			int index = toExamine.IndexOf(p);
			fcost[index] = DetermineFValue(next.x, next.y, next.z, p.x, p.y, p.z, end);
		}
		next = toExamine[IndexOfCheapest(fcost)];
		path.Add(next);
		int dist = (int)System.Math.Sqrt(System.Math.Pow((end.x - next.x), 2) + System.Math.Pow((end.y - next.y), 2));
		if(startDist >= 0) {
			startDist--;
			goto repeat;
		}
		
		if(path[path.Count-1] != endP)
			Debug.Log("Failed.");
		
		foreach(Line.Point p in path)
			GameManager.WorldManager.world.map[p.x,p.y,p.z].tColor = ColorLib.Red("natural");
		ind = 0;
	}
	
	public void FollowPath() {
		if(ind == path.Count)
			return;
		thisObject.Coordinates = new Vector3(path[ind].x,path[ind].y,path[ind].z);
		ind++;
		
	}
	//x0, y0, z0 are the current tile. x1, y1, z1 are the next tile.
	//Crashes when trying to lower the y value.
	private int DetermineFValue(int x0, int y0, int z0, int x1, int y1, int z1, Vector3 e) {
		int G = 0;
		
		int ex = (int)e.x;
		int ey = (int)e.y;
		int ez = (int)e.z;
		
		int ri = x1 - x0;
		int ru = y1 - y0;
		
		int rise = System.Math.Abs(ey - y1);
		int run = System.Math.Abs(ex - x1);
		
		if(ru == 0 || ri/ru == 0) {//CARDINAL DIRECTIONS
			G = 10;
		}
		else {//DIAGONALS
			G = 14;
		}
		
		//if(GameManager.WorldManager.world.map[x1,y1,z1].canMoveTo == false)
		//	G = 60;
				
		int H = (run + rise) * 10;
		int F = G + H;
		return F;
	}
	
	private int IndexOfCheapest(int[] cost) {
		int[] sorted = new int[cost.Length];
		System.Array.Copy(cost, sorted, cost.Length);
		System.Array.Sort(sorted);
		return System.Array.IndexOf(cost, sorted[0]);
	}
	
	private Line.Point[] FindAvailable(Line.Point point) {
		int x = point.x;
		int y = point.y;
		int z = point.z;
		 
		return new Line.Point[] {
			new Line.Point(x+1,y,z),
			new Line.Point(x-1,y,z),
			new Line.Point(x,y+1,z),
			new Line.Point(x,y-1,z),
			new Line.Point(x-1,y-1,z),
			new Line.Point(x+1,y-1,z),
			new Line.Point(x+1,y+1,z),
			new Line.Point(x-1,y+1,z)
		};
	}
	
	private bool ValidCoordinates(Line.Point point) {
		int x = point.x;
		int y = point.y;
		int z = point.z;
		
		if(x < 0)
			return false;
		if(y < 0)
			return false;
		if(z < 0)
			return false;
		if(x > GameManager.WorldManager.world.map.GetLength(0) - 1)
			return false;
		if(y > GameManager.WorldManager.world.map.GetLength(1) - 1)
			return false;
		if(z > GameManager.WorldManager.world.map.GetLength(2) - 1)
			return false;
		if(GameManager.WorldManager.world.map[x,y,z].canMoveTo == false)
			return false;
		if(path.Contains(point))
			return false;
		if(GameManager.WorldManager.world.map[x,y,z].isDoor && GameManager.WorldManager.world.map[x,y,z].isOpen == false)
			return false;
		return true;
	}
}