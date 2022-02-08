using Godot;
using System;

public class Options : Control
{
	Progression Progression;
	HSlider musicslider;
	HSlider sfxslider;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Progression = GetNode<Progression>("/root/Progression");
		
		VBoxContainer sliderparent = GetNode<VBoxContainer>("SliderBox");
		musicslider = sliderparent.GetNode<HSlider>("MusicSlider");
		sfxslider = sliderparent.GetNode<HSlider>("SFXSlider");
		
		musicslider.Value = Progression.MusicVol;
		sfxslider.Value = Progression.SFXVol;
		
	}
	private void _on_MusicSlider_value_changed(float value)
	{
		Progression.SetMusicVolume(value);
	}
	private void _on_SFXSlider_value_changed(float value)
	{
		Progression.SetSfxVolume(value);
		GetParent().GetParent().GetParent().GetNode<AudioStreamPlayer>("ProjectileAudio").Play();
	}
	
}



