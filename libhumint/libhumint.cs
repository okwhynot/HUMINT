using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using libhumint.RNG;
namespace libhumint {
	public class Console : ColorLib {
		//w and h are grid dimensions.
		int w,h;
		Font font;
		public GridSlot[] grid;
		public Console(string filename = "terminal8x14_gs_ro") {
			font = new Font(filename);
			w = Screen.width/font.fx;
			h = Screen.height/font.fy;
			grid = new GridSlot[w*h];
			for(int num = 0; num < w*h; num++)
				grid[num] = new GridSlot(font.fx,font.fy,GetCoords(num));
		}
		public class GridSlot {
			public Vector2 location;
			public int character;
			public Color foreground;
			public Color background;
			//x & y are grid coordinates, not pixel. Width and height, however, are pixel.
			public GridSlot(int Width,int Height,Vector2 loc,Color32 FGColor = new Color32(),Color32 BGColor = new Color32()) {
				location = loc;
				foreground = FGColor;
				background = BGColor;
			}
			public void Set_Character(char charc = ' ',int chari = 0){
				if(charc == ' ' && chari != 0)
					character = chari;
				else if(charc != ' ' && chari == 0)
					character = (int)charc;
				else if(charc == ' ' && chari == 0)
					character = 0;
			}
		}
		public void Render() {
			foreach(GridSlot g in grid)
				if(g.character != 0)
					font.Draw((int)g.location.x,(int)g.location.y,g.foreground,g.background,(char)g.character);
		}
		public void Flush() {
			for(int num = 0; num < w*h; num++)
				grid[num] = new GridSlot(font.fx,font.fy,GetCoords(num));
		}
		//x and y are grid
		public void Put(int x, int y, object obj,Color32 fg = new Color32(),Color32 bg = new Color32()) {
			string type = obj.GetType().ToString();
			if(fg.r == 0 && fg.g == 0 && fg.b == 0 && fg.a == 0)
				fg = White();
			if(type == "System.String")
			{
				string text = (string)obj;
				int init = (y*w) + x;
				int i = 0;
				foreach(char c in text.ToCharArray())
				{
					grid[init+i].character = (int)c;
					grid[init+i].foreground = fg;
					grid[init+i].background = bg;
					i+=1;
				}
			}
			else if(type == "System.Char")
			{
				char text = (char)obj;
				grid[(y*w) + x].character = (int)text;
				grid[(y*w) + x].foreground = fg;
				grid[(y*w) + x].background = bg;
			}
			else if(type == "System.Int32")
			{
				grid[(y*w) + x].character = (int)obj;
				grid[(y*w) + x].foreground = fg;
				grid[(y*w) + x].background = bg;
			}
		}
		public void Blit_To(Console receiver) {
			
		}
		//Trivial operations
		public Vector2 GetCoords(int num) {
			int y = (int)num/w;
			int x = num-(y*w);
			Vector2 loc = new Vector2(x,y);
			return loc;
		}
		public int GetNum(Vector2 coords) {
			int num = 0;
			return num;
		}
	}
	//Rewriting name generator to use TextAssets
	public class NameGenerator {
		public TextAsset source;
		List<string> maleFirst = new List<string>();
		List<string> femaleFirst = new List<string>();
		List<string> last = new List<string>();
		List<string> usedNames = new List<string>();
		public NameGenerator(string civ) {
			source = Resources.Load(civ) as TextAsset;
			string[] sections = source.text.Split(';');
			foreach(string s in sections) {
				if(s.TrimStart().StartsWith("MALE_FIRST"))
				{
					foreach(string name in s.Substring(11).Split(','))
						maleFirst.Add(name.TrimStart().Trim('"'));
				}
				else if(s.TrimStart().StartsWith("FEMALE_FIRST"))
				{
					foreach(string name in s.Substring(15).Split(','))
						femaleFirst.Add(name.TrimStart().Trim('"'));
				}
				else if(s.TrimStart().StartsWith("LAST"))
				{
					foreach(string name in s.Substring(6).Split(','))
						last.Add(name.TrimStart().Trim('"'));
				}
			}
		}
		
		public string Name(int gender) {
			string first = null;
			int mnlen = maleFirst.Count;
			int fnlen = femaleFirst.Count;
			int lnlen = last.Count;
			if(gender == 0)
				first = maleFirst[Random.Range(0,mnlen)];
			else
				first = femaleFirst[Random.Range(0,fnlen)];
			string lastn = last[Random.Range(0,lnlen)];
			string n = first + " " + lastn;
			if(usedNames.Contains(n))
				n = Name(gender);
			else
				usedNames.Add(n);
			return n;
		}
	}
	public class ColorLib {
		//Blank for default, natural, colorblind, cga, alternate
		public static Color Black() {
			Color32 b = new Color32(0,0,0,255);
			return b;
		}
		public static Color Blue(string s = "default") {
			Color32 b = new Color32();
			if(s == "default")
				b = new Color32(13,103,196,255);
			else if(s == "natural")
				b = new Color32(73,95,157,255);
			else if(s == "colorblind")
				b = new Color32(0,0,240,255);
			else if(s == "cga")
				b = new Color32(0,0,170,255);
			else if(s == "alternate")
				b = new Color32(30,85,165,255);
			return b;
		}
		public static Color Green(string s = "default") {
			Color32 g = new Color32();
			if(s == "default")
				g = new Color32(68,158,53,255);
			else if(s == "natural")
				g = new Color32(89,117,55,255);
			else if(s == "colorblind")
				g = new Color32(0,128,0,255);
			else if(s == "cga")
				g = new Color32(0,170,0,255);
			else if(s == "alternate")
				g = new Color32(70,125,55,255);
			return g;
		}
		public static Color Cyan(string s = "default") {
			Color32 c = new Color32();
			if(s == "default")
				c = new Color32(86,163,205,255);
			else if(s == "natural")
				c = new Color32(101,144,158,255);
			else if(s == "colorblind")
				c = new Color32(0,112,144,255);
			else if(s == "cga")
				c = new Color32(0,170,170,255);
			else if(s == "alternate")
				c = new Color32(45,145,135,255);
			return c;
		}
		public static Color Red(string s = "default") {
			Color32 r = new Color32();
			if(s == "default")
				r = new Color32(151,26,26,255);
			else if(s == "natural")
				r = new Color32(146,0,0,255);
			else if(s == "colorblind")
				r = new Color32(240,0,0,255);
			else if(s == "cga")
				r = new Color32(170,0,0,255);
			else if(s == "alternate")
				r = new Color32(170,20,0,255);
			return r;
		}
		public static Color Magenta(string s = "default") {
			Color32 m = new Color32();
			if(s == "default")
				m = new Color32(255,110,187,255);
			else if(s == "natural")
				m = new Color32(165,54,101,255);
			else if(s == "colorblind")
				m = new Color32(160,0,128,255);
			else if(s == "cga")
				m = new Color32(170,0,170,255);
			else if(s == "alternate")
				m = new Color32(130,40,115,225);
			return m;
		}
		public static Color Brown(string s = "default") {
			Color32 b = new Color32();
			if(s == "default")
				b = new Color32(120,94,47,255);
			else if(s == "natural")
				b = new Color32(138,105,59,255);
			else if(s == "colorblind")
				b = new Color32(128,96,0,255);
			else if(s == "cga")
				b = new Color32(170,85,0,255);
			else if(s == "alternate")
				b = new Color32(120,80,50,255);
			return b;
		}
		public static Color LightGray(string s = "default") {
			Color32 lg = new Color32();
			if(s == "default")
				lg = new Color32(185,192,162,255);
			else if(s == "natural")
				lg = new Color32(128,128,128,255);
			else if(s == "colorblind")
				lg = new Color32(208,208,208,255);
			else if(s == "cga")
				lg = new Color32(170,170,170,255);
			else if(s == "alternate")
				lg = new Color32(160,160,160,255);
			return lg;
		}
		public static Color DarkGray(string s = "default") {
			Color32 dg = new Color32();
			if(s == "default")
				dg = new Color32(88,83,86,255);
			else if(s == "natural")
				dg = new Color32(80,80,80,255);
			else if(s == "colorblind")
				dg = new Color32(112,112,112,255);
			else if(s == "cga")
				dg = new Color32(85,85,85,255);
			else if(s == "alternate")
				dg = new Color32(100,100,100,255);
			return dg;
		}
		public static Color LightBlue(string s = "default") {
			Color32 lb = new Color32();
			if(s == "default")
				lb = new Color32(145,202,255,255);
			else if(s == "natural")
				lb = new Color32(111,138,165,255);
			else if(s == "colorblind")
				lb = new Color32(80,80,255,255);
			else if(s == "cga")
				lb = new Color32(85,85,255,255);
			else if(s == "alternate")
				lb = new Color32(90,130,210,255);
			return lb;
		}
		public static Color LightGreen(string s = "default") {
			Color32 lg = new Color32();
			if(s == "default")
				lg = new Color32(131,212,82,255);
			else if(s == "natural")
				lg = new Color32(160,200,82,255);
			else if(s == "colorblind")
				lg = new Color32(0,224,0,255);
			else if(s == "cga")
				lg = new Color32(85,255,85,255);
			else if(s == "alternate")
				lg = new Color32(110,180,55,255);
			return lg;
		}
		public static Color LightCyan(string s = "default") {
			Color32 lc = new Color32();
			if(s == "default")
				lc = new Color32(176,223,215,255);
			else if(s == "natural")
				lc = new Color32(159,196,210,255);
			else if(s == "colorblind")
				lc = new Color32(64,224,255,255);
			else if(s == "cga")
				lc = new Color32(85,255,255,255);
			else if(s == "alternate")
				lc = new Color32(70,215,195,255);
			return lc;
		}
		public static Color LightRed(string s = "default") {
			Color32 lr = new Color32();
			if(s == "default")
				lr = new Color32(255,34,34,255);
			else if(s == "natural")
				lr = new Color32(206,73,1,255);
			else if(s == "colorblind")
				lr = new Color32(255,80,80,255);
			else if(s == "cga")
				lr = new Color32(255,85,85,255);
			else if(s == "alternate")
				lr = new Color32(215,60,0,255);
			return lr;
		}
		public static Color LightMagenta(string s = "default") {
			Color32 lm = new Color32();
			if(s == "default")
				lm = new Color32(255,167,246,255);
			else if(s == "natural")
				lm = new Color32(239,150,207,255);
			else if(s == "colorblind")
				lm = new Color32(255,48,240,255);
			else if(s == "cga")
				lm = new Color32(255,85,255,255);
			else if(s == "alternate")
				lm = new Color32(210,85,190,255);
			return lm;
		}
		public static Color Yellow(string s = "default") {
			Color32 y = new Color32();
			if(s == "default")
				y = new Color32(255,218,90,255);
			else if(s == "natural")
				y = new Color32(255,198,0,255);
			else if(s == "colorblind")
				y = new Color32(255,255,64,255);
			else if(s == "cga")
				y = new Color32(255,255,85,255);
			else if(s == "alternate")
				y = new Color32(235,180,0,255);
			return y;
		}
		public static Color White() {
			Color32 w = new Color32(255,255,255,255);
			return w;
		}
	}
	class Font : ColorLib {
		Texture2D atlas;
		List<Vector2> coordinates = new List<Vector2>();
		public int fx,fy;
		public Font(string filename) {
			atlas = Resources.Load("libhumint/font/"+filename) as Texture2D;
			fx = atlas.width/16;
			fy = atlas.height/16;
			Index_Font();
		}
		//Finds the character in the inside the index.
		public void Draw(int locx, int locy, Color col, Color bgCol, char character = ' ') {
			Texture2D draw = atlas;
			if(col.r == 0 && col.g == 0 && col.b == 0 && col.a == 0)
				col = White();
			if(bgCol.r == 0 && bgCol.g == 0 && bgCol.b == 0 && bgCol.a == 0)
				bgCol = Black();
			Vector2 charLoc = coordinates[System.Convert.ToInt32(character)];
			Vector2 bgLoc = coordinates[System.Convert.ToInt32((char)219)];
			GUI.skin = Resources.Load("libhumint/Font Disp") as GUISkin;
			GUI.BeginGroup(new Rect(locx*fx,locy*fy,fx,fy));
				if(bgCol != Black())
				{
					GUI.contentColor = bgCol;
					GUI.Label(new Rect(-fx*bgLoc.x,-fy*bgLoc.y,fx*16,fy*16),draw);
				}
				GUI.contentColor = col;
				GUI.Label(new Rect(-fx*charLoc.x,-fy*charLoc.y,fx*16,fy*16),draw);
			GUI.EndGroup();
		}
		void Index_Font() {
			for(int y=0;y<16;y++)
				for(int x=0;x<16;x++)
					coordinates.Add(new Vector2(x,y));
		}
	}
	public class Math {
		public bool isOdd(int num) {
			bool isO = false;
			if(num % 2 == 1)
				isO = true;
			else
				isO = false;
			return isO;
		}
		public bool isEven(int num) {
			bool isE = false;
			if(num % 2 == 0)
				isE = true;
			else
				isE = false;
			return isE;
		}
	}
}
namespace libhumint.RNG {
	public class RNG {
		public RNG(int seed) {
			
		}
	}
}
namespace libhumint.AI {
	public class Pathfinder {
	
	}
}