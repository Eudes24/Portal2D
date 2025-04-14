using System;
using Godot;

// Scripts where the main menu is set up
public partial class MainMenu : Control
{
	[Export]
	public PackedScene GameScene;
	private Control OptionsMenu;
	private AudioStreamPlayer2D Music;
	private HSlider slider;
	private int MusicIndex;

	public override void _Ready() // Sets up the different buttons and the slider
	{
		Music = GetNode<AudioStreamPlayer2D>("/root/Music");
		OptionsMenu = GetNode<Control>("OptionsMenu");
		OptionsMenu.Visible = false; // Hide the second panel 

		GetNode<Button>("VBoxContainer/StartGame").Pressed += OnStartGamePressed;
		GetNode<Button>("VBoxContainer/Options").Pressed += OnOptionsPressed;
		GetNode<Button>("OptionsMenu/Back").Pressed += OnOptionsBackPressed;

		slider = GetNode<HSlider>("OptionsMenu/HSlider");
		MusicIndex = AudioServer.GetBusIndex("Music");
	}

	private void OnStartGamePressed() // Send the player to the first level
	{
		GetTree().ChangeSceneToFile("res://Scenes/Level1.tscn");
	}

	private void OnOptionsPressed() // shows the volume option panel
	{
		OptionsMenu.Visible = true;
	}

	private void OnOptionsBackPressed() // Sets up the "back" that make the player return to the first menu
	{
		OptionsMenu.Visible = false;
	}

	public override void _Input(InputEvent @event) // Manage the sound volume
	{
		AudioServer.SetBusVolumeDb(MusicIndex, Mathf.LinearToDb((float)slider.Value));
	}
}
