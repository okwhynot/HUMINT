using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using libhumint;

public class HUMINT_Object : MonoBehaviour {
	//Item IDs break down like this:
	//First number is the type of item (weapon, etc)
	//Second is the item subtype
	//The rest are its unique identifier within that category. 5 digits long.
	//1 = Player, 2 = Object, 3 = Entity
	//21 = World Obj, 22 = Clothes, 23 = Weapons, 24 = Gadgets, 25 = Ammo
	//231 = Melee, 232 = Pistol, 233 = Rifle
	//For world objects, the second digit determines whether or not it blocks movement and line of sight.
	public enum Type {Player,Object,Entity};
	public Type type;
	public Vector2 Coordinates;
	public string name;
	public int id;
	public Item item;
	public Player player;
	public Entity entity;
	public Vector2 curMap;
	
	public void Create(string name, int id, int[] ccval = null, int[] vsval = null,Vector2 curMap = new Vector2()) {
		this.name = name;
		this.id = id;
		this.curMap = curMap;
		char[] idS = id.ToString().ToCharArray();
		int idS1 = Convert.ToInt32(id.ToString().Substring(0,1));
		int idS2 = Convert.ToInt32(id.ToString().Substring(1,1));
		type = (Type)idS1-1;
		switch(type)
		{
			case Type.Player:
			{
				player = new Player(ccval,vsval);
				break;
			}
			case Type.Object:
			{
				item = new Item(idS2);
				break;
			}
			case Type.Entity:
			{
				entity = new Entity();
				break;
			}
		}
	}
	public void Draw() {
		
	}
	public void Move() {
		
	}
	public class Entity {
		public enum Sex {Male,Female};
		public Sex sex;
		public int age;
		public int btype;
		public int cob;
		public DateTime dob;
		public Entity() {
			sex = (Sex)UnityEngine.Random.Range(0,2);
			cob = UnityEngine.Random.Range(0,UIStrings.CityNames.Length);
			btype = UnityEngine.Random.Range(0,UIStrings.BloodTypes.Length);
			//dob = new DateTime(1980-age,1,UnityEngine.Random.Range(1,31)); add back when random age stuff is done!
		}
	}
	[System.Serializable]
	public class Player : Entity {
		public enum Spec {Pistols,Explosives,Manipulation,Rifles};
		public Spec spec;
		int[] visual;
		public Container playerInventory;
		public Player(int[] ccval,int[] vsval) : base() {
			sex = (Sex)ccval[4];
			spec = (Spec)ccval[3];
			visual = vsval;
			age = vsval[5];
			dob = new DateTime(1980-age,1,UnityEngine.Random.Range(1,31));
			Debug.Log(dob.Day+"/"+dob.Month+"/"+dob.Year);
			playerInventory = new Container(8);
			StartingGear();
		}
		void StartingGear() {
			GameObject p = GameObject.FindWithTag("Player");
			switch(spec)
			{
				case Spec.Pistols:
				{
					GameObject wpn = (GameObject)Instantiate(GameObject.Find("PSM (G)"));
					GameObject amm1 = (GameObject)Instantiate(GameObject.Find("5.45x18mm Mag (G)"));
					GameObject amm2 = (GameObject)Instantiate(GameObject.Find("5.45x18mm Mag (G)"));
					GameObject amm3 = (GameObject)Instantiate(GameObject.Find("5.45x18mm Mag (G)"));
					GameObject knife = (GameObject)Instantiate(GameObject.Find("Survival Knife (G)"));
					playerInventory.Add(wpn,p);
					playerInventory.Add(amm1,p);
					playerInventory.Add(amm2,p);
					playerInventory.Add(amm3,p);
					playerInventory.Add(knife,p);
					break;
				}
				case Spec.Explosives:
				{
					GameObject wpn = (GameObject)Instantiate(GameObject.Find("PSM (G)"));
					playerInventory.Add(wpn,p);
					break;
				}
				case Spec.Manipulation:
				{
					break;
				}
				case Spec.Rifles:
				{
					break;
				}
			}
		}
		public void Movement() {
			GameObject p = GameObject.FindWithTag("Player");
			HUMINT_Game hgame = GameObject.Find("Main Camera").GetComponent<HUMINT_Game>();
			HUMINT_World hworld = GameObject.Find("Main Camera").GetComponent<HUMINT_World>();
			Vector2 pCoords = p.GetComponent<HUMINT_Object>().Coordinates;
			Vector2 cMap = p.GetComponent<HUMINT_Object>().curMap;
			if(Input.GetKeyDown("up"))
			{
				if(pCoords.y-1 == -1)
				{
					hworld.ChangeMap('^');
					hgame.Pulse();
				}
				if(hworld.worldMap[(int)cMap.x,(int)cMap.y].collisionMap[(int)pCoords.x,(int)pCoords.y-1] == true)
					p.GetComponent<HUMINT_Object>().Coordinates.y-=1;
				//hgame.Pulse();
			}
			else if(Input.GetKeyDown("down"))
			{
				if(pCoords.y+1 == 18)
				{
					hworld.ChangeMap('v');
					hgame.Pulse();
				}
				if(hworld.worldMap[(int)cMap.x,(int)cMap.y].collisionMap[(int)pCoords.x,(int)pCoords.y+1] == true)
				{
					p.GetComponent<HUMINT_Object>().Coordinates.y+=1;
					//hgame.Pulse();
				}
			}
			else if(Input.GetKeyDown("right"))
			{
				if(hworld.worldMap[(int)cMap.x,(int)cMap.y].collisionMap[(int)pCoords.x+1,(int)pCoords.y] == true)
					p.GetComponent<HUMINT_Object>().Coordinates.x+=1;
				//hgame.Pulse();
			}
			else if(Input.GetKeyDown("left"))
			{
				if(hworld.worldMap[(int)cMap.x,(int)cMap.y].collisionMap[(int)pCoords.x-1,(int)pCoords.y] == true)
					p.GetComponent<HUMINT_Object>().Coordinates.x-=1;
				//hgame.Pulse();
			}
			if(Input.GetKeyDown("up") || Input.GetKeyDown("down") || Input.GetKeyDown("left") || Input.GetKeyDown("right"))
				hgame.Pulse();
		}
	}
	[System.Serializable]
	public class Item {
		public enum Subtype {WorldObject,Clothing,Weapon,Gadget,Ammo};
		public Subtype itemType;
		public bool isHeld,doesStack,canPickUp;
		public Item(int subtype) {
			itemType = (Subtype)subtype-1;
			switch(itemType)
			{
				case Subtype.Weapon:
				{
					doesStack = false;
					canPickUp = true;
					break;
				}
				case Subtype.Ammo:
				{
					doesStack = true;
					canPickUp = true;
					break;
				}
			}
		}
	}
	public class Weapon {
		
	}
	public class Magazine {
		
	}
	public class WorldObject {
		public bool blockMovement,blockSight;
	}
	[System.Serializable]
	public class Container {
		public GameObject[][] contents;
		int cSlot;
		public Container(int size) {
			contents = new GameObject[size][];
			for(int i=0;i<size;i++)
				contents[i] = new GameObject[10];
		}		
		public void Add(GameObject toAdd,GameObject to) {
			toAdd.name = toAdd.GetComponent<HUMINT_Object>().name;
			toAdd.GetComponent<HUMINT_Object>().item.isHeld = true;
			toAdd.transform.parent = to.transform;
			//SETUP TAGS TOO
			for(int i=0;i<contents.Length;i++)
			{
				if(contents[i][0] == null)
				{
					contents[i][0] = toAdd;
					break;
				}
				else if(contents[i][0].name == toAdd.name)
				{
					for(int x=0;x<10;x++)
						if(contents[i][x] == null)
						{
							contents[i][x] = toAdd;
							break;
						}
					break;
				}
			}		
		}
		public void Remove() {
			
		}
	}
}