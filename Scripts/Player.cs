using System;
using Godot;

public partial class Player : CharacterBody2D
{
	public const float MaxSpeed = 250.0f;
	public const float JumpVelocity = -450.0f;
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

	public override void _Ready() // sets up all the animated sprites and timers, avoid the possibility to throw portal if the player is in the 2 first levels, Connect the portal scenes
	{
		string sceneName1 = GetTree().CurrentScene.Name;
		if (sceneName1 != "Level1" && sceneName1 != "Level2")
			Input.SetCustomMouseCursor(
				ResourceLoader.Load<Texture2D>("res://Objects_and_tiles/Normal.png")
			);
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.AnimationFinished += OnAnimationFinished;
		BluePortal = GetTree().GetCurrentScene().FindChild("BluePortal") as Portal;
		OrangePortal = GetTree().GetCurrentScene().FindChild("OrangePortal") as Portal;
		_portalAnimTimer = new Timer();
		_portalAnimTimer.WaitTime = 0.1f; // Duration when shooting animation
		_portalAnimTimer.OneShot = true;
		AddChild(_portalAnimTimer);
		_portalAnimTimer.Timeout += () =>
		{
			IsShootingPortal = false;
		};
	}

	public override void _PhysicsProcess(double delta) //All the moves and animations
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

		// Gravity
		if (!IsOnFloor())
			Velocity += GetGravity() * (float)delta;

		// horizontal moves
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		if (IsOnFloor())
		{
			Velocity = new Vector2(direction.X * MaxSpeed, Velocity.Y);
		}
		else
		{
			if (direction.X != 0)
				Velocity = new Vector2(
					Mathf.MoveToward(
						Velocity.X,
						direction.X * MaxSpeed,
						Acceleration * (float)delta
					),
					Velocity.Y
				);
			else
				Velocity = new Vector2(
					Mathf.MoveToward(Velocity.X, 0, Friction * (float)delta),
					Velocity.Y
				);
		}

		if (Input.IsActionPressed("Pause"))
		{
			((PauseMenu)GetNode("/root/PauseMenu")).TogglePause();
			GD.Print("Game paused");
		}

		// Animations
		if (IsOnFloor())
		{
			if (IsShootingPortal)
				return; // No other animation plays when the player shoots portal
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

		// Jump
		if (Input.IsActionPressed("ui_up") && IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, JumpVelocity);
			_animatedSprite.Play("Jumping");
		}

		// In the air
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

	private void OnAnimationFinished() // Plays the right animation after shooting portal
	{
		if (_animatedSprite.Animation == "ShootPortal")
		{
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
				_animatedSprite.Play("Iddle");
			}
		}
	}

	public void ForceVelocityAfterTeleport(Vector2 newVelocity) // manage the velocity when out of the portal
	{
		_nextVelocity = newVelocity;
		_forceNextVelocity = true;
	}

	public override void _UnhandledInput(InputEvent @event) { }

	private void ShootPortal(Portal portalToPlace) // shoot the portal where the player mouse is
	{
		// Turn the character to the mouse
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

			// Correct the orientation of the portal to face the player
			Vector2 portalForward = -hitNormal;
			portalToPlace.Rotation = portalForward.Angle();
		}
		else
		{
			NoAnimationShoot = true;
			GD.Print("You can't TP there");
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
				if (!NoAnimationShoot)
					_animatedSprite.Play("ShootPortal");
				_portalAnimTimer.Start();
				OrangePortal.Open = true;
			}
			else if (mouseEvent.ButtonIndex == MouseButton.Right && canShootPortal)
			{
				ShootPortal(OrangePortal);
				IsShootingPortal = true;
				if (!NoAnimationShoot)
					_animatedSprite.Play("ShootPortal");
				_portalAnimTimer.Start();
				BluePortal.Open = true;
			}
			canShootPortal = true;
		}
	}
}
