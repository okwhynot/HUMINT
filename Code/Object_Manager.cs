using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using libhumint;

public class Object_Manager : MonoBehaviour {
	
	void Awake() {
		LoadItems();
	}
	
	void Update() {
		
	}
	
	void LoadItems() {
		TextAsset source = Resources.Load("Items") as TextAsset;
		GameObject itemParent = new GameObject();
		itemParent.name = "Item Parent";
		DontDestroyOnLoad(itemParent);
		string[] sections = source.text.Split('}');
		foreach(string s in sections) {
			if(s.Contains("name:") == false)
				break;
			GameObject item = new GameObject();
			item.AddComponent<Object>();
			item.GetComponent<Object>().objectType = (Object.Type)2;
			item.GetComponent<Object>().item = new Object.Item();
			string[] fields = s.TrimStart().Trim('{').TrimStart().Split('\n');
			string _name = null;
			string _desc = null;
			int _id = 0;
			string _ammo = null;
			int _minDmg = 0;
			int _maxDmg = 0;
			int _capacity = 0;
			string _color = null;
			foreach(string f in fields) {
				string fi = f;
				if(fi.Contains("#")) {
					int index = fi.IndexOf('#');
					fi = fi.Substring(0,index);
				}
				if(fi.Contains("name:"))
					_name = fi.Replace("name:",string.Empty).TrimStart();
				else if(fi.Contains("id:"))
					_id = System.Int32.Parse(fi.Replace("id:",string.Empty).TrimStart());
				else if(fi.Contains("description:"))
					_desc = fi.Replace("description:",string.Empty).TrimStart();
				else if(fi.Contains("ammo:"))
					_ammo = fi.Replace("ammo:",string.Empty).TrimStart();
				else if(fi.Contains("damage:")) {
					_minDmg = System.Int32.Parse(fi.Replace("damage:",string.Empty).TrimStart().Split(',')[0]);
					_maxDmg = System.Int32.Parse(fi.Replace("damage:",string.Empty).TrimStart().Split(',')[1]);
				}
				else if(fi.Contains("cap:")) {
					_capacity = System.Int32.Parse(fi.Replace("cap:",string.Empty).TrimStart());
				}
				else if(fi.Contains("color:"))
					_color = fi.Replace("color:",string.Empty).TrimStart();
			}
			if(_id.ToString()[1] == '3') {
				item.GetComponent<Object>().item.itemType = Object.Item.Subtype.Weapon;
				item.GetComponent<Object>().display += (char)169;
				item.GetComponent<Object>().item.doesStack = false;
			}
			if(_id.ToString()[1] == '5') {
				item.GetComponent<Object>().item.itemType = Object.Item.Subtype.Ammo;
				item.GetComponent<Object>().display += (char)254;
				item.GetComponent<Object>().item.doesStack = true;
			}
			item.name = _name + " (G)";
			item.tag = "Generic Item";
			item.GetComponent<Object>().name = _name;
			item.GetComponent<Object>().id = _id;
			item.GetComponent<Object>().desc = _desc;
			item.GetComponent<Object>().item.canPickUp = true;
			item.GetComponent<Object>().item.minDmg = _minDmg;
			item.GetComponent<Object>().item.maxDmg = _maxDmg;
			item.GetComponent<Object>().item.capacity = _capacity;
			item.GetComponent<Object>().item.ammoType = _ammo;
			item.transform.parent = itemParent.transform;
			DontDestroyOnLoad(item);
		}
	}
}