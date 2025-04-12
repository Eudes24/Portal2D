using System;
using Godot;

public partial class Killzone : Area2D
{
	public string DeathMessage { get; set; }
	public Timer timer { get; set; }

	public Killzone()
	{
		DeathMessage = "Tu es mort!";
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));

		// Create and initialize timer
		timer = new Timer();
		timer.WaitTime = 2.0f;
		timer.OneShot = true;
		timer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
		AddChild(timer);
	}

	public void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			GD.Print(DeathMessage);
			timer.Start();
		}
	}

	public void OnTimerTimeout()
	{
		GetTree().ReloadCurrentScene();
		;
	}
}
