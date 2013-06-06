using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using libhumint;
using libhumint.Data;

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
		int maxTries = 10 * (Math.Abs(start.x - end.x) + Math.Abs (start.y - end.y));
		//int maxTries = 300;
		BinaryHeap<Node> open = new BinaryHeap<Node>();
		//List<Node> open = new List<Node>();
		List<Node> closed = new List<Node>();
		
		//Line toPlayer = new Line(start.x, start.y, end.x, end.y);
		
		Node finish = new Node(end, start);
		Node origin = new Node(start, end);
		
		
		open.Add(origin);
		Node current = open.Remove(); //Sets current to the lowest f-cost square in open and removes it.
		closed.Add(current); //Adds the current tile to closed.
		#region Loop
		loopstart:
		
		Node n = closed[closed.Count - 1];
		Node[] adjacent = AdjacentNodes(n); //Ignores closed or unwalkable spaces.
		open.Clear();
		List<Node> _temp = new List<Node>();
		List<Node> _open = open.ToList();
		foreach(Node node in adjacent) {
			int newg = n.g + node.gValue();
			int indexC = closed.FindIndex(f => f._this == node._this);
			int indexO = _open.FindIndex(f => f._this == node._this);
			int newf = newg + node.h;
			if(indexC != -1 && closed[indexC].g <= newg || indexO != -1 && _open[indexO].g <= newg) {
				break;
			}
			Node n1 = new Node(node._this,end);
			n1.parent = n;
			n1._parent = n._this;
			n1._end = n1.parent._end;
			n1.g = newg;
			n1.h = n1.heuristic();
			n1.f = n1.g + n1.h;
			
			if(indexC != -1) {
				closed.RemoveAt(indexC);
				Debug.Log ("Success");
			}
			
			if(indexO == -1) {
				open.Add(n1);
			}
		}
		
		current = open.Peek();
		if(current.x == end.x && current.y == end.y) {
			Debug.Log("Successfully found path.");
			Debug.Log (closed.Count);
			goto done;
		}
		_temp.Add(open.Peek());
		closed.Add(open.Remove());
		
		curTry++;
		if(curTry < maxTries && open.Count > 0) {
			
			goto loopstart;
		}
		else if(curTry == maxTries && open.Count > 0) {
			
		}
		else if(curTry >= maxTries) {
			Debug.Log ("Failed to find line.");
			Debug.Log (closed.Count + " items in the closed list.");
			
		}
		#endregion
		
		#region Finalization (Debug coloring)
		done:
		closed.RemoveAt(0);
		Node last = closed[closed.Count - 1];
		
		//foreach(Line.Point p in toPlayer.getPoints()) {
			//GameManager.WorldManager.world.map[p.x,p.y,p.z].tColor = ColorLib.LightGreen("cga");
			//GameManager.WorldManager.world.map[p.x,p.y,p.z].tDefault = '-';
		//}
		
		foreach(Node no in open) {
			//GameManager.WorldManager.world.map[no.x,no.y,no.z].tColor = ColorLib.Green("cga");
			//GameManager.WorldManager.world.map[no.x,no.y,no.z].tDefault = 'O';
		}
		
		foreach(Node no in closed) {
			GameManager.WorldManager.world.map[no.x,no.y,no.z].tColor = ColorLib.Red("cga");
			GameManager.WorldManager.world.map[no.x,no.y,no.z].tDefault = 'P';
		}
		GameManager.WorldManager.world.map[last.x,last.y,last.z].tColor = ColorLib.Yellow("cga");
		GameManager.WorldManager.world.map[last.x,last.y,last.z].tDefault = 'E';
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
				if(_x < 0 || _y < 0 || _x > w.world.map.GetUpperBound(0) || _y > w.world.map.GetUpperBound(1))
					continue;
				
				//Add in z-axis later. maybe. 
				if(w.world.map[_x,_y,0].canMoveTo == true) {//Ignores invalid nodes.
					Line.Point cur = new Line.Point(_x,_y,0);
					Line.Point end = parent._end;
					Node toAdd = new Node(cur);
					toAdd.parent = parent;
					toAdd._parent = parent._this;
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
		int _g;
		public Node parent;
		
		//Only used to initialize the starting node.
		public Node(Line.Point cur, Line.Point end) {
			_this = cur;
			x = cur.x;
			y = cur.y;
			z = cur.z;
			_end = end;
			g = 0;
			parent = this;
			h = heuristic();
			//h *= (int)(1.0 + (1/1000));
			f = h + g;
			parent = null;
		}
		
		public Node(Line.Point cur) {
			_this = cur;
			x = cur.x;
			y = cur.y;
			z = cur.z;
		}
		
		public int CompareTo(Node other) {
			return f - other.f;
		}
		public override string ToString () {
			 return "Node @ ("+x+","+y+"):" + f.ToString();
		}
		public int gValue() {
			int rise = Math.Abs(_parent.y - y);
			int run = Math.Abs(_parent.x - x);
			
			if(rise == run) {
				return 14;
			}
			return 10;
		}
		//Honestly you could -probably- just use the manhattan method. It'd give decent-ish lines.
		public int heuristic() {
			int xDistance = Math.Abs(x - _end.x);
			int yDistance = Math.Abs(y - _end.y);
			
			//return (g - parent.g) * Math.Max(xDistance, yDistance);
			
			int _g1 = 0;
			int _g0 = g - parent.g;
			if(_g0 == 10)
				_g1 = 14;
			else if(_g0 == 14)
				_g1 = 10;
			
			//return _g0 * (xDistance + yDistance) + (_g1 - 2 * _g0) * Math.Min(xDistance, yDistance);
			
			if(xDistance > yDistance)
				return 14*yDistance + 10*(xDistance-yDistance);
			else
				return 14*xDistance + 10*(yDistance-xDistance);
		}
	}
}