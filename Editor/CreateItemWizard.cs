using UnityEngine;
using UnityEditor;
using libhumint;

public class CreateItemWizard : ScriptableWizard {
	
	[MenuItem("HUMINT/Create Item")]
	static void CreateItem () {
		ScriptableWizard.DisplayWizard<CreateItemWizard>("Create Item", "Create");
	}
	
	void OnItemCreate() {
		GameObject item = new GameObject("Item");
	}
}