using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using libhumint;

public class Object : MonoBehaviour {
	public enum Type {Player,NPC,Item};
	public Vector3 Coordinates;
	public Type objectType;
	public string name;
	public string desc;
	public int id;
	public string display;
	public Player player;
	public NPC npc;
	public Item item;
	public Color objectColor;
	public bool doesBlockMovement;
	
	public class Entity {
		public enum SEX {Male,Female};
		public enum FACTION {SIS,CIA,Staasi,KGB,Civ};
		public SEX sex;
		public FACTION faction;
		public GameObject[] inventory;
		public GameObject EquippedWeapon;
		public int health = 100;
	}
	
	[System.Serializable]
	public class Player : Entity {
		enum SPEC {Pistols,Rifles,Equipment,Explosives,Manipulation};
		public string realName;
		private Object _parent;
		private SPEC _spec;
		private int _perk;
		public int experience;
		private Color32[] _visual = new Color32[3];
		public Player(Object parent, string alias, string realName,int[] ccvalues) {
			health = 100;
			_parent = parent;
			_parent.name = alias;
			this.realName = realName;
			faction = (FACTION)ccvalues[2];
			_spec = (SPEC)ccvalues[3];
			_perk = ccvalues[4];
			sex = (SEX)ccvalues[5];
			for(int i=0;i<3;i++)
				_visual[i] = Menu_Manager.ccCols[i][ccvalues[i+6]];
		}
	}
	[System.Serializable]
	public class NPC : Entity {
		public NPC() {
			
		}
	}
	[System.Serializable]
	public class Item {
		public enum Subtype {WorldObject,Clothing,Weapon,Gadget,Ammo};
		public Subtype itemType;
		public bool isHeld,doesStack,canPickUp;
		public int minDmg,maxDmg;
		public int capacity,curRounds;
		public string ammoType;
		
		public Item() {
		
		}
	}
}