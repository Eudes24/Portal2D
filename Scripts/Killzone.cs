using System;
using Godot;

// Scripts that sets up the killzone and restart the level when the character dies
public partial class Killzone : Area2D
{
	public string DeathMessage { get; set; }
	public Timer timer { get; set; } // Sets up the timer of respawn

	public Killzone()
	{
		DeathMessage = "Tu es mort!";
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered))); // Connect OnBodyEntered function

		// Create and initialize timer
		timer = new Timer();
		timer.WaitTime = 2.0f;
		timer.OneShot = true;
		timer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
		AddChild(timer);
	}

	public void OnBodyEntered(Node body) //when the player touches the killzone hitbox
	{
		if (body is Player player)
		{
			GD.Print(DeathMessage);
			timer.Start();
		}
	}

	public void OnTimerTimeout() // reload the scene (respawn)
	{
		GetTree().ReloadCurrentScene();
		;
	}
}
