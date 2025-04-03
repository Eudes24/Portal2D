using Godot;
using System;

public partial class OrangePortal : Area2D
{
	[Export]
	public NodePath DestinationPortalPath; // Reference to the other portal
	private BluePortal DestinationPortal;
	public OrangePortal ()
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}

   public override void _Ready()
	{
		if (DestinationPortalPath != null)
		{
			DestinationPortal = GetNodeOrNull<BluePortal>(DestinationPortalPath);
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

	public void OnBodyEntered(Node body)
	{
		GD.Print("The player is in the portal."); // Check if OnbodyEntered is connected

		if (body is Player player && DestinationPortal != null)
		{
			GD.Print("Téléportation en cours..."); // check if DestinationPortal works
			// Teleport the player to the destination portal
			player.GlobalPosition = DestinationPortal.GlobalPosition;
		}
	}
}
