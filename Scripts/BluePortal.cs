using Godot;
using System;

public partial class BluePortal : Area2D
{
	[Export]
	public NodePath DestinationPortalPath; // Reference to the other portal
	private OrangePortal DestinationPortal;
	private bool _canTeleport = true;
	public BluePortal ()
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}

   public override void _Ready()
	{
		if (DestinationPortalPath != null)
		{
			DestinationPortal = GetNodeOrNull<OrangePortal>(DestinationPortalPath);
			if (DestinationPortal == null)
			{
				GD.PrintErr("ERREUR : Impossible de trouver le portail de destination !");
			}
		}
		else
		{
			GD.PrintErr("ERREUR : DestinationPortalPath n'est pas défini !");
		}
	}

	public void BlockTeleportTemporarily()
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

	public void OnBodyEntered(Node body)
	{
		if (!_canTeleport) return;
		GD.Print("The player is in the portal."); // Check if OnbodyEntered is connected
		GD.Print("DestinationPortal: ", DestinationPortal);
		if (body is Player player && DestinationPortal != null)
		{
			GD.Print("Téléportation en cours..."); // check if DestinationPortal works
			DestinationPortal.BlockTeleportTemporarily(); // Avoid inifint TP
			// Teleport the player to the destination portal
			player.GlobalPosition = DestinationPortal.GlobalPosition;
		}
	}
}
