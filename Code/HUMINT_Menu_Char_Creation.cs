using UnityEngine;
using System.Collections;
using libhumint;

public class Character_Creator : ColorLib {
	Console bkg,menu;
	string[] bez,ssn,fac,spc,gen;
	int selMen,selBz,selSn,selFac,selSpc,selGen;
	
	public Character_Creator() {
		menu = new Console("terminal12x12_gs_ro");
		bkg = new Console("terminal12x12_gs_ro");
		bez = UIStrings.Bezirke;
		ssn = UIStrings.Seasons;
		fac = UIStrings.FactionsShort;
		spc = UIStrings.Specializations;
		gen = UIStrings.Gender;
		selMen = 1;
		SetBkg();
		SetText();
	}
	void SetBkg() {
		int[] bxl = {2,17,26,38,46,52,13,28,36};
		int[] byl = {3,3,3,3,3,3,4,4,4};
		int[] fxl = {24,31};
		int[] spxl = {12,20,31,44};
		int[] gxl = {25,30};
		int[] dx = {16,25,37,45,51,26,34};
		int[] dy = {3,3,3,3,3,4,4};
		for(int i=0;i<9;i++)
		{
			bkg.Put(bxl[i],byl[i],bez[i],LightGray("natural"));
			if(i<7)
				bkg.Put(dx[i],dy[i],(char)179,Red("natural"));
			//Seasons and factions
			if(i<2)
				bkg.Put(fxl[i],8,fac[i]);
			if(i<4)
			{
				bkg.Put(17+7*i,6,ssn[i]);
				bkg.Put(spxl[i],10,spc[i]);
			}
			if(i<3)
			{
				bkg.Put(24+7*i-1,6,(char)179,Red("natural"));
				bkg.Put(spxl[i+1]-1,10,(char)179,Red("natural"));
			}
			if(i<2)
				bkg.Put(gxl[i],12,gen[i]);
		}
		bkg.Put(29,12,(char)179,Red("natural"));
		bkg.Put(30,8,(char)179,Red());
		for(int x=0;x<61;x++)
		{
			bkg.Put(x,0,(char)219);
			bkg.Put(x,19,(char)219);
		}
		bkg.Put(27,2,UIStrings.ccsections[0],LightBlue());
		bkg.Put(27,5,UIStrings.ccsections[1],LightBlue());
		bkg.Put(27,7,UIStrings.ccsections[2],LightBlue());
		bkg.Put(23,9,UIStrings.ccsections[3],LightBlue());
		bkg.Put(27,11,UIStrings.ccsections[4],LightBlue());
	}
	void SetText() {
		menu.Put(21,0,(char)174+"SET GAME PARAMATERS"+(char)175,Green("natural"));
		menu.Put(10,19,"<+shift back"+(char)179+"shift+r random"+(char)179+"shift+> next",Black());
		int[] xl = {26,26,26,22,26};
		int[] yl = {2,5,7,9,11};
		for(int i=0;i<5;i++) {	
			if(selMen == i) {
				menu.Put(xl[i],yl[i],(char)174,LightBlue());
				menu.Put(xl[i]+UIStrings.ccsections[i].Length+1,yl[i],(char)175,LightBlue());
			}
		}
		int[] xloc = {2,17,26,38,46,51,13,28,36};
		int[] yloc = {3,3,3,3,3,3,4,4,4};
		int[] snxloc = {17,24,31,38};
		int[] facxloc = {24,31};
		int[] spxl = {12,20,31,44};
		int[] gxl = {25,30};
		//Selected
		for(int i=0;i<9;i++)
		{
			//Bezirk
			if(selBz == i)
				menu.Put(xloc[i],yloc[i],bez[i],Blue());
			//Season
			if(selSn == i)
				menu.Put(snxloc[i],6,ssn[i],Blue());
			//Faction
			if(selFac == i)
				menu.Put(facxloc[i],8,fac[i],Blue());
			//Specialization
			if(selSpc == i)
				menu.Put(spxl[i],10,spc[i],Blue());
			//Gender
			if(selGen == i)
				menu.Put(gxl[i],12,gen[i],Blue());
		}
	}
	public void DrawUI() {
		bkg.Render();
		menu.Render();
	}
	public void Selection(HUMINT_Menu parent) {
		if(Input.GetKeyDown("down") && selMen < 4)
		{
			selMen += 1;
			menu.Flush();
			SetText();
		}
		if(Input.GetKeyDown("up") && selMen > 0)
		{
			selMen -= 1;
			menu.Flush();
			SetText();
		}
		if(Input.GetKeyDown("left"))
		{
			if(selMen == 1 && selSn > 0)
				selSn -= 1;
			else if(selMen == 2 && selFac > 0)
				selFac -= 1;
			else if(selMen == 3 && selSpc > 0)
				selSpc -= 1;
			else if(selMen == 4 && selGen > 0)
				selGen -= 1;
			menu.Flush();
			SetText();
		}
		if(Input.GetKeyDown("right"))
		{
			if(selMen == 1 && selSn < 3)
				selSn += 1;
			else if(selMen == 2 && selFac < 1)
				selFac += 1;
			else if(selMen == 3 && selSpc < 3)
				selSpc += 1;
			else if(selMen == 4 && selGen < 1)
				selGen += 1;
			menu.Flush();
			SetText();
		}
		if(Input.GetKey(KeyCode.RightShift))
		{
			if(Input.GetKeyDown("."))
			{
				parent.vmenu = new VisualSet(selBz,selSn,selFac,selSpc,selGen);
				parent.mode = HUMINT_Menu.Mode.Visual;
				//parent.ppmenu = new Passport(parent.gNG,selGen);
				//parent.mode = HUMINT_Menu.Mode.Passport;
			}
			if(Input.GetKeyDown(","))
				parent.mode = HUMINT_Menu.Mode.Main;
			if(Input.GetKeyDown("r"))
			{
				selSn = Random.Range(0,4);
				selFac = Random.Range(0,2);
				selSpc = Random.Range(0,4);
				selGen = Random.Range(0,2);
				menu.Flush();
				SetText();
			}
		}
	}
}
public class VisualSet : ColorLib {
	//selBz,selSn,selFac,selSpc,selGen
	int selMen,bz,sn,fac,spc,gen;
	int selSCol,selHCol,selECol,selScar,selHStyle,selAge = 25;
	Console bkg,fg,gs;
	Color hairCol;
	Color skinCol;
	Color eyeCol;
	
	public VisualSet(int bzk, int ssn, int fact, int spec, int gend) {
		bkg = Background();
		this.bz = bzk;
		this.sn = ssn;
		this.fac = fact;
		this.spc = spec;
		Debug.Log(spc);
		gen = gend;
		Refresh();
		//this.gen = 1; //for building female and generic settings
		if(gen == 1)
			fg = ForegroundFemale();
		else
			fg = ForegroundMale();
	}
	public void DrawUI() {
		//GS refreshes on keypress
		bkg.Render();
		gs.Render();
		fg.Render();
	}
	public void Selection(HUMINT_Menu parent) {
		if(Input.GetKeyDown("down") && selMen < 5)
		{
			selMen += 1;
			Refresh();
		}
		if(Input.GetKeyDown("up") && selMen > 0)
		{
			selMen -= 1;
			Refresh();
		}
		if(Input.GetKeyDown("left"))
		{
			if(selMen == 0 && selSCol > 0)
				selSCol -= 1;
			else if(selMen == 1 && selHStyle > 0)
				selHStyle -= 1;
			else if(selMen == 2 && selHCol > 0)
				selHCol -= 1;
			else if(selMen == 3 && selECol > 0)
				selECol -= 1;
			else if(selMen == 4 && selScar > 0)
				selScar -= 1;
			else if(selMen == 5 && selAge > 25)
				selAge -= 1;
			Refresh();
		}
		if(Input.GetKeyDown("right"))
		{
			if(selMen == 0 && selSCol < 3)
				selSCol += 1;
			else if(selMen == 1 && selHStyle < 1)
				selHStyle += 1;
			else if(selMen == 2 && selHCol < 3)
				selHCol += 1;
			else if(selMen == 3 && selECol < 3)
				selECol += 1;
			else if(selMen == 4 && selScar < 3)
				selScar += 1;
			else if(selMen == 5 && selAge < 49)
				selAge += 1;
			Refresh();
		}
		if(Input.GetKey(KeyCode.RightShift))
		{
			if(Input.GetKeyDown("."))
			{
				int[] ccvals = {bz,sn,fac,spc,gen};
				int[] vcvals = {selSCol,selHCol,selECol,selScar,selHStyle,selAge};
				
				//string name, int[] ccval, int[] vsval
				

				parent.ppmenu = new Passport(parent.gNG,gen,ccvals,vcvals);
				parent.mode = HUMINT_Menu.Mode.Passport;
			}
			if(Input.GetKeyDown(","))
				parent.mode = HUMINT_Menu.Mode.Generate;
			if(Input.GetKeyDown("r"))
			{
				
			}
		}
	}
	void Refresh() {
		Color32[] skinC = {new Color32(230,200,145,255),new Color32(213,179,135,255),new Color32(200,163,122,255),new Color32(173,134,98,255)};
		//131,91,79 | 247,238,127 | 112,108,104 | 200,91,82
		Color32[] hairC = {new Color32(131,91,79,255),new Color32(247,238,127,255),new Color32(112,108,104,255),new Color32(200,91,82,255)};
		//158,164,172 | 163,221,126 | 139,120,71| 160,170,170
		Color32[] eyeC = {new Color32(158,164,172,255),LightGreen("natural"),new Color32(139,120,71,255),new Color32(160,170,170,255)};
		gs = GenericSettings();
		skinCol = skinC[selSCol];
		hairCol = hairC[selHCol];
		eyeCol = eyeC[selECol];
		if(gen == 1)
			fg = ForegroundFemale();
		else if(gen == 0)
			fg = ForegroundMale();
	}
	public Console Background() {
		Console bg = new Console("terminal12x12_gs_ro");
		for(int x=0;x<61;x++)
		{
			bg.Put(x,0,(char)219);
			bg.Put(x,19,(char)219);
		}
		for(int y=0;y<20;y++)
			bg.Put(42,y,(char)219);
		//SETTINGS
		for(int i=0;i<UIStrings.VisSections.Length;i++)
		{
			if(i<5)
				bg.Put(3,2+2*i+1*i,UIStrings.VisSections[i]);
			else
				bg.Put(15,2+2*(i-5)+1*(i-5),UIStrings.VisSections[i]);
		}
		return bg;
	}
	public Console GenericSettings() {
		Console gs = new Console("terminal12x12_gs_ro");
		int[] options = {selSCol,selHStyle,selHCol,selECol,selScar,selAge};
		//SELECTED MENU
		for(int i=0;i<UIStrings.VisSections.Length;i++)
		{
			if(selMen == i)
			{
				if(i<5)
				{
					gs.Put(2,2+2*i+1*i,(char)174,LightBlue());
					gs.Put(3,2+2*i+1*i,UIStrings.VisSections[i],LightBlue());
					gs.Put(3+UIStrings.VisSections[i].Length,2+2*i+1*i,(char)175,LightBlue());
				}
				else
				{
					gs.Put(14,2+2*(i-5)+1*(i-5),(char)174,LightBlue());
					gs.Put(15,2+2*(i-5)+1*(i-5),UIStrings.VisSections[i],LightBlue());
					gs.Put(15+UIStrings.VisSections[i].Length,2+2*(i-5)+1*(i-5),(char)175,LightBlue());
				}
			}
			int o = options[i];
			if(i!=4 && i!=5)
				o = options[i] + 1;
			if(i<5)
				gs.Put(3,3+2*i+1*i,o.ToString());
			else
				gs.Put(15,3+2*(i-5)+1*(i-5),o.ToString());
		}
		
		gs.Put(10,19,"<+shift back"+(char)179+"shift+r random"+(char)179+"shift+> next",Black());
		return gs;
	}
	public Console ForegroundMale() {
		Color32 hsBkg = Black();
		Console fg = new Console("terminal12x12_gs_ro");
		//Head
		for(int x=45;x<50;x++)
		{
			for(int y=3;y<9;y++)
				fg.Put(x,y,(char)219,skinCol);
		}
		Color32 eyebCol = new Color32(221,221,221,255);
		fg.Put(46,9,(char)222,skinCol); //NECK
		fg.Put(47,9,(char)219,skinCol); //NECK
		fg.Put(47,9,(char)227,Black(),skinCol);
		fg.Put(48,9,(char)221,skinCol); //NECK
		fg.Put(46,10,(char)232,Black(),skinCol); //TORSO -- BLACK IS DEBUG. REPLACE W/ JACKET COLOR.
		fg.Put(47,10,(char)219,skinCol); //TORSO
		fg.Put(48,10,(char)229,Black(),skinCol); //TORSO
		fg.Put(45,3,(char)222,skinCol);
		fg.Put(49,3,(char)227,Black(),skinCol); //HEAD ROUNDING
		fg.Put(45,8,(char)232,Black(),skinCol); //HEAD ROUNDING
		fg.Put(49,8,(char)229,Black(),skinCol); //HEAD ROUNDING
		fg.Put(46,4,(char)229,eyeCol,eyebCol); //L-EYE
		fg.Put(48,4,(char)229,eyeCol,eyebCol); //R-EYE
		fg.Put(46,5,(char)223,eyebCol,skinCol); //L-EYE
		fg.Put(48,5,(char)223,eyebCol,skinCol); //R-EYE
		fg.Put(46,7,(char)220,Black(),skinCol); //MOUTH
		fg.Put(47,7,(char)220,Black(),skinCol); //MOUTH
		//Hair
		if(selHStyle == 0)
		{
			//for(int x=45;x<49;x++)
			//	fg.Put(x,2,(char)220,hairCol);
			//fg.Put(49,2,(char)232,hairCol);
		}
		else if(selHStyle == 1)
		{
			
		}
		//Scars
		Color32 scarCol = new Color32(240,210,155,255);
		if(selScar == 1) //L-EYE
		{
			fg.Put(46,3,(char)229,scarCol,skinCol);
			fg.Put(45,5,(char)229,scarCol,skinCol);
		}
		else if(selScar == 2) //EYEPATCH
		{
			fg.Put(47,3,(char)230,skinCol,Black()); //STRAP
			fg.Put(45,5,(char)229,Black(),skinCol); //STRAP
			fg.Put(45,6,(char)226,Black(),skinCol); //STRAP
			fg.Put(46,4,(char)219,Black()); //PATCH
			fg.Put(46,5,(char)223,Black(),skinCol); //PATCH
		}
		else if(selScar == 3) //R-CHEEK
			fg.Put(48,6,(char)230,scarCol,skinCol);
		return fg;
	}
	public Console ForegroundFemale() {
		Console fg = new Console("terminal12x12_gs_ro");
		Color32 hsBkg = Black();
		//Hair
		if(selHStyle == 0)
		{
			hsBkg = hairCol;
			for(int x=44;x<51;x++)
			{
				for(int y=2;y<10;y++)
					fg.Put(x,y,(char)219,hairCol);
				fg.Put(x,2,(char)220,hairCol);
			}
			for(int y=3;y<10;y++)
			{
				fg.Put(44,y,(char)231,hairCol);
				fg.Put(50,y,(char)221,hairCol);
			}
			fg.Put(44,2,(char)229,hairCol);
			fg.Put(50,2,(char)232,hairCol);
		}
		else if(selHStyle == 1)
		{
			for(int x=44;x<51;x++)
			{
				fg.Put(x,2,(char)220,hairCol);
			}
			for(int y=3;y<8;y++)
			{
				fg.Put(44,y,(char)231,hairCol);
				fg.Put(50,y,(char)221,hairCol);
			}
		}
		//Head
		for(int x=45;x<50;x++)
			for(int y=3;y<9;y++)
				fg.Put(x,y,(char)219,skinCol); //HEAD
		fg.Put(46,9,(char)222,skinCol,hsBkg); //NECK
		fg.Put(47,9,(char)219,skinCol); //NECK
		fg.Put(48,9,(char)221,skinCol,hsBkg); //NECK
		//Face
		Color32 eyebCol = new Color32(221,221,221,255);
		fg.Put(44,2,(char)229,Black()); //HAIR ROUNDING
		fg.Put(50,2,(char)232,Black()); //HAIR ROUNDING
		fg.Put(45,3,(char)229,skinCol,hairCol); //HAIR STRAND
		fg.Put(45,4,(char)226,hairCol,skinCol); //HAIR STRAND
		fg.Put(46,4,(char)229,eyeCol,eyebCol); //L-EYE
		fg.Put(48,4,(char)229,eyeCol,eyebCol); //R-EYE
		fg.Put(46,5,(char)223,eyebCol,skinCol); //L-EYE
		fg.Put(48,5,(char)223,eyebCol,skinCol); //R-EYE
		fg.Put(46,7,(char)220,Black(),skinCol); //MOUTH
		fg.Put(47,7,(char)220,Black(),skinCol); //MOUTH
		fg.Put(49,3,(char)227,hairCol,skinCol); //HEAD ROUNDING
		fg.Put(45,8,(char)232,hsBkg,skinCol); //HEAD ROUNDING
		fg.Put(49,8,(char)229,hsBkg,skinCol); //HEAD ROUNDING
		//Torso
		Color32 jacketCol = new Color32(127,70,44,255);
		for(int x=44;x<51;x++)
			for(int y=10;y<18;y++)
				fg.Put(x,y,(char)219,jacketCol); //JACKET
		fg.Put(46,10,(char)232,jacketCol,skinCol); //TORSO
		fg.Put(47,10,(char)219,skinCol); //TORSO
		fg.Put(48,10,(char)229,jacketCol,skinCol); //TORSO
		for(int y=11;y<18;y++)
		{
			fg.Put(47,y,(char)186,Black(),jacketCol); //ZIPPER
			fg.Put(43,y,(char)219,jacketCol); //LEFT ARM
			fg.Put(51,y,(char)219,jacketCol); //RIGHT ARM
			fg.Put(52,y,(char)221,jacketCol); //RIGHT ARM
		}
		fg.Put(43,10,(char)226,Black(),jacketCol); //LEFT ARM
		fg.Put(43,18,(char)219,jacketCol); //LEFT ARM
		fg.Put(51,10,(char)227,Black(),jacketCol); //RIGHT ARM
		fg.Put(51,18,(char)219,jacketCol); //RIGHT ARM
		fg.Put(52,18,(char)221,jacketCol); //RIGHT ARM
		//Pants
		for(int x=44;x<51;x++)
			fg.Put(x,18,(char)219,Blue("natural"));
		//Scars
		Color32 scarCol = new Color32(240,210,155,255);
		if(selScar == 1) //L-EYE
		{
			fg.Put(46,3,(char)229,scarCol,skinCol);
			fg.Put(45,5,(char)229,scarCol,skinCol);
		}
		else if(selScar == 2) //EYEPATCH
		{
			fg.Put(47,3,(char)230,skinCol,Black()); //STRAP
			fg.Put(45,5,(char)229,Black(),skinCol); //STRAP
			fg.Put(45,6,(char)226,Black(),skinCol); //STRAP
			fg.Put(46,4,(char)219,Black()); //PATCH
			fg.Put(46,5,(char)223,Black(),skinCol); //PATCH
		}
		else if(selScar == 3) //R-CHEEK
			fg.Put(48,6,(char)230,scarCol,skinCol);
		return fg;
	}
}
public class Passport : ColorLib {
	Console bkg,bkg_fg,bkg_mg,bkg_blk,bkg_bord;
	Console menu;
	string name,fname,lname,dateText;
	int gender;
	GameObject p;
	Color32[] skinC = {new Color32(230,200,145,255),new Color32(213,179,135,255),new Color32(200,163,122,255),new Color32(173,134,98,255)};
	Color32[] hairC = {new Color32(131,91,79,255),new Color32(247,238,127,255),new Color32(112,108,104,255),new Color32(200,91,82,255)};
	Color32[] eyeC = {new Color32(158,164,172,255),LightGreen("natural"),new Color32(139,120,71,255),new Color32(160,170,170,255)};
	Color skinCol;
	Color hairCol;
	Color eyeCol;
	//bz,sn,fac,spc,gen;
	int[] ccvals;
	//selSCol,selHCol,selECol,selScar,selHStyle,selAge = 25;
	int[] vcvals;
	public Passport(NameGenerator ng, int gen,int[] ccv,int[] vcv) {
		string nam = ng.Name(gen);
		gender = gen;
		skinCol = skinC[vcv[0]];
		hairCol = hairC[vcv[1]];
		eyeCol = eyeC[vcv[2]];
		//bkg = new Console();
		//bkg_fg = new Console();
		p = GameObject.FindWithTag("Player");
		ccvals = ccv;
		vcvals = vcv;
		bkg = Background();
		bkg_mg = new Console();
		bkg_blk = new Console();
		bkg_bord = new Console();
		menu = new Console();

		char[] na = nam.ToCharArray();
		for(int i=0;i<na.Length;i++)
		{
			if(na[i] == 'ä')
				na[i] = (char)132;
			else if(na[i] == 'ü')
				na[i] = (char)129;
			else if(na[i] == 'ö')
				na[i] = (char)148;
		}
		name = new string(na);
		p.GetComponent<HUMINT_Object>().Create(name,10000,ccvals,vcvals,new Vector2(0,0));
		System.DateTime dobt = p.GetComponent<HUMINT_Object>().player.dob;
		int day = dobt.Day;
		int month = dobt.Month;
		int year = dobt.Year;
		dateText = day + "/" + month + "/" + year;
		string[] fnln = name.Split(' ');
		fname = fnln[0];
		lname = fnln[1];
		bkg_fg = Text();
		//SetBkg();
	}
	public void Selection(HUMINT_Menu parent) {
		if(Input.GetKey(KeyCode.RightShift))
		{
			if(Input.GetKeyDown("."))
			{
				MonoBehaviour.DontDestroyOnLoad(p);
				MonoBehaviour.DontDestroyOnLoad(GameObject.Find("Items"));
				Application.LoadLevel(1);
			}
		}
	}
	void SetBkg() {
		Color32 papCol = new Color32(245,244,219,255);
		for(int x=2;x<52;x++)
		{
			bkg.Put(x,0,(char)220,papCol);
			bkg.Put(x,16,(char)228,papCol);
			for(int y=0;y<17;y++)
			{
				if(x==2 && (y!=0||y!=16))
					bkg.Put(x,y,(char)222,papCol);
				else if(x==51)
					bkg.Put(x,y,(char)221,papCol);
				else if(y!=0 && y!=16)
					bkg.Put(x,y,(char)219,papCol);
				if(x==27)
				{
					bkg_mg.Put(x,y,(char)179,new Color32(238,229,139,255));
					if(isOdd(y) == false)
						bkg_fg.Put(x,y,'|',new Color32(187,181,107,255));
				}
				if(x>2 && x < 51)
				{
					bkg_bord.Put(x,0,(char)196,Brown("cga"));
					bkg_bord.Put(x,16,(char)196,Brown("cga"));
				}
			}
			if(x>4 && x<25)
			{
				bkg_blk.Put(x,1,'_',Black());
				bkg_blk.Put(x,3,'_',Black());
				bkg_blk.Put(x,5,'_',Black());
				bkg_blk.Put(x,7,'_',Black());
				bkg_blk.Put(x,9,'_',Black());
				bkg_blk.Put(x,15,'_',Black());
			}
			
		}
		for(int y=1;y<16;y++)
		{
			bkg_bord.Put(2,y,(char)179,Brown("cga"));
			bkg_bord.Put(51,y,(char)179,Brown("cga"));
		}
		//Corners
		bkg.Put(2,0,(char)229,papCol);
		bkg.Put(51,0,(char)232,papCol);
		bkg.Put(2,16,(char)227,papCol);
		bkg.Put(51,16,(char)226,papCol);
		bkg_blk.Put(27,0,(char)228,Black());
		bkg_blk.Put(27,16,(char)220,Black());
		
		bkg_bord.Put(2,0,(char)218,Brown("cga"));
		bkg_bord.Put(51,0,(char)191,Brown("cga"));	
		bkg_bord.Put(2,16,(char)192,Brown("cga"));
		bkg_bord.Put(51,16,(char)217,Brown("cga"));

		//Placeholder contents
		bkg_fg.Put(6,1,lname,Black());
		bkg_fg.Put(5,2,"geburtsname",LightGray("natural"));
		bkg_fg.Put(6,3,fname,Black());
		bkg_fg.Put(5,4,"vorname",LightGray("natural"));
		bkg_fg.Put(6,5,"19 December 1994",Black());
		bkg_fg.Put(5,6,"geburtsdatum",LightGray("natural"));
		bkg_fg.Put(6,7,"BERLIN",Black());
		bkg_fg.Put(5,8,"geburtsort",LightGray("natural"));
		bkg_fg.Put(6,9,"UNVERHEIRATET",Black());
		bkg_fg.Put(5,10,"familienstand",LightGray("natural"));
	}
	public Console Background() {
		Console bg = new Console("terminal12x12_gs_ro");
		Color32 papCol = new Color32(245,244,219,255);
		for(int x=1;x<32;x++) //BACKGROUND & BORDERS
			for(int y=1;y<19;y++)
			{
				bg.Put(x,y,219,papCol); //BACKGROUND
				if(x==16)
					bg.Put(x,y,'|',Black(),papCol); //STITCHING
				if(y==1)
					bg.Put(x,y,220,Brown()); //HORIZ BORDER
				if(y==18)
					bg.Put(x,y,228,Brown()); //HORIZ BORDER
				if(x==1)
					bg.Put(x,y,221,Black(),Brown()); //VERT BORDER
				if(x==31)
					bg.Put(x,y,221,Brown(),Black()); //VERT BORDER
				
				if(x>16 && x < 24 && y > 2 && y < 12) //PORTRAIT BG
					bg.Put(x,y,219,LightGray("natural")); //PORTRAIT BG
				if(x>16 && x < 24 && y == 2)
					bg.Put(x,y,220,LightGray("natural"),papCol); //PORTRAIT BG
			}
		Color32 eyebCol = new Color32(221,221,221,255);
		//PORTRAIT -- GENDER NEUTRAL
		for(int x=18;x<23;x++)
			for(int y=4;y<10;y++)
				bg.Put(x,y,219,skinCol); //SKIN
		bg.Put(19,10,222,skinCol,LightGray("natural")); //NECK
		bg.Put(20,10,219,skinCol); //NECK
		bg.Put(21,10,221,skinCol,LightGray("natural")); //NECK
		bg.Put(19,11,232,LightBlue(),skinCol); //TORSO
		bg.Put(20,11,219,skinCol); //TORSO
		bg.Put(21,11,229,LightBlue(),skinCol); //TORSO
		bg.Put(17,11,219,LightBlue());
		bg.Put(18,11,219,LightBlue()); //SHIRT
		bg.Put(22,11,219,LightBlue()); //SHIRT
		bg.Put(23,11,219,LightBlue());
		bg.Put(18,9,232,LightGray("natural"),skinCol); //HEAD ROUNDING
		bg.Put(22,9,229,LightGray("natural"),skinCol); //HEAD ROUNDING
		
		bg.Put(19,5,232,eyeCol,eyebCol); //L-EYE
		bg.Put(21,5,232,eyeCol,eyebCol); //R-EYE
		bg.Put(19,6,223,eyebCol,skinCol); //L-EYE
		bg.Put(21,6,223,eyebCol,skinCol); //R-EYE
		
		bg.Put(20,8,220,Black(),skinCol); //MOUTH
		bg.Put(21,8,220,Black(),skinCol); //MOUTH
		Color32 scarCol = new Color32(240,210,155,255);
		if(gender == 1) //PORTRAIT -- FEMALE
		{
			if(vcvals[4] == 0) //HAIR STYLE 1
			{
				for(int x=18;x<23;x++)
					bg.Put(x,3,(char)220,hairCol,LightGray("natural")); //TOP GENERAL
				for(int y=4;y<11;y++)
				{
					bg.Put(17,y,(char)222,hairCol,LightGray("natural")); //SIDES
					bg.Put(23,y,(char)221,hairCol,LightGray("natural")); //SIDES
				}
				bg.Put(18,9,(char)232,hairCol,skinCol); //HEAD ROUNDING W/ HAIR
				bg.Put(18,10,(char)219,hairCol); //L-NECK FILL
				bg.Put(19,10,(char)222,skinCol,hairCol); //NECK W/ HAIR
				bg.Put(21,10,(char)221,skinCol,hairCol); //NECH W/ HAIR
				bg.Put(22,10,(char)219,hairCol); //R-NECK FILL
				bg.Put(22,9,(char)229,hairCol,skinCol); //HEAD ROUNDING W/ HAIR
				bg.Put(22,5,(char)227,hairCol,skinCol); //HAIR STRAND
				bg.Put(22,4,(char)232,skinCol,hairCol); //HAIR STRAND
				bg.Put(18,4,(char)226,hairCol,skinCol); //HAIR STRAND
			}
			else if(vcvals[4] == 1) //HAIR STYLE 2
			{
				for(int x=18;x<23;x++)
					bg.Put(x,3,(char)220,hairCol,LightGray("natural")); //TOP GENERAL
				for(int y=4;y<8;y++)
				{
					bg.Put(17,y,(char)222,hairCol,LightGray("natural")); //SIDES
					bg.Put(23,y,(char)221,hairCol,LightGray("natural")); //SIDES
				}
				bg.Put(22,5,(char)227,hairCol,skinCol); //HAIR STRAND
				bg.Put(22,4,(char)232,skinCol,hairCol); //HAIR STRAND
				bg.Put(18,4,(char)226,hairCol,skinCol); //HAIR STRAND
			}
			if(vcvals[3] == 1) //SCAR 1
			{
				bg.Put(21,4,232,scarCol,skinCol);
				bg.Put(22,6,232,scarCol,skinCol);
			}
			else if(vcvals[3] == 2) //SCAR 2
			{
				bg.Put(20,4,230,Black(),skinCol); //STRAP
				bg.Put(22,6,232,Black(),skinCol); //STRAP
				bg.Put(22,7,227,Black(),skinCol); //STRAP
				bg.Put(21,5,219,Black()); //PATCH
				bg.Put(21,6,223,Black(),skinCol); //PATCH
			}
			else if(vcvals[3] == 3) //SCAR 3
			{
				bg.Put(19,7,(char)230,skinCol,scarCol);
			}
		}
		else //PORTRAIT -- MALE
		{
			
		}
		for(int field=0;field<6;field++) //FIELD NAMES
			bg.Put(2,2+2*field,UIStrings.FieldNames[field],LightGray("natural"),papCol);
		
		for(int x=35;x<55;x++) //BORDER + BACKGROUND OF DOSSIER
			for(int y=1;y<19;y++)
				bg.Put(x,y,219,papCol);
		bg.Put(36,1,UIStrings.cfied,Red("natural"),papCol);
		bg.Put(36,2,UIStrings.DossierFieldNames[0],LightGray("natural"),papCol);
		bg.Put(36,4,UIStrings.DossierFieldNames[1],LightGray("natural"),papCol);
		bg.Put(36,8,UIStrings.DossierFieldNames[2],LightGray("natural"),papCol);
		return bg;
	}
	public Console Text() {
		Console tx = new Console("terminal12x12_gs_ro");
		tx.Put(3,3,lname,Black());
		tx.Put(3,5,fname,Black());
		tx.Put(3,7,UIStrings.GermanGenders[gender],Black());
		tx.Put(3,9,dateText,Black());
		tx.Put(3,11,UIStrings.CityNames[p.GetComponent<HUMINT_Object>().player.cob],Black());
		tx.Put(3,13,"Unverheiratet",Black());
		tx.Put(37,3,UIStrings.Specializations[ccvals[3]],Black());
		int invCount = 0;
		foreach(GameObject[] go in p.GetComponent<HUMINT_Object>().player.playerInventory.contents)
		{
			if(go[0]!=null)
				invCount += 1;
		}
		Debug.Log(invCount);
		for(int i=0;i<invCount;i++)
		{
			int cToDisp = 0;
			//determine occupied
			for(int x=0;x<10;x++)
				if(p.GetComponent<HUMINT_Object>().player.playerInventory.contents[i][x] != null)
					cToDisp+=1;
			string end = null;
			if(cToDisp > 1)
				end = " x"+cToDisp;
			tx.Put(37,5+i,p.GetComponent<HUMINT_Object>().player.playerInventory.contents[i][0].name + end,Black());
		}
		return tx;
	}
	public void DrawUI() {
		bkg.Render();
		bkg_mg.Render();
		bkg_fg.Render();
		bkg_blk.Render();
		bkg_bord.Render();
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