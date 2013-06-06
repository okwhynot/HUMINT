using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using libhumint;
using libhumint.Dice;

public class Action_Manager : MonoBehaviour {
	public enum ACTIONSTATE {MOVE,ATTACK,CLOSE,OPEN,EXAMINE,LOOK,PICKUP};
	public ACTIONSTATE state = ACTIONSTATE.MOVE;
	public World_Manager WorldManager;
	List<GameObject> npcs = new List<GameObject>();
	
	public void Throw(int x0, int y0, int x1, int y1) {
		
	}
	
	public void Fire(int x0, int y0, int x1, int y1) {
		Line LoFire = new Line(x0,y0,x1,y1);
		GameObject targetobj = null;
		foreach(Line.Point p in LoFire.getPoints()) {
			if(WorldManager.world.map[p.x,p.y,p.z].canSeeThrough == false) {
				Debug.Log("Hit wall @ "+p.x +","+p.y+","+p.z);
				break;
			}
			
			if(WorldManager.world.map[p.x,p.y,p.z].npcPresent) {			
				foreach(GameObject gobj in WorldManager.world.map[p.x,p.y,p.z].tileContents) {
					if(gobj.tag == "NPC") {
						targetobj = gobj;
						break;
					}
				}
			}
		}
		if(targetobj != null)
			Hit(targetobj.GetComponent<Object>(),Dice.Roll(6,2));
	}
	
	public void Punch(GameObject obj) {
		Object o = obj.GetComponent<Object>();
		int dmg = Dice.Roll(6,2);
		o.npc.health -= dmg;
		bool blood = (UnityEngine.Random.Range(0,3) == 1);
		if(blood) {
			Hit(o,10);
		}
			
		string[] locations = {"face","gut","arm","chest"};
		Debug.Log("Hit "+o.name+" in the face for "+dmg+" damage! He now has "+o.npc.health+" health left.");
	}
	
	public void Kill(GameObject obj) {
		GameObject body = GameObject.Instantiate(Resources.Load("Object")) as GameObject;
		body.name = obj.name + " (dead)";
		Object o = body.GetComponent<Object>();
		Object o1 = obj.GetComponent<Object>();
		o.display = o1.display;
		o.name = body.name;
		o.Coordinates = o1.Coordinates;
		o.objectColor = ColorLib.Red("cga");
		Destroy(obj);
	}
	
	void Hit(Object o, int dmg) {
		int x = (int)o.Coordinates.x;
		int y = (int)o.Coordinates.y;
		int z = (int)o.Coordinates.z;
		//Debug.Log("Hit "+o.name+" @ "+x+","+y+","+z+" with WPNNAME for "+dmg +" damage.");
		//WorldManager.world.map[x,y,z].tCDefault = ColorLib.Red("cga");
		GameObject blood = GameObject.Instantiate(Resources.Load("Object")) as GameObject;

		blood.name = "Blood";
		blood.GetComponent<Object>().name = "Blood";
		blood.GetComponent<Object>().display = "~";
		blood.GetComponent<Object>().objectColor = ColorLib.Red("cga");
		blood.GetComponent<Object>().Coordinates = o.Coordinates;
		WorldManager.world.map[x,y,z].tileContents.Add(blood);
	}
	
	void PickUpObject(int x, int y) {
		if(WorldManager.world.map[x,y,0].tileContents.Count == 0) {
			Debug.Log("nothing to pick up!");
			
		}
	}
}