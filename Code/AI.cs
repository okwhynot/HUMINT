using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using libhumint;
using libhumint.Data;

public class AI : MonoBehaviour {
	byte[,] size;
	Game_Manager GameManager;
	Object thisObject,playerObject;
	List<Line.Point> path = new List<Line.Point>();
	
	int ind = 0;
	
	
	void Awake() {
		GameManager = GameObject.FindWithTag("Manager").GetComponent<Game_Manager>();
		thisObject = this.gameObject.GetComponent<Object>();
		playerObject = GameObject.Find("Player").GetComponent<Object>();
	}
	
	void Start() {
	
	}
	
	void Update() {
		
	}
		
	//Horizontal & vertical movement costs 10 points, diag costs 14.
	//F = G + H
	//H = the estimated movement cost to move from that given square to the end.
	//OH, don't forget to add the whole "open" list and stuff. That's super important!
	public void Pathfind() {
		path.Clear();
		ind = 0;
		int curTry = 0;
		//int maxTries = 100;
		Vector3 _start = thisObject.Coordinates;
		Vector3 _end = playerObject.Coordinates;
		Line.Point start = new Line.Point((int)_start.x,(int)_start.y,(int)_start.z);
		Line.Point end = new Line.Point((int)_end.x,(int)_end.y,(int)_end.z);
		int maxTries = 5 * (Math.Abs(start.x - end.x) + Math.Abs (start.y - end.y));
		//int maxTries = 300;
		BinaryHeap<Node> open = new BinaryHeap<Node>();
		//List<Node> open = new List<Node>();
		List<Node> closed = new List<Node>();
			
		Node finish = new Node(end, end);
		Node origin = new Node(start, end);
		open.Add(origin);
		Node current = open.Remove(); //Sets current to the lowest f-cost square in open and removes it.
		closed.Add(current); //Adds the current tile to closed.
		#region Loop
		loopstart:
			
		Node[] adjacent = AdjacentNodes(closed[closed.Count-1]); //Ignores closed or unwalkable spaces.
		open.Clear ();
		foreach(Node node in adjacent) {
			bool found = false;
			foreach(Node n in closed) {
				if(node._this == n._this) {
					found = true;
					break;
				}
			}
			if(found == false)
				open.Add(node);
		}
		current = open.Peek();
		if(current._parent.x == end.x && current._parent.y == end.y) {
			Debug.Log("Successfully found line.");
			goto done;
		}
		closed.Add(open.Remove());
		
		curTry++;
		if(curTry < maxTries)
			goto loopstart;
		#endregion
		
		#region Finalization (Debug coloring)
		done:
			
		Node last = closed[closed.Count - 1];
		
		//foreach(Node n in open) {
		//	GameManager.WorldManager.world.map[n.x,n.y,n.z].tColor = ColorLib.Green("cga");
		//}
		
		restart:
		
		
		
		
		foreach(Node n in closed) {
			GameManager.WorldManager.world.map[n.x,n.y,n.z].tColor = ColorLib.Red("cga");
		}	
		GameManager.WorldManager.world.map[last.x,last.y,last.z].tColor = ColorLib.Yellow("cga");
		#endregion
	}
	
	private Node[] AdjacentNodes(Node parent) {
		World_Manager w = GameManager.WorldManager;
		List<Node> adjacent = new List<Node>();
		for(int x = -1; x <= 1; x++) {
			for(int y = -1; y <= 1; y++) {
				if(x == 0 && y == 0)
					continue;
				int _x = parent.x + x;
				int _y = parent.y + y;
				if(_x < 0 || _y < 0)
					continue;
				//Add in z-axis later. maybe. 
				if(w.world.map[_x,_y,0].canMoveTo == true) {//Ignores invalid nodes.
					Line.Point cur = new Line.Point(_x,_y,0);
					Line.Point end = parent._end;
					Node toAdd = new Node(cur,parent);
					adjacent.Add(toAdd);
				}
			}
		}
		return adjacent.ToArray();
	}
	
	public void FollowPath() {
		if(ind == path.Count)
			return;
		thisObject.Coordinates = new Vector3(path[ind].x,path[ind].y,path[ind].z);
		ind++;
	}
	
	private bool ValidCoordinates(Node point) {
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
		if(path.Contains(point._this))
			return false;
		if(GameManager.WorldManager.world.map[x,y,z].isDoor && GameManager.WorldManager.world.map[x,y,z].isOpen == false)
			return false;
		return true;
	}
	
	class Node : IComparable<Node> {
		public Line.Point _this;
		public int x,y,z;
		public Line.Point _parent;
		public Line.Point _end;
		public int h,g,f;
		public Node parent;
		
		//Only used to initialize the parent.
		public Node(Line.Point cur, Line.Point end) {
			_this = cur;
			x = cur.x;
			y = cur.y;
			z = cur.z;
			_end = end;
			g = 0;
			h = heuristic();
			//h *= (int)(1.0 + (1/1000));
			f = h + g;
			parent = this;
		}
		
		public Node(Line.Point cur, Node parent) {
			_this = cur;
			this.parent = parent;
			_parent = parent._this;
			_end = parent._end;
			x = cur.x;
			y = cur.y;
			z = cur.z;
			
			h = heuristic();
			g = parent.g + gValue();
			f = h + g;
		}
		
		public int CompareTo(Node other) {
			return f - other.f;
		}
		public override string ToString () {
			 return "Node @ ("+x+","+y+"):" + f.ToString();
		}
		int gValue() {
			int rise = Math.Abs(_parent.y - y);
			int run = Math.Abs(_parent.x - x);
			
			if(rise == run) {
				return 14;
			}
			return 10;
		}
		//Honestly you could -probably- just use the manhattan method. It'd give decent-ish lines.
		int heuristic() {
			int H = 0;
			int xDistance = Math.Abs(x - _end.x);
			int yDistance = Math.Abs(y - _end.y);
			
			if(xDistance > yDistance)
				H = 14*yDistance + 10*(xDistance-yDistance);
			else
				H = 14*xDistance + 10*(yDistance-xDistance);
			return H;
		}
	}
}