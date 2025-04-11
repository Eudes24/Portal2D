using System;
using Godot;

public partial class Player : CharacterBody2D
{
	public const float MaxSpeed = 250.0f;
	public const float JumpVelocity = -400.0f;
	public const float Acceleration = 1200.0f;
	public const float Friction = 800.0f;

	private Timer _portalAnimTimer;
	private AnimatedSprite2D _animatedSprite;
	public Portal OrangePortal;
	public Portal BluePortal;
	private Vector2 _nextVelocity = Vector2.Zero;
	
	private bool IsShootingPortal = false;
	private bool _forceNextVelocity = false;
	private bool _justTeleported = false;
	private bool IsSneaking = false;
	private bool InTheAir = false;
	private bool canShootPortal = true;
	private bool NoAnimationShoot = false;
	
	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.AnimationFinished += OnAnimationFinished;
		BluePortal = GetTree().GetCurrentScene().FindChild("BluePortal") as Portal;
		OrangePortal = GetTree().GetCurrentScene().FindChild("OrangePortal") as Portal;
		_portalAnimTimer = new Timer();
		_portalAnimTimer.WaitTime = 0.1f; // durée de l'animation de tir
		_portalAnimTimer.OneShot = true;
		AddChild(_portalAnimTimer);
		_portalAnimTimer.Timeout += () => {
			IsShootingPortal = false;
};

	}

	public override void _PhysicsProcess(double delta)
	{
		if (_forceNextVelocity)
		{
			Velocity = _nextVelocity;
			_forceNextVelocity = false;
			_justTeleported = true;
		}

		if (_justTeleported)
		{
			MoveAndSlide();
			_justTeleported = false;
			return;
		}

		// GRAVITY
		if (!IsOnFloor())
			Velocity += GetGravity() * (float)delta;

		// MOUVEMENT HORIZONTAL
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		if (IsOnFloor())
		{
			Velocity = new Vector2(direction.X * MaxSpeed, Velocity.Y);
		}
		else
		{
			if (direction.X != 0)
				Velocity = new Vector2(Mathf.MoveToward(Velocity.X, direction.X * MaxSpeed, Acceleration * (float)delta), Velocity.Y);
			else
				Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, Friction * (float)delta), Velocity.Y);
		}

		// Animations
		if (IsOnFloor())
		{
			if (IsShootingPortal)return; // empêche les autres animations de s’enclencher
			if (direction.X < 0)
			{
				_animatedSprite.FlipH = true;
				_animatedSprite.Play("Running");
			}
			else if (direction.X > 0)
			{
				_animatedSprite.FlipH = false;
				_animatedSprite.Play("Running");
			}
			else if (!IsShootingPortal && !IsSneaking)
			{
				_animatedSprite.Play("Iddle");
			}

			if (Input.IsActionPressed("ui_down"))
			{
				if (!IsSneaking)
				{
					_animatedSprite.Play("Sneak");
					IsSneaking = true;
				}
			}
			else
			{
				IsSneaking = false;
			}
		}

		// JUMP
		if (Input.IsActionPressed("ui_up") && IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, JumpVelocity);
			_animatedSprite.Play("Jumping");
		}

		// IN THE AIR
		if (IsOnFloor())
		{
			if (InTheAir)
				_animatedSprite.Play("Landing");
			InTheAir = false;
		}
		else
		{
			InTheAir = true;
		}
		MoveAndSlide();
	}
	private void OnAnimationFinished()
	{
		if (_animatedSprite.Animation == "ShootPortal")
		{
			// Détermine la bonne animation à jouer ensuite
			if (!IsOnFloor())
			{
				_animatedSprite.Play("Jumping");
			}
			else if (IsSneaking)
			{
				_animatedSprite.Play("Sneak");
			}
			else
			{
				_animatedSprite.Play("Idle");
			}
		}
	}

	public void ForceVelocityAfterTeleport(Vector2 newVelocity)
	{
		_nextVelocity = newVelocity;
		_forceNextVelocity = true;
	}

	private void ShootPortal(Portal portalToPlace)
	{
		// Tourner le personnage vers la souris
		Vector2 mousePos = GetGlobalMousePosition();
		if (mousePos.X < GlobalPosition.X)
			_animatedSprite.FlipH = true;
		else
			_animatedSprite.FlipH = false;
		var space = GetWorld2D().DirectSpaceState;
		Vector2 start = GlobalPosition;
		Vector2 direction = (mousePos - start).Normalized();
		float maxDistance = 10000f;

		var query = new PhysicsRayQueryParameters2D
		{
			From = start,
			To = start + direction * maxDistance,
			CollisionMask = 1,
		};

		var result = space.IntersectRay(query);

		if (result.Count > 0 && ((Node)result["collider"]).IsInGroup("Portalable"))
		{
			Vector2 hitPoint = (Vector2)result["position"];
			Vector2 hitNormal = (Vector2)result["normal"];
			float offsetDistance = 10f;
			portalToPlace.GlobalPosition = hitPoint + hitNormal * offsetDistance;
			NoAnimationShoot = false;

			// On veut que le portail "regarde" à l'opposé de la normale
			Vector2 portalForward = -hitNormal; // vers l’extérieur
			portalToPlace.Rotation = portalForward.Angle();
			
		}
		else
		{
			NoAnimationShoot = true;
			GD.Print("No wall detected");
		}
	}

	public override void _Input(InputEvent e)
	{
		if (e is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			string sceneName = GetTree().CurrentScene.Name;
			if (sceneName == "Level1" || sceneName == "Level2")
				canShootPortal = false;

			if (mouseEvent.ButtonIndex == MouseButton.Left && canShootPortal)
			{
				ShootPortal(BluePortal);
				IsShootingPortal = true;
				if (!NoAnimationShoot)_animatedSprite.Play("ShootPortal");
				_portalAnimTimer.Start();
				OrangePortal.open = true;
			}
			else if (mouseEvent.ButtonIndex == MouseButton.Right && canShootPortal)
			{
				ShootPortal(OrangePortal);
				IsShootingPortal = true;
				if (!NoAnimationShoot)_animatedSprite.Play("ShootPortal");
				_portalAnimTimer.Start();
				BluePortal.open = true;
			}
			canShootPortal = true;
		}
	}
}
