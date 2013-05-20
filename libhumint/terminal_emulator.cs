using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class terminal_emulator : MonoBehaviour {
	public GUIText contents;
	string comp = "Usr-Mob:";
	string curFolder = "~";
	string path = "~";
	string user = " Main$ ";
	string intro = "Last login: Tue May 14 16:33:36 on ttys000";
	List<string> lines = new List<string>();
	List<string> prevCommands = new List<string>();
	Folder root = new Folder("~");
	string input;
	string end = "█";
	
	List<string> rows = new List<string>();
	
	void Start() {
		lines.Add(intro);
	}
	
	void OnGUI() {
		SetText();
	}
	
	void Update() {
		PlayerIsTyping();
	}
	
	void SetText() {
		string toSet = null;
		string prefix = comp + curFolder + user;
		foreach(string l in lines) {
			toSet += l+"\n";
		}
		contents.text = toSet + prefix + input + end;
	}
	
	void PlayerIsTyping() {
		string prefix = comp + curFolder + user;
		if(Input.inputString != "\n" && Input.inputString != "\b")
			input+=Input.inputString;
		else if(Input.inputString == "\n") {
			prevCommands.Add(input);
			Parse(input);
			if(input.StartsWith("clear") == false)
				lines.Add(prefix+input);
			if(lines.Count == 15)
				lines.Remove(lines[0]);
			input = null;
		}
		else if(Input.inputString == "\b" && input.Length > 0) {
			input = input.Remove(input.Length - 1);
		}
	}
	
	void Parse(string cmd) {
		string c = cmd;
		if(cmd.StartsWith("sudo") == true) {
			c = c.Remove(0,5);
		}
		if(c.StartsWith("cd"))
			cd(c);
		if(c.StartsWith("ls")) {
			//ls(path);
		}
		if(c.StartsWith("clear"))
			clear();
	}
	
	void cd(string text) {
		string cmd = prevCommands[prevCommands.Count - 1].Remove(0,2);
		Debug.Log(cmd);
	}
	
	void ls(Folder curF) {
		
	}
	
	void clear() {
		lines.Clear();
		
	}
}
public struct Folder {
	string name;
	
	public Folder(string n) {
		name = n;
	}
}