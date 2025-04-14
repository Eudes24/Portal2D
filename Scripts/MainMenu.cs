using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] public PackedScene GameScene;
	private Control OptionsMenu;
	private AudioStreamPlayer2D Music;
	private HSlider slider;
	private int MusicIndex;

	public override void _Ready()
	{
		Music = GetNode<AudioStreamPlayer2D>("/root/Music"); // Musique globale
		OptionsMenu = GetNode<Control>("OptionsMenu");
		OptionsMenu.Visible = false;

		GetNode<Button>("VBoxContainer/StartGame").Pressed += OnStartGamePressed;
		GetNode<Button>("VBoxContainer/Options").Pressed += OnOptionsPressed;
		GetNode<Button>("OptionsMenu/Back").Pressed += OnOptionsBackPressed;

		slider = GetNode<HSlider>("OptionsMenu/HSlider");
		MusicIndex = AudioServer.GetBusIndex("Music");
	}

	private void OnStartGamePressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Level1.tscn");
	}

	private void OnOptionsPressed()
	{
		OptionsMenu.Visible = true;
	}

	private void OnOptionsBackPressed()
	{
		OptionsMenu.Visible = false;
	}

	public override void _Input(InputEvent @event)
	{
		AudioServer.SetBusVolumeDb(MusicIndex, Mathf.LinearToDb((float)slider.Value));
	}
}
