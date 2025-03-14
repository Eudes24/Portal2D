using Godot;
using System;

public partial class Killzone : Area2D
{
	public string MessageMort {get; set;}
	public Timer decompte {get; set;}
	
	public Killzone ()
	{
		MessageMort = "Tu es mort!";
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));

		// Créer et configurer le Timer
		decompte = new Timer();
		decompte.WaitTime = 2.0f; 
		decompte.OneShot = true;
		decompte.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
		AddChild(decompte);
	}
	public void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			GD.Print(MessageMort);
			decompte.Start();
		}
	}
	public void OnTimerTimeout()
	{
		GetTree().ReloadCurrentScene();;
	}
}
