using Godot;
using System;

public class Arrow : KinematicBody2D
{
	private float gravity = -9.8f;
	private Vector2 velocity;
	private Vector2 target;
	private float speed = 500;
	private bool Stop = false;
	private float deathtimer = 10;
	
	public int team = 2;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		int randstart = (int)GD.RandRange(16, 584); 
		//int randstart = 100;
		Position = new Vector2(0, randstart);
		velocity = new Vector2(0,0);
		
		
		CollisionLayer = 2;
		CollisionMask = 1;
		
		
	}
	
	public override void _Process(float delta){
		if(!Stop){
			Rotation = velocity.Angle();
			var collision = MoveAndCollide(velocity*delta);
			if(collision != null){
				if((int)collision.Collider.Get("team") == 1){
					collision.Collider.Call("Die");
				}
			}
			velocity.y -= gravity*delta;
			if(Position.y > GetViewport().Size.x){ //removes buggy arrows
				QueueFree();
			}
		} else if(deathtimer < 0){ //kill after 10 seconds
			QueueFree();
		}
		if(Position.DistanceTo(target)<16){ //start timer if pos is close enought to target
			Stop = true;
			deathtimer -= delta;
			CollisionLayer = 8;
			CollisionMask = 8;
		}
	}
	
	public void SetTarget(Vector2 pos){
		target = pos;
		CalculateArcVel(Position, target, 10);
	}
	
	private void CalculateArcVel(Vector2 startpos, Vector2 finishpos, float archeight){
		var dis = finishpos - startpos;
		var timeinair = -dis.x/(speed);
		velocity.y = -(2*dis.y + gravity*(float)Math.Pow(timeinair, 2))/(2*timeinair);
		velocity.x = speed;
	}
	
	
}
