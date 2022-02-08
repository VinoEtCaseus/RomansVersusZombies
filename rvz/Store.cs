using Godot;
using System;

public class Store : Control
{
	Progression Progression;
	Label label;
	
	Label arrowlabel;
	Label reservelabel;
	Label zwtlabel;
	Label sprlabel;
	
	private int arrowcost = 1;
	private int reservecost = 1;
	private int zwtcost = 1;
	private int sprcost = 1;
	
	private float arrowprograte = -0.1f;
	private float reserveprograte = 1f;
	private float zwtprograte = 0.25f;
	private float sprprograte = -0.1f;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Progression = GetNode<Progression>("/root/Progression");
		label = GetNode<Label>("ErrorLabel");
		
		var parent = GetNode("Items");
		arrowlabel = parent.GetNode<Label>("ArrowLabel");
		reservelabel = parent.GetNode<Label>("ReserveLabel");
		zwtlabel = parent.GetNode<Label>("ZwtLabel");
		sprlabel = parent.GetNode<Label>("SprLabel");
		
		ReloadArrow();
		ReloadReserve();
		ReloadZwt();
		ReloadSpr();
		
		label.Text = "";
	}
	private void ReloadArrow(){
		arrowlabel.Text = "Arrow Rate of Fire: " + Progression.arrowrate.ToString();
		arrowcost = 20*(int)(Math.Abs(Progression.arrowrate - Progression.constar)/Math.Abs(arrowprograte)+1);
		arrowlabel.GetNode<Label>("ArrowCostLabel").Text = "Cost: " + arrowcost.ToString() + " Coins";
	}
	private void ReloadReserve(){
		reservelabel.Text = "Default Reserves: " + Progression.defaultreserves.ToString();
		reservecost = 20*(int)(Math.Abs(Progression.defaultreserves - Progression.constres)/Math.Abs(reserveprograte)+1);
		reservelabel.GetNode<Label>("ReserveCostLabel").Text = "Cost: " + reservecost.ToString() + " Coins";
	}
	
	private void ReloadZwt(){
		zwtlabel.Text = "Zombies Spawn Rate: " + Progression.zombieswavetime.ToString();
		zwtcost = 20*(int)(Math.Abs(Progression.zombieswavetime - Progression.constzwt)/Math.Abs(zwtprograte)+1);
		zwtlabel.GetNode<Label>("ZwtCostLabel").Text = "Cost: " + zwtcost.ToString() + " Coins";
	}
	private void ReloadSpr(){
		sprlabel.Text = "Seconds Per Reserve:  " + Progression.secondsperreserve.ToString();
		sprcost = 20*(int)(Math.Abs(Progression.secondsperreserve - Progression.constspr)/Math.Abs(sprprograte));
		sprlabel.GetNode<Label>("SprCostLabel").Text = "Cost: " + sprcost.ToString() + " Coins";
	}
	private void _on_ArrowButton_pressed()
	{
		if(Progression.arrowrate > Progression.armax){
			if(Progression.coins >= arrowcost){
				Progression.coins -= arrowcost;
				Progression.arrowrate = (float)Math.Round(Progression.arrowrate + arrowprograte, 1);
				ReloadArrow();
				
				label.Text = "PurchaseSucessful";
				
			} else {
				label.Text = "Not Enough Coins";
			}
		}else {
			label.Text = "Max Upgrade Reached";
		}
	}
	
	
		private void _on_ReserveButton_pressed()
		{
			if(Progression.defaultreserves <= Progression.resmax){
				if(Progression.coins >= reservecost){
					Progression.coins -= reservecost;
					Progression.defaultreserves = (float)Math.Round(Progression.defaultreserves + reserveprograte, 1);
					ReloadReserve();
					
					label.Text = "PurchaseSucessful";
					
				} else {
					label.Text = "Not Enough Coins";
				}
			}else {
				label.Text = "Max Upgrade Reached";
			}
		}


		private void _on_ZwtButton_pressed()
		{
			if(Progression.zombieswavetime <= Progression.zwtmax){
				if(Progression.coins >= zwtcost){
					Progression.coins -= zwtcost;
					Progression.zombieswavetime = (float)Math.Round(Progression.zombieswavetime + zwtprograte, 1);
					ReloadZwt();
					
					label.Text = "PurchaseSucessful";
					
				} else {
					label.Text = "Not Enough Coins";
				}
			}else {
				label.Text = "Max Upgrade Reached";
			}
		}


		private void _on_SprButton_pressed()
		{
		if(Progression.secondsperreserve > Progression.sprmax){
				if(Progression.coins >= sprcost){
					Progression.coins -= sprcost;
					Progression.secondsperreserve = (float)Math.Round(Progression.secondsperreserve + sprprograte, 1);
					ReloadSpr();
					
					label.Text = "PurchaseSucessful";
					
				} else {
					label.Text = "Not Enough Coins";
				}
			}else {
				label.Text = "Max Upgrade Reached";
			}
		}
}




