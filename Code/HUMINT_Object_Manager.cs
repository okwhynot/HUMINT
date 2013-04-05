using UnityEngine;
using System.Collections;
using libhumint;

public class HUMINT_Object_Manager : MonoBehaviour {
	void Awake() {
		LoadItems();
	}
	
	void LoadItems() {
		TextAsset source = Resources.Load("Items") as TextAsset;
		string[] sections = source.text.Split('}');
		foreach(string s in sections)
		{
			if(s.Contains("name:") == false)
				break;
			GameObject item = (GameObject)Instantiate(Resources.Load("Object") as GameObject);
			string[] fields = s.TrimStart().Trim('{').TrimStart().Split(';');
			string name = null;
			string ammo = null;
			int id = 0;
			int damage = 0;
			int attach = 0;
			foreach(string f in fields)
			{
				if(f.Contains("name:"))
					name = f.Replace("name:",string.Empty).TrimStart();
				else if(f.Contains("id:"))
					id = System.Int32.Parse(f.Replace("id:",string.Empty).TrimStart());
				else if(f.Contains("ammo:"))
					ammo = f.Replace("ammo:",string.Empty).TrimStart();
			}
			item.GetComponent<HUMINT_Object>().Create(name,id);
			item.name = name + " (G)";
			item.tag = "Generic Object";
			item.transform.parent = GameObject.Find("Items").transform;
		}
	}
}