using Godot;
using UIProject.Scripts;

public partial class UI : CanvasLayer
{
	// Export these so you can assign them easily in the editor
	[Export] public NodePath HealthBarPath;
	[Export] public NodePath HealthLabelPath;      // optional
	[Export] public NodePath ScoreLabelPath;
	[Export] public NodePath LivesContainerPath;
	[Export] public NodePath DamageFlashPath;
	[Export] public NodePath StartMenuPath;
	[Export] public NodePath PauseMenuPath;
	[Export] public NodePath GameOverPath;

	private ProgressBar _healthBar;
	private Label _healthLabel;         // optional
	private Label _scoreLabel;
	private HBoxContainer _livesContainer;
	private ColorRect _damageFlash;

	private Control _startMenu;
	private Control _pauseMenu;
	private Control _gameOver;

	private Player _player;

	public override void _Ready()
	{
		// Get all UI nodes
		_healthBar = GetNode<ProgressBar>(HealthBarPath);
		_healthLabel = GetNodeOrNull<Label>(HealthLabelPath);
		_scoreLabel = GetNode<Label>(ScoreLabelPath);
		_livesContainer = GetNode<HBoxContainer>(LivesContainerPath);
		_damageFlash = GetNode<ColorRect>(DamageFlashPath);

		_startMenu = GetNode<Control>(StartMenuPath);
		_pauseMenu = GetNode<Control>(PauseMenuPath);
		_gameOver = GetNode<Control>(GameOverPath);

		// Find player
		_player = GetTree().CurrentScene.GetNode<Player>("Player");
		if (_player == null)
		{
			GD.PrintErr("Player not found in scene!");
			return;
		}

		// Connect signals
		_player.HealthChanged += OnPlayerHealthChanged;
		_player.LivesChanged += OnPlayerLivesChanged;
		GetNode<GameState>("/root/GameState").ScoreChanged += OnScoreChanged;

		// Initial values
		OnPlayerHealthChanged(_player.MaxHealth, _player.MaxHealth);
		OnPlayerLivesChanged(_player.Lives);
		OnScoreChanged(0);

		// Menu setup
		_startMenu.Visible = true;
		_pauseMenu.Visible = false;
		_gameOver.Visible = false;
		GetTree().Paused = true; // start paused until player clicks start

		// Connect buttons (you can also do this in editor with signals)
		SetupButtons();
	}

	private void SetupButtons()
	{
		GetNodeOrNull<Button>("StartMenu/StartButton").Pressed += StartGame;
		GetNodeOrNull<Button>("PauseMenu/ResumeButton").Pressed += ResumeGame;
		GetNodeOrNull<Button>("PauseMenu/QuitButton").Pressed += QuitGame;
		GetNodeOrNull<Button>("GameOver/RestartButton").Pressed += RestartGame;
	}

	private void StartGame()
	{
		_startMenu.Visible = false;
		GetTree().Paused = false;
	}

	private void ResumeGame()
	{
		_pauseMenu.Visible = false;
		GetTree().Paused = false;
	}

	private void QuitGame()
	{
		GetTree().Quit();
	}

	private void RestartGame()
	{
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}

	public void ShowGameOver()
	{
		_gameOver.Visible = true;
		GetTree().Paused = true;
	}

	private void OnPlayerHealthChanged(int current, int max)
	{
		_healthBar.MaxValue = max;
		_healthBar.Value = current;

		if (_healthLabel != null)
			_healthLabel.Text = $"HP: {current}/{max}";

		// Quick damage flash effect
		var tween = CreateTween().SetParallel();
		tween.TweenProperty(_healthBar, "modulate", new Color(1.4f, 0.4f, 0.4f), 0.12f);
		tween.TweenProperty(_healthBar, "modulate", Colors.White, 0.3f);

		// Screen flash
		if (_damageFlash != null)
		{
			_damageFlash.Modulate = new Color(1, 0, 0, 0.35f);
			_damageFlash.Visible = true;
			var flashTween = CreateTween();
			flashTween.TweenProperty(_damageFlash, "modulate:a", 0f, 0.45f)
					 .SetTrans(Tween.TransitionType.Sine)
					 .SetEase(Tween.EaseType.Out);
			flashTween.TweenCallback(Callable.From(() => _damageFlash.Visible = false));
		}
	}

	private void OnPlayerLivesChanged(int lives)
	{
		for (int i = 0; i < _livesContainer.GetChildCount(); i++)
		{
			var heart = _livesContainer.GetChild<Control>(i);
			heart.Modulate = i < lives ? new Color(1, 1, 1) : new Color(0.4f, 0.4f, 0.4f, 0.6f);
			// Alternative: heart.Visible = i < lives;
		}
	}

	private void OnScoreChanged(int score)
	{
		_scoreLabel.Text = $"Score: {score:D6}";
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel")) // ESC / Pause
		{
			GetViewport().SetInputAsHandled();

			if (GetTree().Paused && !_pauseMenu.Visible && !_gameOver.Visible)
				return;

			if (_pauseMenu.Visible)
			{
				ResumeGame();
			}
			else if (!_gameOver.Visible)
			{
				_pauseMenu.Visible = true;
				GetTree().Paused = true;
			}
		}
	}
}
