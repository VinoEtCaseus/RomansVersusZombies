using Godot;
using System;

public class Progression : Node
{
	
	private string path = "user://save.dat";
		
	public float coins = 1000;
		
	public float defaultreserves = 5;
	public float arrowrate = 1f;
	public float zombieswavetime = 5f;
	public float secondsperreserve = 3f;
	
	public float constres;
	public float constar;
	public float constzwt;
	public float constspr;
	
	public float resmax = 20f;
	public float armax = 0.25f;
	public float zwtmax = 8f;
	public float sprmax = 0.5f;
	
	
	public float MusicVol = -6f;
	public float SFXVol = -6f;
	
	Godot.Collections.Dictionary<string, float> vars;
	
	public override void _Ready()
	{
		CreateDict();
		
		constres = defaultreserves;
		constar = arrowrate;
		constzwt = zombieswavetime;
		constspr = secondsperreserve;
		
		LoadGame();
		
		SetMusicVolume(MusicVol);
		SetSfxVolume(SFXVol);
	}
	
	public void SaveGame()
	{
		var savegame = new File();
		var error = savegame.OpenEncryptedWithPass(path, File.ModeFlags.Write, "uenUNR1560");
		if(error == Error.Ok){
			GD.Print("SavingValues");
			CreateDict();
			savegame.StoreVar(vars);
		}
		savegame.Close();
	}
	
	public void LoadGame(){
		var savegame = new File();
		if(savegame.FileExists(path)){
			var error = savegame.OpenEncryptedWithPass(path, File.ModeFlags.Read, "uenUNR1560");
			if(error == Error.Ok){
				Godot.Collections.Dictionary tempdic = (Godot.Collections.Dictionary)savegame.GetVar();
				GD.Print("LoadingValues");
				coins = (float)tempdic["coins"];
				
				defaultreserves = (float)tempdic["DReserves"];
				arrowrate = (float)tempdic["ArrowRate"];
				zombieswavetime = (float)tempdic["ZWT"];
				secondsperreserve = (float)tempdic["SPR"];
				
				MusicVol = (float)tempdic["Music"];
				SFXVol = (float)tempdic["SFX"];
			}
			
			savegame.Close();
		}
		
		
	}
	
	private void CreateDict(){
		vars = new Godot.Collections.Dictionary<string, float>();
		vars.Add("coins", coins);
		
		vars.Add("DReserves", defaultreserves);
		vars.Add("ArrowRate", arrowrate);
		vars.Add("ZWT", zombieswavetime);
		vars.Add("SPR", secondsperreserve);
		
		vars.Add("Music", MusicVol);
		vars.Add("SFX", SFXVol);
	}
	
	public void SetMusicVolume(float vol){
		MusicVol = vol;
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), vol);
	}
	public void SetSfxVolume(float vol){
		SFXVol = vol;
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), vol);
	}
//  public override void _Process(float delta)
}
