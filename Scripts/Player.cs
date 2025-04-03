using System;
using Godot;

public partial class Player : CharacterBody2D
{
	public const float Speed = 250.0f;
	public const float JumpVelocity = -400.0f;
	private AnimatedSprite2D _animatedSprite;
	private bool IsSneaking = false;
	private bool InTheAir = false;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}	
	
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Ajout de gravité
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}


		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			
		}
		
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			if(!InTheAir && !(_animatedSprite.IsPlaying() && _animatedSprite.Animation == "Landing"))
			{
			_animatedSprite.Play("Immobile");
			}
		}
		if (IsOnFloor())
		{
			if (Input.IsActionPressed("ui_left"))
			{
				_animatedSprite.FlipH = true;
				_animatedSprite.Play("Running");
			} 
			
			if (Input.IsActionPressed("ui_right"))
			{
				_animatedSprite.FlipH = false;
				_animatedSprite.Play("Running");
			} 
		}
		
		if (Input.IsActionPressed("ui_down"))
		{
			if (IsSneaking == false)
			{
				_animatedSprite.Play("Sneak");
				IsSneaking = true;
			}
		} 
		else 
		{
			IsSneaking = false;
		}
		// Handle Jump.
		if (Input.IsActionJustPressed("Sauter") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			_animatedSprite.Play("Jumping");
		}
		
		if (IsOnFloor())
		{
			if (InTheAir)
			{
				_animatedSprite.Play("Landing");
			}
			InTheAir = false;
		}
		else{
			InTheAir = true;
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
