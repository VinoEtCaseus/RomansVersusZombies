using Godot;
using System;

public class Unit : KinematicBody2D
{
	[Export]
	public int team = 0;
	private Vector2 movetarget;
	private float Speed = 100f;
	private float hp = 10;
	private float cooldown = 0f;
	
	AnimatedSprite sprt;
	Polygon2D hpbar;
	public AnimationPlayer animplyr;
	
	float zombspeed = 50f;
	
	[Signal]
	public delegate void EndGame();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		movetarget = Position;
		
		sprt = GetNode<AnimatedSprite>("AnimatedSprite");
		hpbar = GetNode<Polygon2D>("HpBar");
		animplyr = GetNode<AnimationPlayer>("AnimationPlayer");
		
		
		sprt.Frame = team;
		CollisionCrunching();
	}
	
	public void SetTarget(Vector2 target){
		movetarget = target;
	}
	public void Hit(){
		if(team == 0){ //Guards
			hp -= 4;
			if(hp <= 0){
				SetTeam(1);
				hp = 8;
				SetTarget(new Vector2(0, Position.y));
			}
		}
		else if(team == 1){ //Zombies
			hp -= 5;
		} else {
			GD.Print("Error, team undetermined");
		}
	}
	
	public override void _Process(float delta){
		//Move Logic
		Vector2 velocity = new Vector2(0,0);
		if((movetarget-Position).Length() > 2){
			velocity = Position.DirectionTo(movetarget).Normalized()*Speed;
			animplyr.Play("Walk");
			animplyr.PlaybackSpeed = Speed/100;
		}
		
		var collision = MoveAndCollide(velocity*delta);
		
		//collisions = attacks
		cooldown -= delta;
		if (collision != null)
		{
			
			if (collision.Collider.HasMethod("Hit") && 
			(int)collision.Collider.Get("team") != team &&
			cooldown <= 0)
			{
				cooldown = 1f; //3 seconds to attack
				collision.Collider.Call("Hit");
			}
		}
		
		//hp stuff
		if(hp <= 0){
			Die();
		}
		hpbar.Scale = new Vector2((float)hp/10f, 1);
		
		//zombiefailurecheck
		if(team == 1 && Position.x < 10){
			EmitSignal(nameof(EndGame));
		}
	}
	
	public void SetTeam(int i){
		team = i;
		sprt.Frame = team;
		CollisionCrunching();
		if(team == 1){
			Speed = zombspeed;
		}
	}
	
	private void CollisionCrunching(){
		if(team == 0){
			CollisionLayer = 0;
			CollisionMask = 1;
		} else if (team == 1){
			CollisionLayer = 1;
			CollisionMask = 1;
		}
	}
	
	private void Die(){
		if(team == 0){
			
		} else if(team == 1){
			//Coin Spawn
			PackedScene coinscene = GD.Load<PackedScene>("res://Coin.tscn");
			Node2D coin = (Node2D)coinscene.Instance();
			GetParent().GetParent().AddChild(coin);
			coin.Position = Position;
			//Audio
			AudioStreamPlayer audio = GetParent().GetParent().GetNode<AudioStreamPlayer>("UnitAudio");
			var rand = (int)GD.RandRange(1,24);
			var audio_file = "res://Art/zombies/zombie-" + rand.ToString() + ".wav";
			
			AudioStream sfx = GD.Load<AudioStream>(audio_file); 
			audio.Stream = sfx;
			audio.Play();
		}
		QueueFree();
	}
}
