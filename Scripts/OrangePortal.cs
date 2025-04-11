using Godot;
using System;

public partial class OrangePortal : Portal
{
	[Export]
	public NodePath DestinationPortalPath;
	private BluePortal DestinationPortal;
	private bool _canTeleport = true;

	public OrangePortal()
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}

	public override void _Ready()
	{
		if (DestinationPortalPath != null)
		{
			DestinationPortal = GetNodeOrNull<BluePortal>(DestinationPortalPath);
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
		if (!_canTeleport) return;

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
			// 🔥 ICI : on applique la vélocité transformée
			player.ForceVelocityAfterTeleport(rotatedVelocity);
			if (rotatedVelocity.Length() < 10f)
			{
				rotatedVelocity = DestinationPortal.GlobalTransform.X * 200f;
			}
		}
	}
}
