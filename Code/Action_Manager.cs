using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using libhumint;

public class Action_Manager : MonoBehaviour {
	public enum ACTIONSTATE {MOVE,ATTACK,CLOSE,OPEN,EXAMINE,LOOK};
	public ACTIONSTATE state = ACTIONSTATE.MOVE;
	
	void Fire(int x0, int y0, int x1, int y1) {
		
	}
	
	void PickUpObject() {
		
	}
}