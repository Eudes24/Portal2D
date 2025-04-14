using System;
using Godot;

// It's the class of the pause menu in game
public partial class PauseMenu : Control
{
	public override void _Ready() // Connects the different buttons
	{
		var resume = GetNode<Button>("PanelContainer/VBoxContainer/Resume");
		var restart = GetNode<Button>("PanelContainer/VBoxContainer/Restart");
		var quit = GetNode<Button>("PanelContainer/VBoxContainer/Quit");

		resume.Pressed += ResumePressed;
		restart.Pressed += RestartPressed;
		quit.Pressed += QuitPressed;
		Visible = false;
	}

	public override void _Input(InputEvent @event) // Sets up the pause when escape is pressed
	{
		if (@event.IsActionPressed("Pause"))
		{
			TogglePause();
		}
	}

	public void TogglePause() // Pause the game and show the menu
	{
		bool isPaused = GetTree().Paused;
		GetTree().Paused = !isPaused;
		Visible = !isPaused;

		ProcessMode = isPaused ? ProcessModeEnum.Inherit : ProcessModeEnum.Always;
	}

	private void ResumePressed() // Resume the game
	{
		TogglePause();
		GD.Print("Game resume");
	}

	private void RestartPressed() // Restart the level
	{
		TogglePause();
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}

	private void QuitPressed() // Send to the main menu 
	{
		TogglePause();
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
	}
}
