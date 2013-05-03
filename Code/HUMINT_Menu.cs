using UnityEngine;
using System.Collections;
using libhumint;

public class HUMINT_Menu : MonoBehaviour {
	Main_Menu mmenu;
	public Character_Creator cmenu;
	public VisualSet vmenu;
	public Passport ppmenu;
	bool CanContinue = false;
	int curSel;
	public enum Mode {Main,Generate,Visual,Passport,Load,Settings,About,Quit,Game};
	public Mode mode = Mode.Main;
	public NameGenerator aNG,gNG,rNG;
	
	void Awake() {
		gNG = new NameGenerator("German");
		rNG = new NameGenerator("Russian");
		//mmenu = new Main_Menu();
	}
	void Start() {
		
	}
	void Update() {
		switch(mode)
		{
			case Mode.Main:
			{
				mmenu.Selection(this);
				break;
			}
			case Mode.Generate:
			{
				cmenu.Selection(this);
				break;
			}
			case Mode.Visual:
			{
				vmenu.Selection(this);
				break;
			}
			case Mode.Passport:
			{
				ppmenu.Selection(this);
				break;
			}
		}
	}
	void OnGUI() {
		switch(mode)
		{
			case Mode.Main:
			{
				mmenu.DrawUI();
				break;
			}
			case Mode.Generate:
			{
				cmenu.DrawUI();
				break;
			}
			case Mode.Visual:
			{
				vmenu.DrawUI();
				break;
			}
			case Mode.Passport:
			{
				ppmenu.DrawUI();
				break;
			}
		}
	}
}
public class Main_Menu : ColorLib {
	int curSel = 0;
	enum Selected {GenWorld,Continue,Load,Archives,Settings,About,Quit};
	Selected sel;
	Selected[] states = {Selected.GenWorld,Selected.Continue,Selected.Load,Selected.Archives,Selected.Settings,Selected.About,Selected.Quit};
	Console bkg,menu;
	int[] xv = {26,24,24,24,27,25,28};
	int[] yv = {3,5,7,9,11,13,18};
	
	public Main_Menu() {
		bkg = new Console("terminal12x12_gs_ro");
		menu = new Console("terminal12x12_gs_ro");
		sel = Selected.GenWorld;
		SetBkg();
		SetText();
	}
	public void SetBkg() {
		bkg.Put(22,0,UIStrings.title);
		bkg.Put(27,1,UIStrings.subtitle,LightBlue("natural"));
		for(int i=0;i<7;i++)
			bkg.Put(xv[i],yv[i],UIStrings.MMButtons[i]);		
		for(int x=0;x<61;x++)
		{
			//Top/bottom borders
			for(int y=0;y<20;y++)
			{
				if(x==20||x==41)
					bkg.Put(x,y,(char)219,DarkGray());
				else if(x<20)
					bkg.Put(x,y,(char)219,Blue());
				else if(x>41)
					bkg.Put(x,y,(char)219,Red());
				else
					continue;
			}
		}
		//MakeEagle();
		//MakeHammerAndSickle();
	}
	public void SetText() {
		for(int i=0;i<7;i++)
			if(i == curSel)
				menu.Put(xv[i],yv[i],UIStrings.MMButtons[i],fg:LightRed("natural"));
	}
	public void DrawUI() {
		bkg.Render();
		menu.Render();
	}
	public void Selection(HUMINT_Menu parent) {
		if(Input.GetKeyDown("down") && curSel < 6)
		{
			curSel += 1;
			menu.Flush();
			SetText();
		}
		else if(Input.GetKeyDown("up") && curSel > 0)
		{
			curSel -= 1;
			menu.Flush();
			SetText();
		}
		sel = states[curSel];
		switch(sel)
		{
			case Selected.GenWorld:
			{
				if(Input.GetKeyDown("return"))
				{
					parent.cmenu = new Character_Creator();
					parent.mode = HUMINT_Menu.Mode.Generate;
				}
				break;
			}
			case Selected.Continue:
			{
				if(Input.GetKeyDown("return"))
					Debug.Log("Continue!");
				break;
			}
			case Selected.Load:
			{
				if(Input.GetKeyDown("return"))
					Debug.Log("Load!");
				break;
			}
			case Selected.Archives:
			{
				if(Input.GetKeyDown("return"))
					Debug.Log("Archives!");
				break;
			}
			case Selected.Settings:
			{
				if(Input.GetKeyDown("return"))
					Debug.Log("Settings!");
				break;
			}
			case Selected.About:
			{
				if(Input.GetKeyDown("return"))
					Debug.Log("About!");
				break;
			}
			case Selected.Quit:
			{
				if(Input.GetKeyDown("return"))
					Debug.Log("Quit :(");
				break;
			}
		}
	}
	public void MakeEagle() {
		bkg.Put(15,0,"BALD EAGLE",bg:Blue());
	}
	public void MakeHammerAndSickle() {
		bkg.Put(47,0,"SOVIET FLAG",bg:Red());
	}
	public bool isOdd(int num) {
		bool y = false;
		if(num % 2 == 1)
			y = true;
		else
			y = false;
		return y;
	}
}