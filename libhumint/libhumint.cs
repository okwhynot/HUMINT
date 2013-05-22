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
		public struct GridSlot {
			public Vector2 location;
			public int character;
			public Color foreground;
			public Color background;
			public int bgcharacter;
			//x & y are grid coordinates, not pixel. Width and height, however, are pixel.
			public GridSlot(int Width,int Height,Vector2 loc,Color32 FGColor = new Color32(),Color32 BGColor = new Color32(),char BGChar = (char)219) {
				location = loc;
				character = (char)0;
				foreground = FGColor;
				background = BGColor;
				bgcharacter = BGChar;
			}
		}
		public void Render() {
			foreach(GridSlot g in grid)
				if(g.character != 0)
					font.Draw((int)g.location.x,(int)g.location.y,g.foreground,g.background,(char)g.character,(char)g.bgcharacter);
		}
		public void Flush() {
			for(int num = 0; num < w*h; num++)
				grid[num] = new GridSlot(font.fx,font.fy,GetCoords(num));
		}
		//x and y are grid
		public void Put(int x, int y, object obj,Color32 fg = new Color32(),Color32 bg = new Color32(),char bgchar = (char)219) {
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
					grid[init+i].bgcharacter = bgchar;
					i+=1;
				}
			}
			else if(type == "System.Char")
			{
				char text = (char)obj;
				grid[(y*w) + x].character = (int)text;
				grid[(y*w) + x].foreground = fg;
				grid[(y*w) + x].background = bg;
				grid[(y*w) + x].bgcharacter = bgchar;
			}
			else if(type == "System.Int32")
			{
				grid[(y*w) + x].character = (int)obj;
				grid[(y*w) + x].foreground = fg;
				grid[(y*w) + x].background = bg;
				grid[(y*w) + x].bgcharacter = bgchar;
			}
		}
		public void Recolor(int x, int y, Color32 fg, Color32 bg = new Color32()) {
			grid[(y*w) + x].foreground = fg;
			if(bg.r != 0 && bg.g != 0 && bg.g != 0 && bg.a != 0)
				grid[(y*w) + x].background = bg;
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
					foreach(string name in s.Substring(11).Split(',')) {
						List<char> accentedName = new List<char>();
						foreach(char c in name.ToCharArray())
							accentedName.Add(CharOps.ToASCII(c));
						string toAdd = new string(accentedName.ToArray());
						maleFirst.Add(toAdd.TrimStart().Trim('"'));
					}
				}
				else if(s.TrimStart().StartsWith("FEMALE_FIRST"))
				{
					foreach(string name in s.Substring(15).Split(',')) {
						List<char> accentedName = new List<char>();
						foreach(char c in name.ToCharArray())
							accentedName.Add(CharOps.ToASCII(c));
						string toAdd = new string(accentedName.ToArray());
						femaleFirst.Add(toAdd.TrimStart().Trim('"'));
					}
				}
				else if(s.TrimStart().StartsWith("LAST"))
				{
					foreach(string name in s.Substring(6).Split(',')) {
						List<char> accentedName = new List<char>();
						foreach(char c in name.ToCharArray())
							accentedName.Add(CharOps.ToASCII(c));
						string toAdd = new string(accentedName.ToArray());
						last.Add(toAdd.TrimStart(':').TrimStart().Trim('"'));
					}
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
		public int fx,fy; //fx = character width, fy = character height
		public Font(string filename) {
			atlas = Resources.Load("font/"+filename) as Texture2D;
			fx = atlas.width/16;
			fy = atlas.height/16;
			Index_Font();
		}
		//Finds the character in the inside the index.
		public void Draw(int locx, int locy, Color col, Color bgCol, char character = ' ', char bgchar = ' ') {
			Texture2D draw = atlas;
			if(col.r == 0 && col.g == 0 && col.b == 0 && col.a == 0)
				col = White();
			Vector2 charLoc = coordinates[System.Convert.ToInt32(character)];
			Vector2 bgLoc = coordinates[System.Convert.ToInt32(bgchar)];
			GUI.skin = Resources.Load("font/Font Disp") as GUISkin;
			GUI.BeginGroup(new Rect(locx*fx,locy*fy,fx,fy));
				if(bgCol != new Color32(0,0,0,0))
				{
					GUI.contentColor = bgCol;
					GUI.Label(new Rect(-fx*bgLoc.x,-fy*bgLoc.y,fx*16,fy*16),draw);
				}
				GUI.contentColor = col;
				if(character != ' ')
					GUI.Label(new Rect(-fx*charLoc.x,-fy*charLoc.y,fx*16,fy*16),draw);
			GUI.EndGroup();
		}
		void Index_Font() {
			for(int y=0;y<16;y++)
				for(int x=0;x<16;x++)
					coordinates.Add(new Vector2(x,y));
		}
	}
	public class Line {
		private List<Point> points;
		public List<Point> getPoints() {return points;}
		public Line(int x0, int y0, int x1, int y1) {
			points = new List<Point>();
		
			int dx = System.Math.Abs(x1-x0);
			int dy = System.Math.Abs(y1-y0);
		
			int sx = x0 < x1 ? 1 : -1;
			int sy = y0 < y1 ? 1 : -1;
			int err = dx-dy;
		
			while(true) {
				points.Add(new Point(x0, y0, 0));
				if(x0==x1 && y0==y1)
					break;
				int e2 = err*2;
				if(e2 > -dx) {
					err -= dy;
					x0 += sx;
				}
				if(e2 < dx) {
					err += dx;
					y0 += sy;
				}
			}
		}
		public class Point {
			public int x,y,z;
			public Point(int x, int y, int z) {
				this.x = x;
				this.y = y;
				this.z = z;
			}
		}
	}
	public static class CharOps {
		public static char ToASCII(char c) {
			int n = (int)c;
			if(n == 9836)
				c = (char)14;
			else if(n == 199)
				c = (char)128;
			else if(n == 252)
				c = (char)129;
			else if(n == 233)
				c = (char)130;
			else if(n == 226)
				c = (char)131;
			else if(n == 228)
				c = (char)132;
			else if(n == 224)
				c = (char)133;
			else if(n == 231)
				c = (char)134;
			else if(n == 231)
				c = (char)135;
			else if(n == 234)
				c = (char)136;
			else if(n == 235)
				c = (char)137;
			else if(n == 232)
				c = (char)138;
			else if(n == 239)
				c = (char)139;
			else if(n == 238)
				c = (char)140;
			else if(n == 236)
				c = (char)141;
			else if(n == 196)
				c = (char)142;
			else if(n == 197)
				c = (char)143;
			else if(n == 201)
				c = (char)144;
			else if(n == 230)
				c = (char)145;
			else if(n == 198)
				c = (char)146;
			else if(n == 244)
				c = (char)147;
			else if(n == 246)
				c = (char)148;
			else if(n == 242)
				c = (char)149;
			else if(n == 251)
				c = (char)150;
			else if(n == 249)
				c = (char)151;
			else if(n == 255)
				c = (char)152;
			else if(n == 214)
				c = (char)153;
			else if(n == 220)
				c = (char)154;
			else if(n == 162)
				c = (char)155;
			else if(n == 163)
				c = (char)156;
			else if(n == 165)
				c = (char)157;
			else if(n == 402)
				c = (char)159;
			else if(n == 224)
				c = (char)160;
			else if(n == 237)
				c = (char)161;
			else if(n == 243)
				c = (char)162;
			else if(n == 250)
				c = (char)163;
			else if(n == 241)
				c = (char)164;
			else if(n == 209)
				c = (char)165;
			else if(n == 170)
				c = (char)166;
			else if(n == 186)
				c = (char)167;
			else if(n == 191)
				c = (char)168;
			else if(n == 8976)
				c = (char)169;
			else if(n == 172)
				c = (char)170;
			else if(n == 189)
				c = (char)171;
			else if(n == 190)
				c = (char)172;
			else if(n == 161)
				c = (char)173;
			else if(n == 171)
				c = (char)174;
			else if(n == 187)
				c = (char)175;
			else if(n == 9617)
				c = (char)176;
			else if(n == 9618)
				c = (char)177;
			else if(n == 9619)
				c = (char)178;
			else if(n == 9474)
				c = (char)179;
			else if(n == 9508)
				c = (char)180;
			else if(n == 9569)
				c = (char)181;
			else if(n == 9570)
				c = (char)182;
			else if(n == 9558)
				c = (char)183;
			else if(n == 9557)
				c = (char)184;
			else if(n == 9571)
				c = (char)185;
			else if(n == 9553)
				c = (char)186;
			else if(n == 9559)
				c = (char)187;
			else if(n == 9565)
				c = (char)188;
			else if(n == 9564)
				c = (char)189;
			else if(n == 9563)
				c = (char)190;
			else if(n == 9488)
				c = (char)191;
			else if(n == 9492)
				c = (char)192;
			else if(n == 9524)
				c = (char)193;
			else if(n == 9516)
				c = (char)194;
			else if(n == 9500)
				c = (char)195;
			else if(n == 9472)
				c = (char)196;
			else if(n == 9532)
				c = (char)197;
			else if(n == 9566)
				c = (char)198;
			else if(n == 9567)
				c = (char)199;
			else if(n == 9562)
				c = (char)200;
			else if(n == 9556)
				c = (char)201;
			else if(n == 9577)
				c = (char)202;
			else if(n == 9574)
				c = (char)203;
			else if(n == 9568)
				c = (char)204;
			else if(n == 9552)
				c = (char)205;
			else if(n == 9580)
				c = (char)206;
			else if(n == 9575)
				c = (char)207;
			else if(n == 9576)
				c = (char)208;
			else if(n == 9572)
				c = (char)209;
			else if(n == 9573)
				c = (char)210;
			else if(n == 9561)
				c = (char)211;
			else if(n == 9560)
				c = (char)212;
			else if(n == 9554)
				c = (char)213;
			else if(n == 9555)
				c = (char)214;
			else if(n == 9579)
				c = (char)215;
			else if(n == 9578)
				c = (char)216;
			else if(n == 9496)
				c = (char)217;
			else if(n == 9484)
				c = (char)218;
			else if(n == 9608)
				c = (char)219;
			else if(n == 9604)
				c = (char)220;
			else if(n == 9612)
				c = (char)221;
			else if(n == 9616)
				c = (char)222;
			else if(n == 9600)
				c = (char)223;
			else if(n == 8733)
				c = (char)224;
			else if(n == 223)
				c = (char)225;
			else if(n == 9624)
				c = (char)226;
			else if(n == 9629)
				c = (char)227;
			//skip 228, same as 223.
			else if(n == 9623)
				c = (char)229;
			else if(n == 9626)
				c = (char)230;
			//skip 231, same as 222.
			else if(n == 9622)
				c = (char)232;
			else if(n == 952)
				c = (char)233;
			else if(n == 937)
				c = (char)234;
			else if(n == 948)
				c = (char)235;
			else if(n == 8734)
				c = (char)236;
			else if(n == 8709)
				c = (char)237;
			else if(n == 8712)
				c = (char)238;
			else if(n == 8745)
				c = (char)239;
			else if(n == 8801)
				c = (char)240;
			else if(n == 177)
				c = (char)241;
			else if(n == 8805)
				c = (char)242;
			else if(n == 8804)
				c = (char)243;
			else if(n == 8992)
				c = (char)244;
			else if(n == 8993)
				c = (char)245;
			else if(n == 247)
				c = (char)246;
			else if(n == 8776)
				c = (char)247;
			else if(n == 8728)
				c = (char)248;
			else if(n == 8226)
				c = (char)249;
			else if(n == 8729)
				c = (char)250;
			else if(n == 9829)
				c = (char)3;
			//251 AND ON ARE LEFT FINAL STRETCH!
			return c;
		}
		public static char toUTF8(char c) {
			return c;
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