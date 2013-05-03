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
	//221 = hat, 222 = jacket, 223 = shirt, 224 = gloves, 225 = pants, 226 = shoes
	//For world objects, the second digit determines whether or not it blocks movement and line of sight.
	public enum Type {Player,Object,NPC};
	public Type type;
	public Vector2 Coordinates;
	public string name;
	public int id;
	public Item item;
	public Player player;
	public NPC other;
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
			case Type.NPC:
			{
				other = new NPC();
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
		public int hp;
		//public GameObject equippedWeapon;
		//public GameObject equippedHat,equippedShirt,equippedGloves,equippedPants,equippedShoes;
		//weapon, hat, jacket, shirt, gloves, pants, shoes
		public GameObject[] equipped = new GameObject[7];
		public Sex sex;
		public int age;
		public int btype;
		public int cob;
		public DateTime dob;
		public Entity() {
			hp = 100;
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
		public bool wpnHolstered = true;
		public Player(int[] ccval,int[] vsval) : base() {
			sex = (Sex)ccval[4];
			spec = (Spec)ccval[3];
			visual = vsval;
			age = vsval[5];
			dob = new DateTime(1980-age,1,UnityEngine.Random.Range(1,31));
			Debug.Log(dob.Day+"/"+dob.Month+"/"+dob.Year);
			playerInventory = new Container(8,GameObject.FindWithTag("Player"));
			StartingGear();
		}
		void StartingGear() {
			GameObject p = GameObject.FindWithTag("Player");
			GameObject jacket = (GameObject)Instantiate(GameObject.Find("Winter jacket (G)"));
			GameObject shirt = (GameObject)Instantiate(GameObject.Find("Worker's shirt (G)"));
			GameObject pants = (GameObject)Instantiate(GameObject.Find("Worker's pants (G)"));
			GameObject shoes = (GameObject)Instantiate(GameObject.Find("Worker's boots (G)"));
			playerInventory.Add(jacket,p);
			playerInventory.Add(shirt,p);
			playerInventory.Add(pants,p);
			playerInventory.Add(shoes,p);
			equipped[2] = jacket;
			equipped[3] = shirt;
			equipped[5] = pants;
			equipped[6] = shoes;
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
					equipped[0] = wpn;
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
			HUMINT_World hworld = Camera.main.GetComponent<HUMINT_World>();
			int pX = (int)p.GetComponent<HUMINT_Object>().Coordinates.x;
			int pY = (int)p.GetComponent<HUMINT_Object>().Coordinates.y;
			if(Input.GetKeyDown("up")) {
				if(hworld.world.z1[pX,pY-1].canMoveTo)
					p.GetComponent<HUMINT_Object>().Coordinates.y-=1;
				else if(hworld.world.z1[pX,pY-1].isDoor)
					hworld.world.z1[pX,pY-1].Open();
			}
			if(Input.GetKeyDown("down")) {
				if(hworld.world.z1[pX,pY+1].canMoveTo)
					p.GetComponent<HUMINT_Object>().Coordinates.y+=1;
				else if(hworld.world.z1[pX,pY+1].isDoor)
					hworld.world.z1[pX,pY+1].Open();
			}
			if(Input.GetKeyDown("left")) {
				if(hworld.world.z1[pX-1,pY].canMoveTo)
					p.GetComponent<HUMINT_Object>().Coordinates.x-=1;
				else if(hworld.world.z1[pX-1,pY].isDoor)
					hworld.world.z1[pX-1,pY].Open();
			}
			if(Input.GetKeyDown("right")) {
				if(hworld.world.z1[pX+1,pY].canMoveTo)
					p.GetComponent<HUMINT_Object>().Coordinates.x+=1;
				else if(hworld.world.z1[pX+1,pY].isDoor)
					hworld.world.z1[pX+1,pY].Open();
			}
		}
	}
	public class NPC : Entity {
		
	}
	[System.Serializable]
	public class Container {
		public GameObject[][] contents;
		public GameObject parent;
		int cSlot;
		public Container(int size, GameObject par) {
			parent = par;
			contents = new GameObject[size][];
			for(int i=0;i<size;i++)
				contents[i] = new GameObject[10];
		}
		public void Add(GameObject toAdd,GameObject to) {
			toAdd.name = toAdd.GetComponent<HUMINT_Object>().name;
			toAdd.GetComponent<HUMINT_Object>().item.isHeld = true;
			toAdd.transform.parent = to.transform;
		}
		public void Remove() {
			
		}
	}
}
[System.Serializable]
public struct Item {
	public enum Subtype {WorldObject,Clothing,Weapon,Gadget,Ammo};
	public Subtype itemType;
	public bool isHeld,doesStack,canPickUp;
	public Item(int subtype) {
		isHeld = false; //placeholder values
		doesStack = true; //placeholder values
		canPickUp = true; //placeholder values
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