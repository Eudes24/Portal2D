using System;
using Godot;

public partial class BluePortal : Portal
{
	[Export]
	public NodePath DestinationPortalPath;
	private OrangePortal DestinationPortal;
	private bool _canTeleport = true;

	public BluePortal()
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}

	public override void _Ready()
	{
		string sceneName = GetTree().CurrentScene.Name;
		if (sceneName == "Level2")
			open = true;
		else
		{
			open = false;
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
		if (!_canTeleport)
			return;

		if (body is Player player && DestinationPortal != null && open)
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
			if (rotatedVelocity.Length() < 10f)
			{
				rotatedVelocity = DestinationPortal.GlobalTransform.X * 200f;
			}
		}
	}
}
