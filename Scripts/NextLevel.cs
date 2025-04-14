using System;
using Godot;

//The class that lets the player go to the next level when the player hitbox hits the NextLevel hitbox
public partial class NextLevel : Area2D
{
	public static int NumberNextLevel = 2;
	public string NextLevelPath = $"res://Scenes/Level{NumberNextLevel}.tscn";

	public NextLevel() //Connect the OnBodyEntered function
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}

	public void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			GD.Print("GG WP, go next now");
			NumberNextLevel++; // Prepare the next level "NextLevel" object

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
