using System;
using Godot;

public partial class NextLevel : Area2D
{
	public static int NumberNextLevel = 2;
	public string NextLevelPath = $"res://Niveaux/Level{NumberNextLevel}.tscn";

	public NextLevel()
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}

	public void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			GD.Print("GG WP, go next now");
			NumberNextLevel++;

			// Using calldeffered to avoid a warning about suppression while changin scene
			CallDeferred(nameof(ChangeScene));
		}
	}

	// Changing scene
	public void ChangeScene()
	{
		GetTree().ChangeSceneToFile(NextLevelPath);
	}
}
