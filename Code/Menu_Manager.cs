using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using libhumint;

//All menu variables should be stored in here.
public class Menu_Manager : MonoBehaviour {
	List<string> loadedFields = new List<string>(); //internal storage of loaded strings
	string[] Titles = {"title","subtitle"}; //Game title & abbreviation
	string[] MMItemTitles = {"ng_button","lg_button","am_button","aa_button","go_button","ab_button","re_button"}; //Main menu items
	Console Menu_BG;
	
	void Awake() {
		LoadStrings();
		SetStrings();
		Menu_BG = new Console("terminal10x16_gs_ro");
	}
	void Start() {
		LoadBackground("Menu Backgrounds/MainMenu");
		FinishMMBackground();
	}
	void OnGUI() {
		Menu_BG.Render();
	}
	void LoadStrings() {
		TextAsset source = Resources.Load("Game Strings") as TextAsset;
		foreach(string row in source.text.Split('\n')) {
			if(row.StartsWith("#"))
				continue;
			else
				loadedFields.Add(row);
		}
	}
	void SetStrings() {
		foreach(string field in loadedFields) {
			string f = field.Remove(field.IndexOf("=")).TrimEnd();
			if(Titles.Contains(f))
				Titles[System.Array.IndexOf(Titles,f)] = formatField(field);
			if(MMItemTitles.Contains(f))
				MMItemTitles[System.Array.IndexOf(MMItemTitles,f)] = formatField(field);
		}
	}
	void LoadBackground(string filename) {
		TextAsset source = Resources.Load(filename) as TextAsset;
		string[] rows = source.text.Split('\n');
		int y = 0;
		foreach(string row in rows) {
			char[] cont = row.ToCharArray();
			int x = 0;
			foreach(char c in cont) {
				Menu_BG.Put(x,y,CharOps.ToASCII(c));
				x++;
			}
			y++;
		}
	}
	void FinishMMBackground() {
		for(int x=0;x<=25;x++) {//AMERICAN FLAG
			for(int y=0;y<15;y++) {
				if(x<10 && y<6) //STARS
					Menu_BG.grid[(y*70) + x].background = ColorLib.Blue("alternate");
				//else if(Menu_BG.grid[(y*70) + x].character != (char)219) //STRIPES
				else if(y % 2 == 1)
					Menu_BG.grid[(y*70) + x].background = ColorLib.Red("natural");
				else if(y % 2 != 1 && x == 25)
					Menu_BG.grid[(y*70) + x].background = ColorLib.White();
			}
		}
		for(int x=44;x<70;x++) {//EAST GERMAN FLAG
			for(int y=0;y<15;y++) {
				if(y<5)
					Menu_BG.grid[(y*70) + x].background = ColorLib.Black();
				else if(y >= 5 && y < 10)
					Menu_BG.grid[(y*70) + x].background = ColorLib.Red("natural");
				else if(y >= 10)
					Menu_BG.grid[(y*70) + x].background = ColorLib.Yellow("natural");
			}
		}
		for(int y=0;y<15;y++) { //BORDER COLORING
			Menu_BG.grid[(y*70) + 25].foreground = ColorLib.LightGray("natural");
			Menu_BG.grid[(y*70) + 44].foreground = ColorLib.LightGray("natural");
		}
	}
	string formatField(string s) {
		char prevChar = new char();
		int trimEnd = 0;
		foreach(char c in s.ToCharArray()) {
			if(c == '=')
			{
				trimEnd = System.Array.IndexOf(s.ToCharArray(),c);
				break;
			}
			prevChar = c;
		}
		s = s.Remove(0,trimEnd).TrimStart('=',' ');
		return s;
	}
}