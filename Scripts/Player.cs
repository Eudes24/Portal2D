using System;
using Godot;

public partial class Player : CharacterBody2D
{	
	public const float MaxSpeed = 250.0f;
	public const float JumpVelocity = -400.0f;
	public const float Acceleration = 1200.0f;
	public const float Friction = 800.0f;

	private AnimatedSprite2D _animatedSprite;
	public Area2D OrangePortal;
	public Area2D BluePortal;
	private bool IsSneaking = false;
	private bool InTheAir = false;
	private bool canShootPortal = true;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		BluePortal = GetTree().GetCurrentScene().FindChild("BluePortal") as Area2D;
		OrangePortal = GetTree().GetCurrentScene().FindChild("OrangePortal") as Area2D;
	}	
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		//gravity
		if (!IsOnFloor())
			velocity += GetGravity() * (float)delta;

		//direction keys
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		//on the ground
		if (IsOnFloor())
		{
			velocity.X = direction.X * MaxSpeed;
		}
		else //in the air
		{
			if (direction.X != 0)
			{
				velocity.X = Mathf.MoveToward(velocity.X, direction.X * MaxSpeed, Acceleration * (float)delta);
			}
			else
			{
				velocity.X = Mathf.MoveToward(velocity.X, 0, Friction * (float)delta);
			}
		}

		//sprite animations
		if (IsOnFloor())
		{
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
			
			else if (!InTheAir && !(_animatedSprite.IsPlaying() && _animatedSprite.Animation == "Landing"))
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

		//jumps
		if (Input.IsActionPressed("ui_up") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			_animatedSprite.Play("Jumping");
		}

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
		Velocity = velocity;
		MoveAndSlide();
	}
	
	private void ShootPortal(Area2D portalToPlace)
	{
		var space = GetWorld2D().DirectSpaceState;

		Vector2 start = GlobalPosition;
		Vector2 mousePos = GetGlobalMousePosition();
		Vector2 direction = (mousePos - start).Normalized();
		float maxDistance = 1000f;

		var query = new PhysicsRayQueryParameters2D
		{
			From = start,
			To = start + direction * maxDistance,
			CollisionMask = 1, // put the layer of the walls
		};

		var result = space.IntersectRay(query);

		if (result.Count > 0)
		{
			Vector2 hitPoint = (Vector2)result["position"];
			Vector2 hitNormal = (Vector2)result["normal"];

			portalToPlace.GlobalPosition = hitPoint+ hitNormal*1;
			portalToPlace.Rotation = hitNormal.Angle(); // orientation of the wall/ground

		}
		else
		{
			GD.Print("No wall detected"); // if no wall where the player shoots
		}
	}
	
	public override void _Input(InputEvent e)
	{
		if (e is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			string sceneName = GetTree().CurrentScene.Name;
			if (sceneName == "Level1" || sceneName == "Level2")
			{
				canShootPortal = false;
			}
			
			if (mouseEvent.ButtonIndex == MouseButton.Left && canShootPortal)
			{
				ShootPortal(BluePortal);
				_animatedSprite.Play("ShootPortal");
			}
			else if (mouseEvent.ButtonIndex == MouseButton.Right && canShootPortal)
			{
				ShootPortal(OrangePortal);
				_animatedSprite.Play("ShootPortal");
			}
			
			canShootPortal = true;
		}
	}
}
