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
	public Menu_Manager MenuManager;
	
	public void Throw(int x0, int y0, int x1, int y1) {
		
	}
	
	public void Fire(int x0, int y0, int x1, int y1, GameObject wpn) {
		Object wpnObject = wpn.GetComponent<Object>();
		if(wpnObject.item.curRounds == 0) {
			GameObject parent = wpn.transform.parent.gameObject;
			Object parObj = parent.GetComponent<Object>();
			GameObject[] inv = new GameObject[1];
			if(parObj.objectType == Object.Type.NPC) {
				inv = parObj.npc.inventory;
			}
			if(parObj.objectType == Object.Type.Player) {
				inv = parObj.player.inventory;
			}
			Reload(inv,wpn);
			return;
		}
		
		int damage = UnityEngine.Random.Range(wpnObject.item.minDmg,wpnObject.item.maxDmg)*10;
		Debug.Log(damage);
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
			Hit(targetobj.GetComponent<Object>(),damage);
		wpn.GetComponent<Object>().item.curRounds -= 1;
		if(targetobj != null)
			MenuManager.combatLog = "Fired "+wpn.name+" at "+targetobj.name+" and hit!";
	}
	
	public void Attack(GameObject obj, GameObject wpn) {
		bool miss = (UnityEngine.Random.Range(0,100) < 30);
		if(miss) {
			MenuManager.combatLog = "Missed "+obj.name+"!";
			return;
		}
		Object w = wpn.GetComponent<Object>();
		MenuManager.combatLog = "Hit "+obj.name+" with "+w.name+"!";
		obj.GetComponent<Object>().npc.health -= UnityEngine.Random.Range(40,60);
	}
	
	public void Punch(GameObject obj) {
		bool miss = (UnityEngine.Random.Range(0,100) < 30);
		if(miss) {
			MenuManager.combatLog = "Missed "+obj.name+"!";
			return;
		}
		Object o = obj.GetComponent<Object>();
		int dmg = Dice.Roll(8,2);
		o.npc.health -= dmg;
		int h = o.npc.health;
		bool blood = (UnityEngine.Random.Range(0,6) == 1);
		
		if(blood) {
			Hit(o,10);
		}
			
		string[] locations = {"face","gut","arm","chest"};
		string cLog = "Punched "+o.name+" in the "+locations[UnityEngine.Random.Range(0,4)];
		if(h < 0)
			cLog += ", killing him!";
		else
			cLog +="!";
		MenuManager.combatLog = cLog;
	}
	
	public void PunchPlayer(GameObject obj) {
		bool miss = (UnityEngine.Random.Range(0,100) < 40);
		if(miss) {
			return;
		}
		Object player = GameObject.FindWithTag("Player").GetComponent<Object>();
		int dmg = Dice.Roll(4,2);
		player.player.health -= dmg;
	}
	
	public void Kill(GameObject obj) {
		GameObject body = GameObject.Instantiate(Resources.Load("Object")) as GameObject;
		body.name = obj.name + " (d)";
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
		if(o.objectType == Object.Type.NPC) {
			o.npc.health -= dmg;
			Debug.Log(o.npc.health);
		}
		else if(o.objectType == Object.Type.Player) {
			o.player.health -= dmg;
			Debug.Log(o.player.health);
		}
		
		GameObject blood = GameObject.Instantiate(Resources.Load("Object")) as GameObject;
		
		blood.name = "Blood";
		blood.GetComponent<Object>().name = "Blood";
		blood.GetComponent<Object>().display = "~";
		blood.GetComponent<Object>().objectColor = ColorLib.Red("cga");
		blood.GetComponent<Object>().Coordinates = o.Coordinates;
		WorldManager.world.map[x,y,z].tileContents.Add(blood);
	}
	
	public void Reload(GameObject[] inventory, GameObject weapon) {
		Object wpn = weapon.GetComponent<Object>();
		string ammo = wpn.item.ammoType;
		GameObject magazine = null;
		foreach(GameObject item in inventory) {
			Object o = item.GetComponent<Object>();
			if(o.name.Contains(ammo) && o.item.itemType == Object.Item.Subtype.Ammo) {
				Debug.Log("Has ammo.");
				magazine = item;
				break;
			}
		}
		if(magazine == null) {
			Debug.Log("No ammo.");
			return;
		}
		wpn.item.curRounds = magazine.GetComponent<Object>().item.capacity;
		Destroy(magazine);
	}
	
	public void PickUpObject(int x, int y, GameObject obj) {
		Object o = obj.GetComponent<Object>();
		GameObject playerO = GameObject.FindWithTag("Player");
		obj.transform.parent = playerO.transform;
		if(playerO.GetComponent<Object>().player.EquippedWeapon == null && o.id.ToString()[1] == '3') {
			playerO.GetComponent<Object>().player.EquippedWeapon = obj;
		}
	}
}