using Godot;
using System;

public partial class PauseMenu : Control
{
	public override void _Ready()
	{
	var resume = GetNode<Button>("PanelContainer/VBoxContainer/Resume");
	var restart = GetNode<Button>("PanelContainer/VBoxContainer/Restart");
	var quit = GetNode<Button>("PanelContainer/VBoxContainer/Quit");
	
	resume.Pressed += ResumePressed;
	restart.Pressed += RestartPressed;
	quit.Pressed += QuitPressed;
	Visible = false;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Pause"))
		{
			TogglePause();
		}
	}

	public void TogglePause()
	{
		bool isPaused = GetTree().Paused;
		GetTree().Paused = !isPaused;
		Visible = !isPaused;

		ProcessMode = isPaused ? ProcessModeEnum.Inherit : ProcessModeEnum.Always;
	}
	
	private void ResumePressed()
	{
		TogglePause();
		GD.Print("Game resume");
	}

	private void RestartPressed()
	{
		TogglePause();
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}

	private void QuitPressed()
	{
		GetTree().Paused = false;
		//GetTree().ChangeSceneToFile("res://MainMenu.tscn");
	}
}
