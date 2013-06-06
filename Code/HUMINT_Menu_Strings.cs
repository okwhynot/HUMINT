//This should read this data from a TextAsset at some point. Please impliment this :B
public static class UIStrings {
	//Main Menu
	public static string title = "HUMAN INTELLIGENCE";
	public static string subtitle = (char)249+"HUMINT"+(char)249;
	public static string[] MMButtons = {"New Asset","Review Dossier","Agent Memorial","Agency Archive","Settings","About HUMINT","Retire"};
	//Character Creator
	public static string[] ccsections = {"Bezirk","Season","Faction","Specialization","Gender"};
	public static string commandant = "Karl-Heinz Drews";
	public static string Date = "January 1";
	public static string year = "1980";
	public static string[] Bezirke = {"Friedrichshain","K"+(char)148+"penick","Lichtenberg","Marzahn","Mitte","Pankow","Prenzlaur Berg","Treptow","Weissensee"};
	public static string[] Seasons = {"Winter","Spring","Summer","Autumn"};
	public static string[] Factions = {"Staatssicherheit","Central Intelligence Agency"};
	public static string[] FactionsShort = {"Staasi","CIA"};
	public static string[] Specializations = {"Pistols","Explosives","Manipulation","Rifles"};
	public static string[] Gender = {"Male","Female"};
	//Visual Creator
	public static string[] VisSections = {"Skin Color","Hair Style","Hair Color","Eye Color","Scars","Age"};
	//Passport
	public static string[] FieldNames = {"geburtsname","vorname","geschlect","geburtsdatum","geburtsort","familienstand"};
	public static string[] CityNames = {"Dresden","Leipzig","Koblenz","Kassel","Augsburg","N"+(char)129+"rnberg"};
	public static string[] GermanGenders = {"M"+(char)132+"nnlich","Weiblich"};
	public static string cfied = "CLASSIFIED";
	public static string[] DossierFieldNames = {"specialization","starting gear","perks"};
	public static string[] BloodTypes = {"O-","O+","A-","A+","B-","B+","AB-","AB+"};
	
	//In-Game
	public static string[] IGMenuTitles = {"INFO","HOTKEYS"};
	public static string[] IGMenu = {"attack","character","holster","interact","inventory","map","operations"};
	public static string[] Holstered = {"unholstered","holstered"};
}
public static class WeaponStrings {
	//Modifiers
	public static string silencer = "silenced";
	public static string exmag = "extended magazine";
	public static string[] mods = {silencer,exmag};
	//Pistols
	public static string makarov = "Makarov PM";
	public static string ppk = "Walther PPK";
	public static string p5 = "Walther P5";
	public static string p38 = "Walther P38";
	public static string psm = "PSM";
	public static string sauer38 = "Sauer 38H";
	public static string t33 = "Tokarev T-33";
	public static string aps = "Stechkin APS";
	public static string[] pistols = {makarov,ppk,p5,p38,psm,sauer38,t33,aps};
	//Rifles
	public static string svt40 = "SVT-40";
	//Knives
	
	//Melee
	public static string baton = "Baton";
	public static string kknife = "Kitchen Knife";
	public static string bknife = "Butcher Knife";
}
public static class AmmoStrings {
	public static string TsN7N7 = "5.45x18mm";
	public static string LR22 = ".22 LR";
	public static string ACP380 = ".380 ACP";
	public static string PB9x19 = "9x19mm Parabellum";
	public static string Mak9x18 = "9x18mm Makarov";
	public static string Tok762 = "7.62x25mm Tokarev";
	public static string SVT762 = "7.62x54mmR";
}
public static class ClothingStrings {
	//Colors
	public static string[] colors = {"red","blue","green","black"};
	//Quality
	public static string casual = "casual";
	public static string worker = "workman's";
	public static string nice = "nice";
	public static string[] quality = {casual,worker,nice};
	//Article
	public static string hat = "hat"; //nice
	public static string cap = "cap"; //casual or workman's
	public static string coat = "coat"; //nice
	public static string jacket = "jacket"; //casual or workman's
	public static string shirt = "shirt"; //all
	public static string gloves = "gloves"; //all
	public static string belt = "belt"; //all
	public static string pants = "pants"; //nice
	public static string trousers = "trousers"; //casual or workman's
	public static string slacks = "slacks"; //casual or workman's
	//Suits
}
public static class GadgetStrings {
	
}
public static class WeaponDesStrings {
	
}
public static class AmmoDesStrings {
	
}
public static class ClothingDesStrings {
	
}
public static class GadgetDesStrings {
	
}