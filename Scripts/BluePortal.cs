using System;
using Godot;

// Here is the class that sets up the blue Portal.
public partial class BluePortal : Portal
{
	[Export]
	public NodePath DestinationPortalPath;
	private OrangePortal DestinationPortal; // To teleport to the other portal
	private bool _canTeleport = true; // Property that avoid infinite teleportation

	public BluePortal()
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered))); // Connect the OnBodyEntered Function
	}

	public override void _Ready() // Load the "Open" variable and check if the DestinationportalPath is correct
	{
		string sceneName = GetTree().CurrentScene.Name;
		if (sceneName == "Level2")
			Open = true;
		else
		{
			Open = false;
		}

		if (DestinationPortalPath != null)
		{
			DestinationPortal = GetNodeOrNull<OrangePortal>(DestinationPortalPath);
			if (DestinationPortal == null)
				GD.PrintErr("ERREUR : Impossible de trouver le portail de destination !");
		}
		else
		{
			GD.PrintErr("ERREUR : DestinationPortalPath n'est pas défini !");
		}
	}

	public void BlockTeleportTemporarily() // Avoids infinite tp
	{
		_canTeleport = false;
		var timer = new Timer();
		timer.WaitTime = 0.2f;
		timer.OneShot = true;
		AddChild(timer);
		timer.Timeout += () =>
		{
			_canTeleport = true;
			timer.QueueFree();
		};
		timer.Start();
	}

	public void OnBodyEntered(Node body) // Detect the player TP the player
	{
		if (!_canTeleport)
			return;

		if (body is Player player && DestinationPortal != null && Open)
		{
			DestinationPortal.BlockTeleportTemporarily();
			Vector2 inputVelocity = player.Velocity;
			Vector2 inNormal = -GlobalTransform.X;
			Vector2 outNormal = DestinationPortal.GlobalTransform.X;
			float angleDiff = outNormal.AngleTo(inNormal);
			Vector2 rotatedVelocity = inputVelocity.Rotated(angleDiff);

			Vector2 exitDir = -DestinationPortal.GlobalTransform.Y.Normalized();
			float offset = 20f;
			player.GlobalPosition = DestinationPortal.GlobalPosition + exitDir * offset;
			player.ForceVelocityAfterTeleport(rotatedVelocity);
			if (rotatedVelocity.Length() < 10f) //Gives a boost to the player when he'll get out the portal when the speed in almost null
			{
				rotatedVelocity = DestinationPortal.GlobalTransform.X * 200f;
			}
		}
	}
}
