using Godot;
using System;

public partial class Killzone : Area2D
{
	public string MessageMort {get; set;}
	public Timer Decompte {get; set;}
	
	public Killzone ()
	{
		MessageMort = "Tu es mort!";
	}
	public void _on_body_entered()
	{
		GD.Print(MessageMort);
		Decompte.Start();
	}
	public void _on_timer_timout()
	{
		GetTree().ReloadCurrentScene();;
	}
}
