using Godot;
using System;

public class Coin : Node2D
{
	Progression Progression;
	float radius = 16;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Progression = GetNode<Progression>("/root/Progression");
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta){
		Vector2 mousepos = GetGlobalMousePosition();
		if((mousepos-Position).Length() < radius){
			Progression.coins++;
			QueueFree();
		}
	}
}
