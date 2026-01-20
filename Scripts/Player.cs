using Godot;

namespace UIProject.Scripts;

public partial class Player : Creature
{
	[Signal]
	public delegate void LivesChangedEventHandler(int lives);

	[Export]
	public int Lives = 3;

	private Vector2 _startPosition;
	private bool IsAttacking => _sprite.Animation.ToString() == "attack" && _sprite.IsPlaying();

	private AnimatedSprite2D _sprite;
	private Area2D _hurtBox;

	public override void _Ready()
	{
		CurrentHealth = MaxHealth;
		_startPosition = GlobalPosition;

		_sprite = GetNode<AnimatedSprite2D>("Sprite");
		_hurtBox = GetNode<Area2D>("HurtBox");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Skip all input/movement during pause or game over
		if (GetTree().Paused)
			return;

		var direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		UpdateVelocity(direction);
		UpdateDirection(direction);

		var attacking = Input.IsActionJustPressed("ui_accept");

		// Don't let the user spam attacks
		if (attacking && !IsAttacking)
			ActivateAttack();

		UpdateSpriteAnimation(direction, attacking);

		MoveAndSlide();
	}

	public void TakeDamage(int damage)
	{
		CurrentHealth -= damage;
		if (CurrentHealth < 1)
		{
			GetNode<TextureRect>($"../UI/LivesContainer/Life3").Visible = false;
		}
		if (CurrentHealth < 2)
		{
			GetNode<TextureRect>($"../UI/LivesContainer/Life2").Visible = false;
		}
		if (CurrentHealth < 3)
		{
			GetNode<TextureRect>($"../UI/LivesContainer/Life1").Visible = false;
		}
	
		

		if (CurrentHealth <= 0)
		{
			Lives -= 1;
			if (Lives <=2)
			{
				GetNode<TextureRect>($"../UI/LivesContainer/Life3").Visible = true;
			}
			if (Lives <=2)
			{
				GetNode<TextureRect>($"../UI/LivesContainer/Life2").Visible = true;
			}
			if (Lives <=2)
			{
				GetNode<TextureRect>($"../UI/LivesContainer/Life1").Visible = true;
			}
			EmitSignal(SignalName.LivesChanged, Lives);

			if (Lives <= 0)
			{
				// Show game over screen instead of quitting immediately
				var ui = GetTree().ChangeSceneToFile("res://Scenes/game_over.tscn");
				
			}
			else
			{
				// Respawn
				GlobalPosition = _startPosition;
				CurrentHealth = MaxHealth;
				GD.Print($"Player respawned - Lives remaining: {Lives}");
			}
		}

		GD.Print($"Player Health: {CurrentHealth}/{MaxHealth}");
		EmitSignal(Creature.SignalName.HealthChanged, CurrentHealth, MaxHealth);
	}

	private void UpdateVelocity(Vector2 direction)
	{
		Vector2 velocity = Velocity;

		if (direction != Vector2.Zero && !IsAttacking)
		{
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}

		Velocity = velocity;
	}

	private void UpdateDirection(Vector2 direction)
	{
		if (direction.X < 0)
		{
			_sprite.FlipH = true;
			if (_hurtBox.Position.X > 0)
				_hurtBox.Position = new Vector2(_hurtBox.Position.X * -1, _hurtBox.Position.Y);
		}
		else if (direction.X > 0)
		{
			_sprite.FlipH = false;
			if (_hurtBox.Position.X < 0)
				_hurtBox.Position = new Vector2(_hurtBox.Position.X * -1, _hurtBox.Position.Y);
		}
	}

	private void UpdateSpriteAnimation(Vector2 direction, bool attacking)
	{
		// Don't interrupt the attack animation
		if (!IsAttacking)
		{
			if (direction != Vector2.Zero)
				_sprite.Play("walk");
			else
				_sprite.Play("idle");

			// Attack has priority
			if (attacking)
			{
				_sprite.Play("attack");
				// Stop moving during attack
				Velocity = Vector2.Zero;
			}
		}
	}

	private void ActivateAttack()
	{
		var bodies = _hurtBox.GetOverlappingBodies();

		foreach (var body in bodies)
		{
			if (body is Enemy enemy)
			{
				// For this demo, each attack does 1 damage
				enemy.TakeDamage(1);
			}
		}
	}
}
