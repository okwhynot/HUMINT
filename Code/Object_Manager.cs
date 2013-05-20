using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using libhumint;

public class Object_Manager : MonoBehaviour {
	bool hasLoadedObjects = false;
	
	void Awake() {
		//LoadItems();
	}
	
	void Update() {
		if(Application.loadedLevel == 1 && hasLoadedObjects == false) {
			LoadItems();
			hasLoadedObjects = true;
		}
	}
	
	void LoadItems() {
		TextAsset source = Resources.Load("Items") as TextAsset;
		GameObject itemParent = new GameObject();
		itemParent.name = "Item Parent";
		string[] sections = source.text.Split('}');
		foreach(string s in sections) {
			if(s.Contains("name:") == false)
				break;
			GameObject item = new GameObject();
			item.AddComponent<Object>();
			item.GetComponent<Object>().objectType = (Object.Type)2;
			string[] fields = s.TrimStart().Trim('{').TrimStart().Split(';');
			string _name = null;
			int _id = 0;
			string _ammo = null;
			foreach(string f in fields) {
				if(f.Contains("name:"))
					_name = f.Replace("name:",string.Empty).TrimStart();
				else if(f.Contains("id:"))
					_id = System.Int32.Parse(f.Replace("id:",string.Empty).TrimStart());
				else if(f.Contains("ammo:"))
					_ammo = f.Replace("ammo:",string.Empty).TrimStart();
			}
			item.name = _name + " (G)";
			item.tag = "Generic Item";
			item.GetComponent<Object>().name = _name;
			item.GetComponent<Object>().id = _id;
			item.transform.parent = itemParent.transform;
		}
	}
}