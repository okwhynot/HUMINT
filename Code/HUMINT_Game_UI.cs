using UnityEngine;
using System.Collections;
using libhumint;

public class Overlay : ColorLib {
	//720x240 is in game res
	public Console Bkg;
	public Console Fg;
	public Console Refreshable;
	public Overlay() {
		Bkg = new Console("terminal12x12_gs_ro");
		Refreshable = Refresh();
		Fg = Foreground();
		SetBorder();
		SetBack();
	}
	void SetBorder() {
		for(int y=0;y<20;y++)
		{
			Bkg.Put(0,y,(char)219,LightGray("natural"));
			Bkg.Put(34,y,(char)219,LightGray("natural"));
			Bkg.Put(47,y,(char)179,LightGray("natural"));
		}
		for(int x=1;x<34;x++)
		{
			Bkg.Put(x,0,(char)219,LightGray("natural"));
			Bkg.Put(x,19,(char)219,LightGray("natural"));
		}
	}
	void SetBack() {
		//Titles
		Bkg.Put(40,0,UIStrings.IGMenuTitles[0]);
		Bkg.Put(50,0,UIStrings.IGMenuTitles[1]);
		//Health
		Bkg.Put(35,1,"HP:");
		Bkg.Put(43,1,'/');
		Bkg.Put(44,1,"100",LightGreen("natural"));
		//Weapon
		Bkg.Put(35,2,"WPN:");
		//Ammo
		Bkg.Put(35,4,"AM:");
		Bkg.Put(43,4,'/');
	}
	public void Draw() {
		Bkg.Render();
		Fg.Render();
		Refreshable = Refresh();
		Refreshable.Render();
	}
	public Console Foreground() {
		Console fg = new Console("terminal12x12_gs_ro");
		fg.Put(48,1,"a."+UIStrings.IGMenu[0]);
		fg.Put(48,2,"c."+UIStrings.IGMenu[1]);
		fg.Put(48,3,"h."+UIStrings.IGMenu[2]);
		fg.Put(48,4,"e."+UIStrings.IGMenu[3]);
		fg.Put(48,5,"i."+UIStrings.IGMenu[4]);
		fg.Put(48,6,"m."+UIStrings.IGMenu[5]);
		fg.Put(48,7,"o."+UIStrings.IGMenu[6]);
		return fg;
	}
	public Console Refresh() {
		Console rf = new Console("terminal12x12_gs_ro");
		//Placeholders, impliment proper
		rf.Put(40,1,"100",LightGreen("natural"));
		rf.Put(40,4,"006",LightBlue("natural"));
		rf.Put(44,4,"006",LightBlue("natural"));
		rf.Put(36,3,"holstered",LightGray("natural"));
		return rf;
	}
}
//Should only refresh on keypress.
public class GameArea : ColorLib {
	public GameObject player;
	public Console DispArea,DispAreaOverlay;
	public GameArea() {
		player = GameObject.FindWithTag("Player");
		DispArea = Display();
		//DispAreaOverlay = GameOverlay();
	}
	//Call on movement, attacking, etc.
	public void Refresh() {
		DispArea = Display();
		//DispAreaOverlay = GameOverlay();
	}
	public void Draw() {
		DispArea.Render();
		//DispAreaOverlay.Render();
	}
	//Impliment player-location based drawing.
	public Console Display() {
		Console da = new Console("terminal12x12_gs_ro");
		GameObject cam = GameObject.Find("Main Camera");
		GameObject player = GameObject.FindWithTag("Player");
		HUMINT_Object p = GameObject.FindWithTag("Player").GetComponent<HUMINT_Object>();
		//33x18
		for(int x=0;x<33;x++)
		{
			for(int y=0;y<18;y++)
			{
				char disp = cam.GetComponent<HUMINT_World>().worldMap[(int)p.curMap.x,(int)p.curMap.y].mTiles[x,y].tileChar;
				Color32 col = cam.GetComponent<HUMINT_World>().worldMap[(int)p.curMap.x,(int)p.curMap.y].mTiles[x,y].tileColor;
				Vector2 cur = new Vector2(x,y);
				if(cur == player.GetComponent<HUMINT_Object>().Coordinates)
					da.Put(x+1,y+1,'@');
				else
					da.Put(x+1,y+1,disp,col);
			}
		}
		//da.Put(17,9,'@');
		return da;
	}
	public Console GameOverlay() {
		Console da = new Console("terminal12x12_gs_ro");

		return da;
	}
	public Vector2 GetCoords(int num,int w) {
		int y = (int)num/w;
		int x = num-(y*w);
		Vector2 loc = new Vector2(x,y);
		return loc;
	}
}