using System;
using Godot;

public partial class Portal : Area2D
{
	public bool Open = false; // Avoid the TP while both portals haven't been shooted

	public override void _Ready() { }

	public override void _Process(double delta) { }
}
