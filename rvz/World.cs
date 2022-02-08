using Godot;
using System;

public class World : Node2D
{
	Progression Progression;
	
	Line2D holdline;
	RichTextLabel reservelabel;
	Control hud;
	Control gameinfo;
	Panel gopanel; //go = game over
	RichTextLabel wavelabel;
	VBoxContainer startmenu;
	
	PackedScene zombiescene = GD.Load<PackedScene>("res://Unit.tscn");
	PackedScene arrowscene = GD.Load<PackedScene>("res://Arrow.tscn");
	
	
	private int screenstart = 32; //y
	private int screenlen = 568; //y
	
	Godot.Collections.Dictionary<Vector2, Node2D> unitposhashes;
	private int unitcount = 0;
	
	//zombie vars;
	private float ztimer = 0;
	private int zombiesperwave = 1;
	private int waves = 0;
	
	//Default Player values
	private int reservelimit = 256;
	
	//input vars
	private string mode = "unit";
	private float inputtimer = 0;
	private float reservetimer = 0;
	private float arrowtimer = 0;
	
	private float xlinepos = -16;
	private int reserves;
	
	private bool StopGame = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Randomize();
		//Control Node References
		holdline = GetNode<Line2D>("HoldLine");
		
		hud = GetNode("HudLayer").GetNode<Control>("Hud");
		gameinfo = hud.GetNode<Control>("GameInfo");
		reservelabel =gameinfo.GetNode<RichTextLabel>("ReserveLabel");
		wavelabel = gameinfo.GetNode<RichTextLabel>("WaveLabel");
		gopanel = hud.GetNode<Panel>("GameOverPanel");
		startmenu = hud.GetNode<VBoxContainer>("StartMenu");
		
		unitposhashes = new Godot.Collections.Dictionary<Vector2, Node2D>();
		
		//ConnectButton Signals*******************************
		//Menu Buttons
		gopanel.GetNode("RetryButton").Connect("pressed", this, "StartGame");
		gopanel.GetNode("EndSessionButton").Connect("pressed", this, "EndGame");
		gopanel.GetNode("MenuButton").Connect("pressed", this, "GoToMenu");
		//Start Buttons
		startmenu.GetNode("StartGame").Connect("pressed", this, "StartGame");
		startmenu.GetNode("Tutorial").Connect("pressed", this, "OpenTutorial");
		startmenu.GetNode("Options").Connect("pressed", this, "OpenOptions");
		startmenu.GetNode("Upgrades").Connect("pressed", this, "OpenStore");
		startmenu.GetNode("QuitGame").Connect("pressed", this, "EndGame");
		
		//Game Buttons
		gameinfo.GetNode("ArrowButton").Connect("pressed", this, "SetMode", new Godot.Collections.Array() {"arrow"});
		gameinfo.GetNode("UnitButton").Connect("pressed", this, "SetMode", new Godot.Collections.Array() {"unit"});
		//startmenu.GetNode("").Connect("pressed", this, "");
		
		
		
		/*****************************************************************/
		//Upgrade Handling
		Progression = GetNode<Progression>("/root/Progression");
		
		/*******************************************************************/
		
		
		GoToMenu();
		//Debug Options
		//StartGame();
		/*
		for(int i =0; i<16; i++){//debugging
			
		}*/
	}
	
	//Process****************************************************************/
	public override void _Process(float delta){
		var coinpanel = GetNode<RichTextLabel>("CoinLabel");
		coinpanel.Text = "Coins: " + Progression.coins.ToString();
		if(!StopGame){
			Vector2 mousepos = GetGlobalMousePosition();
			//Input /********************************************************************************/
			if(Input.IsActionPressed("secondary_click")){
				//Clear hash table of unit pos's since they are changing
				unitposhashes.Clear();
				
				//Count units (we can transfer this to a function later)
				var units = GetTree().GetNodesInGroup("Units");
				unitcount = 0;
				foreach(KinematicBody2D unit in units){
					if((int)unit.Get("team")==0){
						unitcount++;
					}
				}
				xlinepos = mousepos.x;
				holdline.Position = new Vector2(mousepos.x, holdline.Position.y);
				foreach(KinematicBody2D unit in units){
					if((int)unit.Get("team")==0){
						
						var target = GetUnitLinePos(unit, xlinepos);
						//actually moves target
						unitposhashes.Add(target, unit);
						unit.Call("SetTarget", target);
					}
				}
			}
			//Spawning Units Input
			if(Input.IsActionPressed("primary_click")){
				if(mode == "unit"){
					//Spawn unit to closest point on line on click
					if(inputtimer > 0.125 && reserves > 0){
						var unit = SpawnUnit(new Vector2(0, mousepos.y), 0);
						var temptar = GetUnitLinePos(unit, xlinepos);
						unitposhashes.Add(temptar, unit);
						unitcount ++;
						unit.Call("SetTarget", temptar);
						reserves --;
						inputtimer = 0;
					}
				}  else if(mode == "arrow"){
					//Shoot Arrow on click
					if(arrowtimer > Progression.arrowrate){
						var arrow = SpawnArrow();
						arrow.Call("SetTarget", mousepos);
						arrowtimer = 0;
					}
				}
			}
			inputtimer += delta;
			arrowtimer += delta;
			
			//End of Input /****************************************************************/
			//Reserve Timer:
			reservelabel.Text = "Reserves: "+reserves.ToString();
			if(reservetimer > Progression.secondsperreserve){
				if(reserves < reservelimit){
					reserves ++;
				}
				reservetimer = 0;
			} else {
				reservetimer += delta;
			}
			
			//Zombie Spawner
			if(true){ //placeholder for starting wave offensive 
				if(ztimer > Progression.zombieswavetime){
					for(int i=0; i<zombiesperwave; i++){
						
						var pos = new Vector2(1024+32, (float)GD.RandRange(screenstart, screenlen));
						KinematicBody2D instance = SpawnUnit(pos, 1);
						instance.Call("SetTarget", new Vector2(0, instance.Position.y));
						
					}
					waves ++;
					zombiesperwave = (int)waves/2+1;
					ztimer = 0;
				} else {
					ztimer += delta;
				}
			}
			
			wavelabel.Text = "Wave: "+waves.ToString();
		}
		//Zombie Failure Check
	}
	
	//Button Pressed Functions ********************************************/
	private void SetMode(string str){
		mode = str;
	}
	
	private void Reset() {
		Progression.SaveGame();
		var units = GetTree().GetNodesInGroup("Units");
		unitcount = 0;
		foreach(KinematicBody2D unit in units){
			unit.QueueFree();
		}
		
		var arrows = GetTree().GetNodesInGroup("Projectiles");
		foreach(KinematicBody2D arrow in arrows){
			arrow.QueueFree();
		}
		
		//Timers Reset
		ztimer = 0;
		reservetimer = 0;
		inputtimer = 0;
		arrowtimer = 0;
		
		//EndGame
		StopGame = true;
		gopanel.Show();
	}
	
	private void StartGame(){
		gopanel.Hide();
		startmenu.Hide();
		gameinfo.Show();
		StopGame = false;
		
		//Music
		GetNode<AudioStreamPlayer>("MenuMusic").Stop();
		GetNode<AudioStreamPlayer>("GameMusic1").Play();
		
		//Menu Animation Finish
		var units = GetTree().GetNodesInGroup("Units");
		foreach(KinematicBody2D unit in units){
			var arrow = SpawnArrow();
			arrow.Call("SetTarget", unit.Position);
		}
		
		//Reset Default Values
		reserves = (int)Progression.defaultreserves;
		waves = 0;
		zombiesperwave = 1;
	}
	
	private void GoToMenu(){
		Progression.SaveGame();
		Show();
		gopanel.Hide();
		startmenu.Show();
		gameinfo.Hide();
		StopGame = true;
		
		//Music
		GetNode<AudioStreamPlayer>("MenuMusic").Play();
		GetNode<AudioStreamPlayer>("GameMusic1").Stop();
		
		//Little Menu "Animation"
		for(int i = 0; i<GD.RandRange(1, 5); i++){
			var pos = new Vector2((float)GD.RandRange(100, 1000), (float)GD.RandRange(screenstart, screenlen));
			var zom = SpawnUnit(pos,1);
		}
		
		int resetlinepos = -16;
		xlinepos = -resetlinepos;
		holdline.Position = new Vector2(resetlinepos, holdline.Position.y);
	}
	
	private void EndGame(){
		Progression.SaveGame();
		GetTree().Quit();
	}
	
	//Menu Buttons
	Control tut;
	private void OpenTutorial(){
		startmenu.Hide();
		
		PackedScene tutorialscene = GD.Load<PackedScene>("res://Tutorial.tscn");
		tut = (Control)tutorialscene.Instance();
		hud.AddChild(tut);
		tut.GetNode<Button>("Button").Connect("pressed", this, "CloseTut");
	}
	
	private void CloseTut(){
		startmenu.Show();
		tut.QueueFree();
	}
	
	Control store;
	private void OpenStore(){
		hud.Hide();
		
		PackedScene storescene = GD.Load<PackedScene>("res://Store.tscn");
		store = (Control)storescene.Instance();
		GetNode("HudLayer").AddChild(store);
		store.GetNode<Button>("CloseButton").Connect("pressed", this, "CloseStore");
	}
	private void CloseStore(){
		Progression.SaveGame();
		hud.Show();
		store.QueueFree();
	}
	
	Control opt;
	private void OpenOptions(){
		startmenu.Hide();
		
		PackedScene optscene = GD.Load<PackedScene>("res://Options.tscn");
		opt = (Control)optscene.Instance();
		hud.AddChild(opt);
		opt.GetNode<Button>("CloseButton").Connect("pressed", this, "CloseOptions");
	}
	private void CloseOptions(){
		Progression.SaveGame();
		startmenu.Show();
		opt.QueueFree();
	}
	
	/*************************************************************************/
	
	//Helper Functions
	private Vector2 NearestPointOnLine(Vector2 origin, Vector2 direction, Vector2 point)
	{
		direction.Normalized();
		Vector2 lhs = point - origin;

		float dotP = lhs.Dot(direction);
		return origin + direction * dotP;
	}//stolen
	
	private Vector2 GetUnitLinePos(Node2D unit, float x){
		int unitwidth = 16;
		int unitsperline = 16;
		Vector2 lineorigin = holdline.GetPointPosition(0);
		Vector2 linedir = lineorigin.DirectionTo(holdline.GetPointPosition(1));
		float linex = x;
		float len = screenlen;
		
		int unidistr = (int)len/unitsperline;
		
		//meat and bones: set target and move
		var np = NearestPointOnLine(new Vector2(linex, lineorigin.y), linedir, unit.Position);
		np.y = Mathf.Clamp(np.y, screenstart, screenlen);
		var target = np - (new Vector2(0, np.y%unidistr)); //uniform distribution on line
		while (unitposhashes.ContainsKey(target)){ //loop though each adjacent pos, then move back to next row
			target -= new Vector2(unitwidth*2, 0); // moves behind closest unit;
		} // changes to O(n)
		
		return target;
	}
	
	private KinematicBody2D SpawnUnit(Vector2 pos, int team){
		KinematicBody2D unit = (KinematicBody2D)zombiescene.Instance();
		GetNode("Units").AddChild(unit);
		unit.Call("SetTeam", team);
		unit.Position = pos;
		unit.Call("SetTarget", pos);

		unit.Connect("EndGame", this, "Reset");
		return unit;
	}
	
	private KinematicBody2D SpawnArrow(){
		KinematicBody2D arrow = (KinematicBody2D)arrowscene.Instance();
		AddChild(arrow);
		AudioStreamPlayer aud = GetNode<AudioStreamPlayer>("ProjectileAudio");
		aud.Play();
		return arrow;
	}
}
